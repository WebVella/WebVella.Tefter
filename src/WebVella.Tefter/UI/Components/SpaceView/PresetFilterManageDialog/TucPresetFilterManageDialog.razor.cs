namespace WebVella.Tefter.UI.Components;
public partial class TucPresetFilterManageDialog : TfFormBaseComponent, IDialogContentComponent<TfPresetFilterManagementContext>
{
	[Parameter] public TfPresetFilterManagementContext Content { get; set; } = default!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private TfSpaceViewPreset _form = new();
	private TfSpaceViewPreset? _selectedParent = null;
	private bool _isSubmitting = false;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.Item is not null)
		{
			_form = new TfSpaceViewPreset
			{
				Id = Content.Item.Id,
				IsGroup = Content.Item.IsGroup,
				Name = Content.Item.Name,
				Filters = Content.Item.Filters.ToList(),
				SortOrders = Content.Item.SortOrders.ToList(),
				Presets = Content.Item.Presets.ToList(),
				ParentId = Content.Item.ParentId,
				Color = Content.Item.Color,
				Icon = Content.Item.Icon
			};
			if (_form.ParentId != null)
			{
				_selectedParent = Content.Parents.FirstOrDefault(x => x.Id == _form.ParentId);
			}
		}
		base.InitForm(_form);
	}


	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		_form.ParentId = _selectedParent?.Id;
		await Dialog.CloseAsync(_form);
	}



}
