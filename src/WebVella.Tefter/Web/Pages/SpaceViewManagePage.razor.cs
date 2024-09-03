namespace WebVella.Tefter.Web.Pages;
public partial class SpaceViewManagePage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceDataId { get; set; }

}