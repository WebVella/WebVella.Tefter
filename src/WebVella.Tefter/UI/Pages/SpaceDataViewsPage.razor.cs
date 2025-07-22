namespace WebVella.Tefter.UI.Pages;
public partial class SpaceDataViewsPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceDataId { get; set; }
}