namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.PresetManageDialog.TfPresetManageDialog", "WebVella.Tefter")]
public partial class TfPresetManageDialog : TfFormBaseComponent, IDialogContentComponent<TucPresetManagementContext>
{
	[Parameter] public TucPresetManagementContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private TucSpaceViewPreset _form = new();
	private bool _isSubmitting = false;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		base.InitForm(_form);
	}


	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private void _save()
	{

	}



}
