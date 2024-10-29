namespace WebVella.Tefter.Web.Pages;
public partial class SpacePage : TfBasePage
{
	[Inject] public IState<TfAppState> TfAppState { get; set; }
	[Parameter] public Guid SpaceId { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		Navigator.NavigateTo(String.Format(TfConstants.SpaceNodePageUrl, SpaceId,null));
	}
}