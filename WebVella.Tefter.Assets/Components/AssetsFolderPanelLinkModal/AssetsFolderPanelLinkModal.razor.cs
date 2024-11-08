using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Assets.Components;
[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderPanelLinkModal.AssetsFolderPanelLinkModal", "WebVella.Tefter.Assets")]
public partial class AssetsFolderPanelLinkModal : TfFormBaseComponent, IDialogContentComponent<AssetsFolderPanelLinkModalContext>
{
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsFolderPanelLinkModalContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private AssetsFolderPanelLinkModalForm _form = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Create link") : LOC("Manage link");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		_form.Label = Content.Label;
		_form.Url = Content.Url;
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

			var result = new Result<Asset>();
			if (_isCreate)
			{
				var submit = new CreateLinkAssetModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					Url = _form.Url,
					CreatedBy = Content.CreatedBy,
					RowIds = Content.RowIds,
					DataProviderId = Content.DataProviderId
				};
				//result = AssetsService.CreateLinkAsset(submit);
			}
			else
			{
				throw new NotImplementedException();
			}

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

}

public class AssetsFolderPanelLinkModalForm
{
	[Required]
	public string Label { get; set; }
	[Required]
	public string Url { get; set; }
}

public class AssetsFolderPanelLinkModalContext
{
	public Guid Id { get; set; }
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string Url { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
}
