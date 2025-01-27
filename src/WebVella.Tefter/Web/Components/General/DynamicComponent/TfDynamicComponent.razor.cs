namespace WebVella.Tefter.Web.Components;
public partial class TfDynamicComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public Type Scope { get; set; }
	[Parameter] public TfBaseDynamicComponentContext Context { get; set; }
	[Parameter] public string Placeholder { get; set; } = null;
	[Parameter] public int? Count { get; set; } = null;

	private Dictionary<string, object> _context
	{
		get
		{
			var dict = new Dictionary<string, object>();
			dict["Context"] = Context;
			return dict;
		}
	}

	private ReadOnlyCollection<TfDynamicComponentMeta> _componentsMeta
	{
		get
		{
			if (Context is null) return (new List<TfDynamicComponentMeta>()).AsReadOnly();

			if (Count is not null)
			{
				return UC.GetDynamicComponentsMetaForContext(
					context: Context.GetType(),
					scope: Scope
				).Take(Count.Value).ToList().AsReadOnly();
			}
			return UC.GetDynamicComponentsMetaForContext(
				context: Context.GetType(),
				scope: Scope
			);
		}
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		StateHasChanged();
	}
}