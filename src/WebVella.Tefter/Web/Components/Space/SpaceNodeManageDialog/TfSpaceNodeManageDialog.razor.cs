namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceNodeManageDialog.TfSpaceNodeManageDialog", "WebVella.Tefter")]
public partial class TfSpaceNodeManageDialog : TfFormBaseComponent, IDialogContentComponent<TucSpaceNode>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucSpaceNode Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private TucSpaceNode _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create space node") : LOC("Manage space node");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		if (_isCreate)
		{
			_form = _form with { Id = Guid.NewGuid(), SpaceId = TfAppState.Value.Space.Id };
		}
		else
		{

			_form = Content with { Id = Content.Id };

		}
		base.InitForm(_form);
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;

		//if (String.IsNullOrEmpty(_form.Name))
		//{
		//	ToastService.ShowWarning(LOC("Please select a name"));
		//	return;
		//}

		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);
		var node = new TucSpaceNode
		{
			Id = Guid.NewGuid(),
			ParentId = _form.ParentId,
			Name = _form.Name,
			Type = _form.Type,
			ParentNode = _form.ParentNode,
			Position = null,
			SpaceId = _form.SpaceId,
			Icon = _form.Type == TfSpaceNodeType.Page ? "Document" : "Folder"
		};

		try
		{
			Result<List<TucSpaceNode>> submitResult = UC.CreateSpaceNode(node);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space page created!"));
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceNodes = submitResult.Value
				}
				));
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
}
