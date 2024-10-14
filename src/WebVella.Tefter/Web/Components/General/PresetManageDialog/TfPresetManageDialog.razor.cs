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
		_form = new TucSpaceViewPreset
		{
			Id = Content.Item.Id,
			IsGroup = Content.Item.IsGroup,
			Name = Content.Item.Name,
			Filters = Content.Item.Filters.ToList(),
			SortOrders = Content.Item.SortOrders.ToList(),
			Nodes = Content.Item.Nodes.ToList()
		};
		base.InitForm(_form);
	}


	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		await Dialog.CloseAsync(_form);
	}



}
