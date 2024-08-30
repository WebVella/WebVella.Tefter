namespace WebVella.Tefter.Web.Pages;
public partial class SpaceDataPage : TfBasePage
{
	[Parameter] public string Menu { get; set; } = string.Empty;
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceDataId { get; set; }

}