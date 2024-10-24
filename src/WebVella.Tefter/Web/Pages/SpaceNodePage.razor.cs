namespace WebVella.Tefter.Web.Pages;
public partial class SpaceNodePage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpacePageId { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
	}
}