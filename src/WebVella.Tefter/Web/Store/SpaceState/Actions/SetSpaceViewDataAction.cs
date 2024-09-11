namespace WebVella.Tefter.Web.Store.SpaceState;

public record SetSpaceViewDataAction
{
	public TfDataTable SpaceViewData { get; }

	internal SetSpaceViewDataAction(
		TfDataTable spaceViewData
		)
	{
		SpaceViewData = spaceViewData;
	}
}
