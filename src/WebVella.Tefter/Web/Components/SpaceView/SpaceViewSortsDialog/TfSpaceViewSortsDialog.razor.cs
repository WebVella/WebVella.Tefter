﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewSortsDialog.TfSpaceViewSortsDialog","WebVella.Tefter")]
public partial class TfSpaceViewSortsDialog : TfFormBaseComponent, IDialogContentComponent<bool>
{
	[Parameter] public bool Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
	}


	private async Task _submit()
	{
		
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
