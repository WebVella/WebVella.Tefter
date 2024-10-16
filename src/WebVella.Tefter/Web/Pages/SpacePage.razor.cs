namespace WebVella.Tefter.Web.Pages;
public partial class SpacePage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, SpaceId, null));
	}
}