namespace WebVella.Tefter.UI.Pages;
public partial class SpaceManageAccessPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public string? Menu { get; set; }
}