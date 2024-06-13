﻿namespace WebVella.Tefter.Web.Components;

public class TfBaseComponent : FluxorComponent
{
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();
}
