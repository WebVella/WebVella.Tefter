namespace WebVella.Tefter.Web.Store;

//Called from various reducers
public record SpaceStateChangedAction : TfBaseAction
{
	public SpaceStateChangedAction(
		TfBaseComponent component
	)
	{
		Component = component;
	}
}
