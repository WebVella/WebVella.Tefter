namespace WebVella.Tefter.Web.Components;
public partial class TfNavigation: TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private void _addSpaceHandler(){
		ToastService.ShowToast(ToastIntent.Warning, "Will open add new space modal");
	}

}