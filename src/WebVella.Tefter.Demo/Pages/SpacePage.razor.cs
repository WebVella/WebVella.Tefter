
namespace WebVella.Tefter.Demo.Pages;
public partial class SpacePage : WvBasePage, IDisposable
{
	[Parameter]
	public Guid SpaceId { get; set; }

	[Parameter]
	public Guid SpaceItemId { get; set; }

	public void Dispose()
	{
		Navigator.LocationChanged -= onChangeLocation;
	}

	protected override void OnInitialized()
	{
		WvState.SetActiveSpaceData(SpaceId, SpaceItemId);
		Navigator.LocationChanged += onChangeLocation;
	}

	protected void onChangeLocation(object sender, LocationChangedEventArgs e)
	{
		var uri = new Uri(e.Location);
		if(!uri.LocalPath.StartsWith("/space/")) return;
		Guid spaceId = Guid.Empty;
		Guid spaceItemId = Guid.Empty;

		var localPathArray = uri.LocalPath.Split('/',StringSplitOptions.RemoveEmptyEntries);
		if(Guid.TryParse(localPathArray[1],out spaceId))
		if (Guid.TryParse(localPathArray[3], out spaceItemId))

		if(spaceId != Guid.Empty && spaceItemId != Guid.Empty)
			WvState.SetActiveSpaceData(spaceId, spaceItemId);

		StateHasChanged();
	}

}