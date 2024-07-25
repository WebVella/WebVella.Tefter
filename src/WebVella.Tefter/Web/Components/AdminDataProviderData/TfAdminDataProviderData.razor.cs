﻿
using WebVella.Tefter.Web.Components.DataProviderSyncLogDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderData;
public partial class TfAdminDataProviderData : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
	}

	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		if (action.Provider is null) return;
		base.InvokeAsync(async () =>
		{
			UC.IsBusy = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(2000);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		});

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await Task.Delay(2000);
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}



}