namespace WebVella.Tefter.UI.Components;
public partial class TucScreenRegionComponent
{
	[Inject] private ITfMetaUIService TfMetaUIService { get; set; } = default!;
	[Parameter] public TfScreenRegionScope? Scope { get; set; }
	[Parameter] public TfBaseScreenRegionContext RegionContext { get; set; } = default!;
	[Parameter] public string? Placeholder { get; set; } = null;
	[Parameter] public int? Count { get; set; } = null;

	private Dictionary<string, object> _context
	{
		get
		{
			var dict = new Dictionary<string, object>();
			dict[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = RegionContext;
			return dict;
		}
	}

	private ReadOnlyCollection<TfScreenRegionComponentMeta> _componentsMeta
	{
		get
		{
			if (RegionContext is null) return (new List<TfScreenRegionComponentMeta>()).AsReadOnly();

			if (Count is not null)
			{
				return TfMetaUIService.GetRegionComponentsMetaForContext(
					context: RegionContext.GetType(),
					scope: Scope
				).Take(Count.Value).ToList().AsReadOnly();
			}
			return TfMetaUIService.GetRegionComponentsMetaForContext(
				context: RegionContext.GetType(),
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