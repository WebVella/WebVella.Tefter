﻿namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceItemToolbar : WvBaseComponent
{

	private WvViewActionSelector _actionSelector;
	private async Task OnActionClick()
	{
		await _actionSelector.ToggleSelector();
	}
	private void OnActionChange(ViewActionChangedEventArgs args)
	{

	}

}