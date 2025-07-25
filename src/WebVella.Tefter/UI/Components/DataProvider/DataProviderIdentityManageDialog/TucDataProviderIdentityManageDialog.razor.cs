﻿namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderIdentityManageDialog.TfDataProviderIdentityManageDialog", "WebVella.Tefter")]
public partial class TucDataProviderIdentityManageDialog : TfFormBaseComponent, IDialogContentComponent<TfDataProviderIdentity?>
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfDataIdentityUIService TfDataIdentityUIService { get; set; } = default!;
	[Parameter] public TfDataProviderIdentity? Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private bool _isCreate = false;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = default!;

	private List<string?> _providerColumns = new();
	private TfDataProvider _provider = default!;
	private TfDataProviderIdentity _form = new();

	private List<string> _identityOptions = new();
	private List<TfDataIdentity> _allDataIdentities = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		if (Content is null) throw new Exception("Content is null");
		if (Content.DataProviderId == Guid.Empty) throw new Exception("DataProvider not provided");
		_provider = TfDataProviderUIService.GetDataProvider(Content.DataProviderId);
		if(_provider == null) throw new Exception("DataProvider not found");
		if (Content.Id == Guid.Empty)
		{
			_isCreate = true;
		}
		_title = _isCreate ? LOC("Create identity implementation") : LOC("Manage identity implementation");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		_allDataIdentities = TfDataIdentityUIService.GetDataIdentities();

		_providerColumns = _provider.Columns.OrderBy(x => x.DbName).Select(x => x.DbName).ToList();
		_identityOptions = _allDataIdentities
			.Where(x=> !_provider.Identities.Any(y=> y.DataIdentity == x.DataIdentity))
			.Select(x=> x.DataIdentity).ToList();


		//Setup form
		if (_isCreate)
		{
			_form = new TfDataProviderIdentity
			{
				Id = Guid.NewGuid(),
				DataProviderId = _provider.Id,
			};
		}
		else
		{
			_identityOptions.Add(Content.DataIdentity);
			_identityOptions = _identityOptions.Order().ToList();
			_form = Content with {Id = Content.Id};
			_providerColumns = _providerColumns.Where(x => !_form.Columns.Any(y => y == x)).ToList();
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
			TfDataProvider provider;
			var submit = _form with
			{
				Id = _form.Id
			};
			if (_isCreate)
			{
				provider = TfDataProviderUIService.CreateDataProviderIdentity(submit);
				ToastService.ShowSuccess(LOC("Data identity is implemented"));
			}
			else
			{
				provider = TfDataProviderUIService.UpdateDataProviderIdentity(submit);
				ToastService.ShowSuccess(LOC("Data identity implementation is updated"));
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

