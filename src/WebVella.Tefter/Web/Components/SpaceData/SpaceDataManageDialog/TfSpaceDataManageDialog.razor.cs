﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataManageDialog.TfSpaceDataManageDialog", "WebVella.Tefter")]
public partial class TfSpaceDataManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceData>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSpaceData Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;

	private TucDataProvider _selectedDataProvider = null;
	private TucSpaceData _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content.SpaceId == Guid.Empty) throw new Exception("SpaceId is required");
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create dataset in {0}", TfAppState.Value.Space.Name) : LOC("Manage dataset in {0}", TfAppState.Value.Space.Name);
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		if (_isCreate)
			_form = Content with { Id = Guid.NewGuid() };
		else
			_form = Content with { Id = Content.Id };

		if (_form.DataProviderId != Guid.Empty)
			_selectedDataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _form.DataProviderId);

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

			TucSpaceData result = null;
			if (_isCreate)
				result = UC.CreateSpaceDataWithForm(_form);
			else
				result = UC.UpdateSpaceDataWithForm(_form);

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

	private void _dataProviderSelected(TucDataProvider provider)
	{
		if (provider == null) return;
		_selectedDataProvider = provider;
		_form.DataProviderId = provider.Id;
	}
}
