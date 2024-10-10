namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	private TfSpaceViewActionSelector _actionSelector;
	private Task OnActionClick()
	{
		_actionSelector.ToggleSelector();
		return Task.CompletedTask;
	}
	//private void OnActionChange(SpaceViewActionChangedEventArgs args)
	//{

	//}

}