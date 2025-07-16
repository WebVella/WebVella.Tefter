namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataManageDialog.TfSpaceDataManageDialog", "WebVella.Tefter")]
public partial class TucSpaceDataManageDialog : TfFormBaseComponent, IDialogContentComponent<TfSpaceData?>
{
	[Inject] protected ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] protected ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] protected ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Parameter] public TfSpaceData? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;
	private bool _isCreate = false;

	private TfDataProvider? _selectedDataProvider = null;
	private TfSpaceData _form = new();
	private TfSpace _space = default!;
	private ReadOnlyCollection<TfDataProvider> _providers = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_space = TfSpaceUIService.GetSpace(Content.SpaceId);
		if(_space is null) throw new Exception("Space is null");
		_title = _isCreate ? LOC("Create dataset in {0}", _space.Name) : LOC("Manage dataset in {0}", _space.Name);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		if (_isCreate)
			_form = Content with { Id = Guid.NewGuid() };
		else
			_form = Content with { Id = Content.Id };
		_providers = TfDataProviderUIService.GetDataProviders();
		if (_form.DataProviderId != Guid.Empty)
			_selectedDataProvider = _providers.FirstOrDefault(x => x.Id == _form.DataProviderId);

		base.InitForm(_form);
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);

			MessageStore.Clear();

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			TfSpaceData result = default!;
			if (_isCreate)
			{
				result = TfSpaceDataUIService.CreateSpaceData(_form);
				ToastService.ShowSuccess(LOC("Space data successfully created!"));
			}
			else
			{
				result = TfSpaceDataUIService.UpdateSpaceData(_form);
				ToastService.ShowSuccess(LOC("Space data successfully updated!"));
			}

			await Dialog.CloseAsync(result);
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private void _dataProviderSelected(TfDataProvider provider)
	{
		if (provider == null) return;
		_selectedDataProvider = provider;
		_form.DataProviderId = provider.Id;
	}
}
