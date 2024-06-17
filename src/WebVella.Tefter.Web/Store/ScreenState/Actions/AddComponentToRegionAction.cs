namespace WebVella.Tefter.Web.Store.ScreenState;

public class AddComponentToRegionAction
{
	public List<ScreenRegionComponent> ScreenComponents { get; }

	public AddComponentToRegionAction(List<ScreenRegionComponent> screenComponents)
	{
		ScreenComponents = screenComponents;
	}
}
