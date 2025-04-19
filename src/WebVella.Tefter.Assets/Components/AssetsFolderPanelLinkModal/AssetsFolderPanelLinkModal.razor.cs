using System.ComponentModel.DataAnnotations;
namespace WebVella.Tefter.Assets.Components;
[LocalizationResource("WebVella.Tefter.Assets.Components.AssetsFolderPanelLinkModal.AssetsFolderPanelLinkModal", "WebVella.Tefter.Assets")]
public partial class AssetsFolderPanelLinkModal : TfFormBaseComponent, IDialogContentComponent<AssetsFolderPanelLinkModalContext>
{
	[Inject] public IAssetsService AssetsService { get; set; }
	[Parameter] public AssetsFolderPanelLinkModalContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private bool _isCreate = false;
	private bool _isGetUrlLoading = false;
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
		_form.IconUrl = Content.IconUrl;
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

			//Get Favicon Url
			if (String.IsNullOrWhiteSpace(_form.IconUrl))
				_form.IconUrl = await new UrlUtility(ConfigurationService).GetFavIconForUrl(_form.Url);

			var result = new Asset();
			if (_isCreate && Content.RowIds is not null && Content.RowIds.Count > 0)
			{
				var submit = new CreateLinkAssetModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					Url = _form.Url,
					IconUrl = _form.IconUrl,
					CreatedBy = Content.CreatedBy,
					RowIds = Content.RowIds,
					DataProviderId = Content.DataProviderId
				};
				result = AssetsService.CreateLinkAsset(submit);
			}
			else if (_isCreate && Content.SKValueIds is not null && Content.SKValueIds.Count > 0)
			{
				var submit = new CreateLinkAssetWithJoinKeyModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					Url = _form.Url,
					IconUrl = _form.IconUrl,
					CreatedBy = Content.CreatedBy,
					SKValueIds = Content.SKValueIds
				};
				result = AssetsService.CreateLinkAsset(submit);
			}
			else
			{
				result = AssetsService.UpdateLinkAsset(Content.Id, _form.Label, _form.Url, _form.IconUrl, Content.CreatedBy);
			}

			ToastService.ShowSuccess(_isCreate ? LOC("Link is added") : LOC("Link is updated"));
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

	private async Task _getNameFromUrl(bool force = false)
	{
		if (!force && !String.IsNullOrWhiteSpace(_form.Label)) return;
		if (String.IsNullOrWhiteSpace(_form.Url)) return;


		_isGetUrlLoading = true;
		await InvokeAsync(StateHasChanged);
		var newLabel = await new UrlUtility(ConfigurationService).GetMetaTitleFromUrl(_form.Url);
		if (!String.IsNullOrWhiteSpace(newLabel)) _form.Label = newLabel;
		_form.IconUrl = await new UrlUtility(ConfigurationService).GetFavIconForUrl(_form.Url);

		_isGetUrlLoading = false;
		await InvokeAsync(StateHasChanged);
	}

	private void _urlChanged(string url)
	{
		if (!String.IsNullOrWhiteSpace(url) && !(url.StartsWith("http") || url.StartsWith("/")))
		{
			url = "https://" + url;
		}
		_form.Url = url;
	}
}

public class AssetsFolderPanelLinkModalForm
{
	[Required]
	public string Label { get; set; }
	[Required]
	public string Url { get; set; }
	public string IconUrl { get; set; }
}

public class AssetsFolderPanelLinkModalContext
{
	public Guid Id { get; set; }
	public Guid FolderId { get; set; }
	public string Label { get; set; }
	public string Url { get; set; }
	public string IconUrl { get; set; }
	public Guid CreatedBy { get; set; }
	public List<Guid> RowIds { get; set; }
	public Guid DataProviderId { get; set; }
	public List<Guid> SKValueIds { get; set; }
}
