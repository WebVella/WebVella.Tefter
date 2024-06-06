namespace WebVella.Tefter.Demo.Pages;
public partial class SpacePage : WvBasePage
{
	[Parameter]
	public Guid SpaceId { get; set; }

	[Parameter]
	public Guid SpaceDataId { get; set; }

	[Parameter]
	public Guid? SpaceViewId { get; set; }

}