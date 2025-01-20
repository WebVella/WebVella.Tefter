using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.ResultPreview.ResultPreviewComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class ResultPreviewComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>
{
	[Inject] private ITfTemplateService TemplateService { get; set; }
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorResultPreviewComponentContext Context { get; set; }

	private EmailTemplatePreviewResult _preview = null;
	private bool _isLoading = true;
	private List<ValidationError> _previewValidationErrors = new();
	private EmailTemplateResultItem _form = new();
	private Dictionary<Guid, int> _itemPositionDict = new();
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null) throw new Exception("Context is not defined");

		Context.ValidatePreviewResult = _validatePreviewResult;
		base.InitForm(_form);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Context.Template is not null && Context.SpaceData is not null)
			{
				ITfTemplatePreviewResult result = TemplateService.GenerateTemplatePreviewResult(
					templateId: Context.Template.Id,
					spaceDataId: Context.SpaceData.Id,
					tfRecordIds: Context.SelectedRowIds
				);
				if (result is not EmailTemplatePreviewResult)
				{
					throw new Exception("Preview result is not of type ExcelFileTemplatePreviewResult");
				}
				_preview = (EmailTemplatePreviewResult)result;
				_itemPositionDict = new();
				if (_preview.Items.Count > 0)
				{
					_form = _preview.Items[0];
					base.InitForm(_form);
					var position = 1;
					foreach (var item in _preview.Items)
					{
						_itemPositionDict[item.Id] = position;
						position++;
					}
				}
				await Context.PreviewResultChanged.InvokeAsync(_preview);
			}

			_isLoading = false;
			StateHasChanged();
		}
	}

	private List<ValidationError> _validatePreviewResult()
	{
		MessageStore.Clear();
		_previewValidationErrors = new List<ValidationError>();
		foreach (var emailItem in _preview.Items)
		{
			var formValErrors = _validateItem(emailItem);
			foreach (var item in formValErrors)
			{
				MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
			}
			_previewValidationErrors.AddRange(formValErrors);
		}
		var isValid = EditContext.Validate();
		StateHasChanged();
		return _previewValidationErrors;
	}

	private void _nextItem()
	{
		if (_form is null || _itemPositionDict is null) return;
		var itemPosition = _itemPositionDict[_form.Id];
		var newPosition = itemPosition + 1;
		if (newPosition <= 0 || newPosition > _preview.Items.Count)
			newPosition = 1;

		MessageStore.Clear();
		var formValErrors = _validateItem(_form);
		foreach (var item in formValErrors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		if (!EditContext.Validate()) return;

		_form = _preview.Items[newPosition - 1];
		base.InitForm(_form);
	}
	private void _prevItem()
	{
		if (_form is null || _itemPositionDict is null) return;
		var itemPosition = _itemPositionDict[_form.Id];
		var newPosition = itemPosition - 1;
		if (newPosition <= 0)
			newPosition = _preview.Items.Count;

		MessageStore.Clear();
		var formValErrors = _validateItem(_form);
		foreach (var item in formValErrors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		if (!EditContext.Validate()) return;

		_form = _preview.Items[newPosition - 1];
		base.InitForm(_form);
	}

	private List<ValidationError> _validateItem(EmailTemplateResultItem item)
	{
		var valErrors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(item.Sender))
			valErrors.Add(new ValidationError(nameof(item.Sender), LOC("required")));
		else if (!item.Sender.IsEmail())
			valErrors.Add(new ValidationError(nameof(item.Sender), LOC("{0} is invalid email", item.Sender)));

		if (item.Recipients is null || item.Recipients.Count == 0)
			valErrors.Add(new ValidationError(nameof(item.Recipients), LOC("required")));
		else
		{
			foreach (var recipient in item.Recipients)
			{
				if (!recipient.IsEmail())
					valErrors.Add(new ValidationError(nameof(item.Recipients), LOC("{0} is invalid email", recipient)));
			}
		}
		if (String.IsNullOrWhiteSpace(item.Subject))
			valErrors.Add(new ValidationError(nameof(item.Subject), LOC("required")));
		if (String.IsNullOrWhiteSpace(item.HtmlContent))
			valErrors.Add(new ValidationError(nameof(item.HtmlContent), LOC("required")));
		return valErrors;
	}

	private void _emailListChanged(string value, string propName)
	{
		var emailList = new List<string>();
		foreach (var firstSplit in value.Split(";", StringSplitOptions.RemoveEmptyEntries))
		{
			foreach (var secondSplit in firstSplit.Split(",", StringSplitOptions.RemoveEmptyEntries))
			{
				emailList.Add(secondSplit.Trim());
			}
		}

		switch (propName)
		{
			case nameof(_form.Recipients):
				_form.Recipients = emailList;
				break;
			case nameof(_form.CcRecipients):
				_form.CcRecipients = emailList;
				break;
			case nameof(_form.BccRecipients):
				_form.BccRecipients = emailList;
				break;
			default:
				throw new Exception("unsupported propName");
		}
	}

	private void _selectedOptionChanged(EmailTemplateResultItem item)
	{
		MessageStore.Clear();
		var formValErrors = _validateItem(_form);
		foreach (var error in formValErrors)
		{
			MessageStore.Add(EditContext.Field(error.PropertyName), error.Message);
		}
		if (!EditContext.Validate()) return;

		_form = item;
		base.InitForm(_form);

	}

}