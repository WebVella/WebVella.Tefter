namespace WebVella.Tefter.UI.Pages;
public partial class SpacePageManageTabPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid PageId { get; set; }
	[Parameter] public string? Tab { get; set; }
}