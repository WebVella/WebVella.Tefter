namespace WebVella.Tefter.UI.Pages;
public partial class SpaceViewColumnsPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceViewId { get; set; }
	[Parameter] public string Menu { get; set; }

}