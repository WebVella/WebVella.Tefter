﻿namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderSyncLogDialog.TfDataProviderSyncLogDialog", "WebVella.Tefter")]
public partial class TucDataProviderSyncLogDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProviderSynchronizeTask?>
{
	[Parameter] public TfDataProviderSynchronizeTask? Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _title = "";
	private bool _isBusy = true;
	private ReadOnlyCollection<TfDataProviderSychronizationLogEntry> _items =
		new List<TfDataProviderSychronizationLogEntry>().AsReadOnly();
	//private int _limit = 1000;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Synchronization log");
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			try
			{
				_items = Content!.SynchronizationLog.GetEntries();
			}
			catch (Exception ex)
			{
				ProcessException(ex);
			}
			finally
			{
				_isBusy = false;
				await InvokeAsync(StateHasChanged);
			}
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}



}

