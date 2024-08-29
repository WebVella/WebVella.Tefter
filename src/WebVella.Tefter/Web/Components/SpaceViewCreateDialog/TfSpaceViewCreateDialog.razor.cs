namespace WebVella.Tefter.Web.Components.SpaceViewCreateDialog;
public partial class TfSpaceViewCreateDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceView>
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public TucSpaceView Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	private TucDataProvider _selectedDataProvider = null;
	private TucSpaceData _selectedDataset = null;
	private List<string> _generatedColumns = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if(Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		await UC.Init(this.GetType(), Content.SpaceId);
		base.InitForm(UC.SpaceViewManageForm);
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create view in {0}", UC.SpaceName) : LOC("Manage view in {0}", UC.SpaceName);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			//if presets are available
			UC.SpaceViewManageForm = new TucSpaceView()
			{
				Id = Guid.NewGuid(),
				SpaceId = Content.SpaceId,
			};
			_generatedColumnsListInit();
			base.InitForm(UC.SpaceViewManageForm);
			await InvokeAsync(StateHasChanged);
		}
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

			var result = UC.CreateSpaceViewWithForm(UC.SpaceViewManageForm);

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				await Dialog.CloseAsync(result.Value);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
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

	private void _dataProviderSelected(TucDataProvider provider)
	{
		_selectedDataProvider = provider;
		UC.SpaceViewManageForm.DataProviderId = provider is null ? null : provider.Id;
		_generatedColumnsListInit();
	}

	private void _datasetSelected(TucSpaceData dataset)
	{
		_selectedDataset = dataset;
		UC.SpaceViewManageForm.SpaceDataId = dataset is null ? null : dataset.Id;
	}

	private void _columnGeneratorSettingChanged(bool value, string field)
	{

		if (field == nameof(UC.SpaceViewManageForm.AddProviderColumns))
		{
			UC.SpaceViewManageForm.AddProviderColumns = value;
		}
		else if (field == nameof(UC.SpaceViewManageForm.AddSharedColumns))
		{
			UC.SpaceViewManageForm.AddSharedColumns = value;
		}
		else if (field == nameof(UC.SpaceViewManageForm.AddSystemColumns))
		{
			UC.SpaceViewManageForm.AddSystemColumns = value;
		}
		_generatedColumnsListInit();
	}

	private void _generatedColumnsListInit()
	{
		_generatedColumns.Clear();
		if (_selectedDataProvider is null || !UC.SpaceViewManageForm.AddsColumns)
			return;

		if (UC.SpaceViewManageForm.AddProviderColumns)
			_generatedColumns.AddRange(_selectedDataProvider.Columns.Select(x => x.DbName));
		if (UC.SpaceViewManageForm.AddSystemColumns)
			_generatedColumns.AddRange(_selectedDataProvider.SystemColumns.Select(x => x.DbName));
		if (UC.SpaceViewManageForm.AddSharedColumns)
			_generatedColumns.AddRange(_selectedDataProvider.SharedColumns.Select(x => x.DbName));
	}
}
