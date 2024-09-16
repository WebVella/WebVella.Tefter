namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.DataProviderKeyManageDialog.TfDataProviderKeyManageDialog","WebVella.Tefter")]
public partial class TfDataProviderKeyManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderSharedKey>
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] private IState<TfAppState> TfState { get; set; }
	[Parameter] public TucDataProviderSharedKey Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isCreate = false;
	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private List<TucDataProviderColumn> _providerColumns = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());

		if (Content is null) throw new Exception("Content is null");
		if (TfState.Value.Provider is null) throw new Exception("DataProvider not provided");
		if (TfState.Value.Provider.ProviderType.SupportedSourceDataTypes is null
		|| !TfState.Value.Provider.ProviderType.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");

		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create key") : LOC("Manage key");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;

		base.InitForm(UC.KeyForm);

	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _loadDataAsync()
	{
		_isBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			_providerColumns = TfState.Value.Provider.Columns.OrderBy(x => x.DbName).ToList();
			//Setup form
			if (_isCreate)
			{
				UC.KeyForm = new TucDataProviderSharedKeyForm
				{
					Id = Guid.NewGuid(),
					DataProviderId = TfState.Value.Provider.Id,
				};
			}
			else
			{
				UC.KeyForm = new TucDataProviderSharedKeyForm(Content);
				_providerColumns = _providerColumns.Where(x=> !UC.KeyForm.Columns.Any(y=> y.Id == x.Id)).ToList();
			}
			base.InitForm(UC.KeyForm);
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
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

			////Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			Result<TucDataProvider> submitResult;
			var submit = UC.KeyForm with
			{
				Id = UC.KeyForm.Id
			};
			if (_isCreate)
			{
				submitResult = UC.CreateDataProviderKey(submit);
			}
			else
			{
				submitResult = UC.UpdateDataProviderKey(submit);
			}

			ProcessFormSubmitResponse(submitResult);
			  if (submitResult.IsSuccess)
			{
				await Dialog.CloseAsync(submitResult.Value);
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

	private void onSelectionUpdate(FluentSortableListEventArgs args)
	{
		if (args is null || args.OldIndex == args.NewIndex)
		{
			return;
		}

		var oldIndex = args.OldIndex;
		var newIndex = args.NewIndex;

		var items = this.UC.KeyForm.Columns;
		var itemToMove = items[oldIndex];
		items.RemoveAt(oldIndex);

		if (newIndex < items.Count)
		{
			items.Insert(newIndex, itemToMove);
		}
		else
		{
			items.Add(itemToMove);
		}
	}
	private void onSelectionRemove(FluentSortableListEventArgs args)
	{
		if (args is null)
		{
			return;
		}
		// get the item at the old index in list 2
		var item = UC.KeyForm.Columns[args.OldIndex];

		// add it to the new index in list 1
		_providerColumns.Insert(args.NewIndex, item);

		// remove the item from the old index in list 2
		UC.KeyForm.Columns.Remove(UC.KeyForm.Columns[args.OldIndex]);
	}

	private void onProviderRemove(FluentSortableListEventArgs args)
	{
		if (args is null)
		{
			return;
		}
		// get the item at the old index in list 2
		var item = _providerColumns[args.OldIndex];

		// add it to the new index in list 1
		UC.KeyForm.Columns.Insert(args.NewIndex, item);

		// remove the item from the old index in list 2
		_providerColumns.Remove(_providerColumns[args.OldIndex]);
	}

}

