using System.ComponentModel.DataAnnotations;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Assets.Components;
[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderPanelFileModal.AssetsFolderPanelFileModal", "WebVella.Tefter.Assets")]
public partial class AssetsFolderPanelFileModal : TfFormBaseComponent, IDialogContentComponent<AssetsFolderPanelFileModalContext>
{
	[Inject] public IState<TfAppState> TfAppState { get; set; }
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsFolderPanelFileModalContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private AssetsFolderPanelFileModalForm _form = new();
	private FluentInputFileEventArgs _upload = null;
	private string _uploadId = $"tf-{Guid.NewGuid()}";
	FluentInputFile? fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Upload file") : LOC("Update file");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		_form.Label = Content.Label;
		_form.LocalPath = null;
		base.InitForm(_form);
	}

	private async Task _onCompleted(IEnumerable<FluentInputFileEventArgs> files)
	{
		Files = files.ToList();
		progressPercent = fileUploader!.ProgressPercent;

		if (Files.Count > 0)
		{
			_upload = Files[0];
			if (_upload is not null)
			{
				_form.LocalPath = _upload.LocalFile.ToString();
				_form.FileName = _upload.Name;
				_getNameFromPath();
			}
		}
	}

	private void _onProgress(FluentInputFileEventArgs e)
	{
		progressPercent = e.ProgressPercent;
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
				var submit = new CreateFileAssetModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					FileName = _form.FileName,
					LocalPath = _form.LocalPath,
					CreatedBy = TfAppState.Value.CurrentUser.Id,
					DataProviderId = Content.DataProviderId,
					RowIds = Content.RowIds,
				};
				result = AssetsService.CreateFileAsset(submit);
			}
			else
			{
				throw new NotImplementedException();
			}

			ProcessFormSubmitResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(_isCreate ? LOC("File is added") : LOC("File is updated"));
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

	private void _getNameFromPath()
	{
		if (_upload is null) return;
		_form.Label = _upload.Name;

	}
}

public class AssetsFolderPanelFileModalForm
{
	[Required]
	public string Label { get; set; }
	[Required]
	public string LocalPath { get; set; }
	[Required]
	public string FileName { get; set; }
}

public class AssetsFolderPanelFileModalContext
{
	public Guid Id { get; set; }
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string FileName { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
}
