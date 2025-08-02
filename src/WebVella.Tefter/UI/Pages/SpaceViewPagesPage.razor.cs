namespace WebVella.Tefter.UI.Pages;
public partial class SpaceViewPagesPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceViewId { get; set; }
}