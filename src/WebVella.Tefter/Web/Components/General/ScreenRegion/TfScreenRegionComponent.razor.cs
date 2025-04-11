﻿namespace WebVella.Tefter.Web.Components;
public partial class TfScreenRegionComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TfScreenRegionScope Scope { get; set; }
	[Parameter] public TfBaseScreenRegionContext RegionContext { get; set; }
	[Parameter] public string Placeholder { get; set; } = null;
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
				return UC.GetRegionComponentsMetaForContext(
					context: RegionContext.GetType(),
					scope: Scope
				).Take(Count.Value).ToList().AsReadOnly();
			}
			return UC.GetRegionComponentsMetaForContext(
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