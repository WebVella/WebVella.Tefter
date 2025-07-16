namespace WebVella.Tefter.UI.Pages;
public partial class SpaceDataDetailsPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceDataId { get; set; }
	[Parameter] public string? Path { get; set; }
}