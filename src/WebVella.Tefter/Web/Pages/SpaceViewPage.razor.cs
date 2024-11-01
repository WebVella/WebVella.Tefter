namespace WebVella.Tefter.Web.Pages;
public partial class SpaceViewPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceViewId { get; set; }
	[Parameter] public string Menu { get; set; }

}