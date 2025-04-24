namespace WebVella.Tefter.Talk.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TalkChannelManageDialog.TalkChannelManageDialog", "WebVella.Tefter.Talk")]
public partial class TalkChannelManageDialog : TfFormBaseComponent, IDialogContentComponent<TalkChannel>
{
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
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
	private List<string> _sharedColumnsOptions = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create channel") : LOC("Manage channel");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon.WithColor(Color.Neutral) : TfConstants.SaveIcon.WithColor(Color.Neutral);
		if (_isCreate)
		{
			_form = _form with { Id = Guid.NewGuid() };
		}
		else
		{

			_form = Content with { Id = Content.Id };
		}
		base.InitForm(_form);
		if(TfAuxDataState.Value.Data.ContainsKey(TalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY))
			_sharedColumnsOptions = ((List<TfSharedColumn>)TfAuxDataState.Value.Data[TalkConstants.TALK_APP_SHARED_COLUMNS_LIST_DATA_KEY]).Select(x=> x.DbName).ToList();
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

			var result = new TalkChannel();
			if (_isCreate)
			{
				result = TalkService.CreateChannel(_form);
			}
			else
			{
				result = TalkService.UpdateChannel(_form);
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
