namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewHeaderNavigationItem.TfSpaceViewHeaderNavigationItem", "WebVella.Tefter")]
public partial class TfSpaceViewHeaderNavigationItem : TfBaseComponent
{
	[Parameter]
	public TucSpaceViewPreset Item { get; set; }
	[Parameter]
	public EventCallback<Guid> OnClick { get; set; }


	private async Task _onClick(Guid itemId)
	{
		await OnClick.InvokeAsync(itemId);
	}
}