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
	FluentInputFile fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();
	string fakeLocalPath = "uploaded";
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (Content.Id == Guid.Empty) _isCreate = true;
		_title = _isCreate ? LOC("Upload file") : LOC("Update file");
		_btnText = _isCreate ? LOC("Create") : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.AddIcon : TfConstants.SaveIcon;
		_form.Label = Content.Label;
		_form.FileName = Content.FileName;
		_form.LocalPath = _isCreate ? null : fakeLocalPath;
		base.InitForm(_form);
	}

	private void _onCompleted(IEnumerable<FluentInputFileEventArgs> files)
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
				_getNameFromPath(false);
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

			if (_isCreate)
			{
				if (String.IsNullOrWhiteSpace(_form.LocalPath))
					MessageStore.Add(EditContext.Field(nameof(_form.LocalPath)), LOC("File is required"));
				if (String.IsNullOrWhiteSpace(_form.FileName))
					MessageStore.Add(EditContext.Field(nameof(_form.LocalPath)), LOC("File is required"));
			}

			if (!EditContext.Validate()) return;

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);

			var result = new Asset();
			if (_isCreate && Content.RowIds is not null && Content.RowIds.Count > 0)
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
			else if (_isCreate && Content.SKValueIds is not null && Content.SKValueIds.Count > 0)
			{
				var submit = new CreateFileAssetWithSharedKeyModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					FileName = _form.FileName,
					LocalPath = _form.LocalPath,
					CreatedBy = TfAppState.Value.CurrentUser.Id,
					SKValueIds = Content.SKValueIds,
				};
				result = AssetsService.CreateFileAsset(submit);
			}
			else
			{
				result = AssetsService.UpdateFileAsset(
					id: Content.Id,
					label: _form.Label,
					localPath: _form.LocalPath == fakeLocalPath ? null : _form.LocalPath,
					userId: TfAppState.Value.CurrentUser.Id);
			}

			ToastService.ShowSuccess(_isCreate ? LOC("File is added") : LOC("File is updated"));
			await Dialog.CloseAsync(result);
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

	private void _getNameFromPath(bool force = false)
	{
		if (!force && !String.IsNullOrWhiteSpace(_form.Label)) return;
		if (String.IsNullOrWhiteSpace(_form.FileName)) return;
		_form.Label = _form.FileName;

	}

	private void _clearFile()
	{
		_form.FileName = null;
		_form.LocalPath = null;
	}
}

public record AssetsFolderPanelFileModalForm
{
	[Required]
	public string Label { get; set; }
	public string LocalPath { get; set; }
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
	public List<Guid> SKValueIds { get; set; }
}
