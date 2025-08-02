namespace WebVella.Tefter.UI.Pages;
public partial class SpaceViewDetailsPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceViewId { get; set; }
}