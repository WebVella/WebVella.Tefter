using Microsoft.AspNetCore.Components.Web;

namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewHeaderNavigationItem.TfSpaceViewHeaderNavigationItem", "WebVella.Tefter")]
public partial class TfSpaceViewHeaderNavigationItem : TfBaseComponent
{
	[Parameter]
	public TucSpaceViewPreset Item { get; set; }

	[Parameter]
	public EventCallback<Guid> OnClick { get; set; }

	[Parameter]
	public Dictionary<Guid, List<Guid>> SelectionDictionary { get; set; } = new();

	[Parameter]
	public Guid? ActivePresetId { get; set; } = null;

	private async Task _onClick(MouseEventArgs args) => await OnClick.InvokeAsync(Item.Id);

}