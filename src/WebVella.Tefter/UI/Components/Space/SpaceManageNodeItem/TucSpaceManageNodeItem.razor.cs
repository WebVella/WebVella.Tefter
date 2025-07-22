namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Space.SpaceManageNodeItem.TfSpaceManageNodeItem", "WebVella.Tefter")]
public partial class TucSpaceManageNodeItem : TfBaseComponent
{
	[Parameter]
	public bool IsFirst { get; set; } = false;

	[Parameter]
	public bool IsLast { get; set; } = false;

	[Parameter]
	public TfSpacePage Item { get; set; } = default!;

	[Parameter]
	public int Level { get; set; } = 0;

	[Parameter]
	public EventCallback<TfSpacePage> OnRemove { get; set; }

	[Parameter]
	public EventCallback<Guid> OnEdit { get; set; }

	[Parameter]
	public EventCallback<Guid> OnCopy { get; set; }

	[Parameter]
	public EventCallback<Tuple<TfSpacePage,bool>> OnMove { get; set; }

	private async Task _onRemove(){ 
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space node removed?")))
			return;
		await OnRemove.InvokeAsync(Item);
	}

	private async Task _onMove(bool isUp){ 
		await OnMove.InvokeAsync(new Tuple<TfSpacePage, bool>(Item,isUp));
	}

	private async Task _onEdit(){ 
		await OnEdit.InvokeAsync(Item.Id);
	}

	private async Task _onCopy(){ 
		await OnCopy.InvokeAsync(Item.Id);
	}
}
