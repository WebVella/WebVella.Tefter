namespace WebVella.Tefter.Web.Pages;
public partial class SpaceNodePage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceNodeId { get; set; }
}