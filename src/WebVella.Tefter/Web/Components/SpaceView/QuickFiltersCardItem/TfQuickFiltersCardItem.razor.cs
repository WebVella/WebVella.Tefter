﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.QuickFiltersCardItem.TfQuickFiltersCardItem", "WebVella.Tefter")]
public partial class TfQuickFiltersCardItem : TfBaseComponent
{
	[Parameter]
	public bool IsFirst { get; set; } = false;

	[Parameter]
	public bool IsLast { get; set; } = false;

	[Parameter]
	public TucSpaceViewPreset Item { get; set; }

	[Parameter]
	public int Level { get; set; } = 0;

	[Parameter]
	public EventCallback<Guid> OnRemove { get; set; }

	[Parameter]
	public EventCallback<Guid> OnEdit { get; set; }

	[Parameter]
	public EventCallback<Guid> OnCopy { get; set; }

	[Parameter]
	public EventCallback<Tuple<Guid,bool>> OnMove { get; set; }

	private async Task _onRemove(){ 
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this quick filter removed?")))
			return;
		await OnRemove.InvokeAsync(Item.Id);
	}

	private async Task _onMove(bool isUp){ 
		await OnMove.InvokeAsync(new Tuple<Guid, bool>(Item.Id,isUp));
	}

	private async Task _onEdit(){ 
		await OnEdit.InvokeAsync(Item.Id);
	}

	private async Task _onCopy(){ 
		await OnCopy.InvokeAsync(Item.Id);
	}
}
