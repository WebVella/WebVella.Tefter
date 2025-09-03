using System.ComponentModel.DataAnnotations;
namespace WebVella.Tefter.Assets.Components;
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
		_title = _isCreate ? LOC("Attach link") : LOC("Manage link");
		_btnText = _isCreate ? (Content.IsAddOnly ? LOC("Add") : LOC("Attach")) : LOC("Save");
		_iconBtn = _isCreate ? TfConstants.GetIcon("Add") : TfConstants.GetIcon("Save");
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
			if ((_isCreate || Content.IsAddOnly) && Content.RowIds is not null && Content.DataProviderId is not null)
			{
				var submit = new CreateLinkAssetWithRowIdModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					Url = _form.Url,
					IconUrl = _form.IconUrl,
					CreatedBy = Content.UserId,
					RowIds = Content.RowIds,
					DataProviderId = Content.DataProviderId.Value
				};
				if(Content.IsAddOnly){ 
					await Dialog.CloseAsync(submit);
					return;
				}
				result = AssetsService.CreateLinkAsset(submit);
			}
			else if ((_isCreate || Content.IsAddOnly) && Content.DataIdentities is not null)
			{
				var submit = new CreateLinkAssetWithDataIdentityModel
				{
					FolderId = Content.FolderId,
					Label = _form.Label,
					Url = _form.Url,
					IconUrl = _form.IconUrl,
					CreatedBy = Content.UserId,
					DataIdentityValues = Content.DataIdentities
				};
				if(Content.IsAddOnly){ 
					await Dialog.CloseAsync(submit);
					return;
				}
				result = AssetsService.CreateLinkAsset(submit);
			}
			else
			{
				result = AssetsService.UpdateLinkAsset(Content.Id, _form.Label, _form.Url, _form.IconUrl, Content.UserId);
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
	public Guid UserId { get; set; }
	public List<string>? DataIdentities { get; set; } = null;
	public List<Guid> RowIds { get; set; } = null;
	public Guid? DataProviderId { get; set; } = null;
	public bool IsAddOnly { get; set; } = false;
}
