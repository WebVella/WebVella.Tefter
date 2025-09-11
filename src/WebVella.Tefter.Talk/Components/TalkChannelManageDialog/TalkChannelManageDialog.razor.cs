namespace WebVella.Tefter.Talk.Components;
public partial class TalkChannelManageDialog : TfFormBaseComponent, IDialogContentComponent<TalkChannel>
{
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkChannel Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TalkChannel _form = new();
	private List<TfSharedColumn> _sharedColumns = new();
	private List<string> _sharedColumnsOptions = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create channel") : LOC("Manage channel");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add") : TfConstants.GetIcon("Save");
		_sharedColumns = TalkService.GetAllSharedColumns();
		_sharedColumnsOptions = _sharedColumns.Where(x => x.DbType == TfDatabaseColumnType.Number
						|| x.DbType == TfDatabaseColumnType.ShortInteger
						|| x.DbType == TfDatabaseColumnType.Integer
						|| x.DbType == TfDatabaseColumnType.LongInteger)
				.Select(x => x.DbName).Order().ToList();
		if (_isCreate)
		{
			_form.Id = Guid.NewGuid();
			if (_sharedColumnsOptions.Count > 0)
				_form.CountSharedColumnName = _sharedColumnsOptions[0];
		}
		else
		{

			_form = Content with { Id = Content.Id };
			var sharedColumn = _sharedColumns.FirstOrDefault(x => x.DbName == _form.CountSharedColumnName);
			if (sharedColumn is null || _form.DataIdentity != sharedColumn.DataIdentity)
				_form.CountSharedColumnName = null;
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

			if (String.IsNullOrWhiteSpace(_form.CountSharedColumnName))
			{
				MessageStore.Add(EditContext.Field(nameof(_form.CountSharedColumnName)), LOC("required"));
			}
			else
			{
				var sharedColumn = _sharedColumns.FirstOrDefault(x => x.DbName == _form.CountSharedColumnName);
				if (sharedColumn is null)
				{
					MessageStore.Add(EditContext.Field(nameof(_form.CountSharedColumnName)), LOC("not found"));
				}
				else
				{
					_form.DataIdentity = sharedColumn.DataIdentity;
				}
			}
			if (String.IsNullOrWhiteSpace(_form.DataIdentity))
			{
				MessageStore.Add(EditContext.Field(nameof(_form.CountSharedColumnName)), LOC("no data identity defined for this column"));
			}

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new TalkChannel();
			if (_isCreate)
			{
				result = TalkService.CreateChannel(_form);
				ToastService.ShowSuccess(LOC("Channel successfully created!"));
			}
			else
			{
				result = TalkService.UpdateChannel(new UpdateTalkChannelModel(_form.Id,_form.Name));
				ToastService.ShowSuccess(LOC("Channel successfully updated!"));
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

}
