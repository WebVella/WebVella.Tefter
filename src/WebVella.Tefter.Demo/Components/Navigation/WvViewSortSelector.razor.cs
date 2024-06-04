﻿
namespace WebVella.Tefter.Demo.Components;
public partial class WvViewSortSelector : WvBaseComponent
{
	[Parameter]
	public EventCallback<ViewSortChangedEventArgs> SortChanged { get; set; }

	private bool _open = false;
	private bool _selectorLoading = false;

	private void _init(){ 
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			_init();

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}