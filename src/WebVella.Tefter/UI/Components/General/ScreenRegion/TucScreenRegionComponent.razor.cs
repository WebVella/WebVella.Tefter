namespace WebVella.Tefter.UI.Components;
public partial class TucScreenRegionComponent
{
	[Inject] public ITfMetaService TfMetaService { get; set; } = null!;
	[Parameter] public TfScreenRegionScope? Scope { get; set; }
	[Parameter] public TfBaseScreenRegion Region { get; set; } = null!;
	[Parameter] public string? Placeholder { get; set; } = null;
	[Parameter] public int? Count { get; set; } = null;

	private Dictionary<string, object> _context
	{
		get
		{
			var dict = new Dictionary<string, object>();
			dict[TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = Region;
			return dict;
		}
	}

	private ReadOnlyCollection<TfScreenRegionComponentMeta> _componentsMeta
	{
		get
		{
			if (Region is null) return (new List<TfScreenRegionComponentMeta>()).AsReadOnly();

			if (Count is not null)
			{
				return TfMetaService.GetRegionComponentsMeta(
					context: Region.GetType(),
					scope: Scope
				).Take(Count.Value).ToList().AsReadOnly();
			}
			return TfMetaService.GetRegionComponentsMeta(
				context: Region.GetType(),
				scope: Scope
			);
		}
	}
}