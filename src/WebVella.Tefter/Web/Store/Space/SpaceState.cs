namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;

public partial record TfState
{
	public bool IsBusy { get; init; } = true;
	public Guid? RouteSpaceId { get; init; }
	public Guid? RouteSpaceDataId { get; init; }
	public Guid? RouteSpaceViewId { get; init; }
	public TucSpace Space { get; init; }
	public OfficeColor SpaceColor
	{
		get
		{
			if (Space is null) return OfficeColor.Excel;

			return Space.Color;
		}
	}
	public string SpaceColorString
	{
		get
		{
			if (RouteSpaceId is null || Space is null) return TfConstants.DefaultThemeColor.ToAttributeValue();

			return Space.Color.ToAttributeValue();
		}
	}
	public string SpaceIconColorString
	{
		get => RenderUtils.ChangeColorDarknessHex(SpaceColorObject, (float)0.25);
	}
	public SystemColor SpaceColorObject
	{
		get
		{
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(SpaceColorString);
		}
	}
	public string SpaceBackgkroundColor => $"{SpaceColorString}25";
	public string SpaceBorderColor => $"{SpaceColorString}75";
	public string SpaceBackgroundAccentColor => $"{SpaceColorString}35";
	public string SpaceSidebarStyle => $"background-color:{SpaceBackgkroundColor} !important; border-color:{SpaceBorderColor} !important";

	//Space Data
	public List<TucSpaceData> SpaceDataList { get; init; } = new();
	public TucSpaceData SpaceData { get; init; }

	//Space view
	public List<TucSpaceView> SpaceViewList { get; init; } = new();
	public TucSpaceView SpaceView { get; init; }
	public TfDataTable SpaceViewData { get; init; }
	public List<TucSpaceViewColumn> SpaceViewColumns { get; init; } = new();
	public int Page { get; init; } = 1;
	public int PageSize { get; init; } = TfConstants.PageSize;
	public string SearchQuery { get; init; }
	public List<TucFilterBase> Filters { get; init; }
	public List<TucSort> Sorts { get; init; }
	public List<Guid> SelectedDataRows { get; init; } = new();
	public bool AllDataRowsSelected
	{
		get
		{
			if(SpaceViewData is null || SpaceViewData.Rows.Count == 0) return false;
			var allSelected = true;
			for (int i = 0; i < SpaceViewData.Rows.Count; i++)
			{
				var rowId = (Guid)SpaceViewData.Rows[i][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
				if(!SelectedDataRows.Contains(rowId))
				{ 
					allSelected = false;
					break;
				}
			}

			return allSelected;
		}
	}
}
