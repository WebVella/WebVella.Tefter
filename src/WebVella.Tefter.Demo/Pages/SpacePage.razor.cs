

namespace WebVella.Tefter.Demo.Pages;
public partial class SpacePage : WvBasePage, IDisposable
{
	[Parameter]
	public Guid SpaceId { get; set; }

	[Parameter]
	public Guid SpaceItemId { get; set; }

	[Parameter]
	public Guid? SpaceItemViewId { get; set; }


	public void Dispose()
	{
		Navigator.LocationChanged -= onChangeLocation;
	}

	protected override void OnInitialized()
	{
		WvState.SetActiveSpaceData(SpaceId, SpaceItemId, SpaceItemViewId);
		Navigator.LocationChanged += onChangeLocation;
	}

	protected void onChangeLocation(object sender, LocationChangedEventArgs e)
	{
		if(SpaceId != Guid.Empty && SpaceItemId != Guid.Empty)
			WvState.SetActiveSpaceData(SpaceId, SpaceItemId, SpaceItemViewId);
	}

}