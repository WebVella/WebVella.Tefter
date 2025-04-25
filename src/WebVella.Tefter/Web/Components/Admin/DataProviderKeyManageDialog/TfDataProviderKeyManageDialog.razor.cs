namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderKeyManageDialog.TfDataProviderKeyManageDialog", "WebVella.Tefter")]
public partial class TfDataProviderKeyManageDialog : TfFormBaseComponent, IDialogContentComponent<TucDataProviderJoinKey>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] private IState<TfAppState> TfAppState { get; set; }
	[Parameter] public TucDataProviderJoinKey Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isCreate = false;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;

	private List<TucDataProviderColumn> _providerColumns = new();
	private TucDataProviderJoinKeyForm _form = new();

	private List<string> _allJoinKeys = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		if (Content is null) throw new Exception("Content is null");
		if (TfAppState.Value.AdminDataProvider is null) throw new Exception("DataProvider not provided");
		if (TfAppState.Value.AdminDataProvider.ProviderType.SupportedSourceDataTypes is null
		|| !TfAppState.Value.AdminDataProvider.ProviderType.SupportedSourceDataTypes.Any()) throw new Exception("DataProvider does not have source supported types");

		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create key") : LOC("Manage key");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		_allJoinKeys = await UC.GetAllJoinKeysAsync();
		_providerColumns = TfAppState.Value.AdminDataProvider.Columns.OrderBy(x => x.DbName).ToList();
		//Setup form
		if (_isCreate)
		{
			_form = new TucDataProviderJoinKeyForm
			{
				Id = Guid.NewGuid(),
				DataProviderId = TfAppState.Value.AdminDataProvider.Id,
			};
		}
		else
		{
			_form = new TucDataProviderJoinKeyForm(Content);
			_providerColumns = _providerColumns.Where(x => !_form.Columns.Any(y => y.Id == x.Id)).ToList();
		}
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

			////Check form
			var isValid = EditContext.Validate();
			if (!isValid) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			TucDataProvider provider;
			var submit = _form with
			{
				Id = _form.Id
			};
			if (_isCreate)
			{
				provider = UC.CreateDataProviderKey(submit);
			}
			else
			{
				provider = UC.UpdateDataProviderKey(submit);
			}

			await Dialog.CloseAsync(provider);
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

	private void onSelectionUpdate(FluentSortableListEventArgs args)
	{
		if (args is null || args.OldIndex == args.NewIndex)
		{
			return;
		}

		var oldIndex = args.OldIndex;
		var newIndex = args.NewIndex;

		var items = this._form.Columns;
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
		var item = _form.Columns[args.OldIndex];

		// add it to the new index in list 1
		_providerColumns.Insert(args.NewIndex, item);

		// remove the item from the old index in list 2
		_form.Columns.Remove(_form.Columns[args.OldIndex]);
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
		_form.Columns.Insert(args.NewIndex, item);

		// remove the item from the old index in list 2
		_providerColumns.Remove(_providerColumns[args.OldIndex]);
	}

}

