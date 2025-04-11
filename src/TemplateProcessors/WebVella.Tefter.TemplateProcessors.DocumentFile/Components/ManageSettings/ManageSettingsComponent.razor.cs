﻿using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Components.ManageSettings.ManageSettingsComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class ManageSettingsComponent : TfFormBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorManageSettingsScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("51157e04-9849-48ec-9bf3-de31308c4b0c");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Document Template Manage Seettings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	[Parameter] public TfTemplateProcessorManageSettingsScreenRegionContext RegionContext { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	private DocumentFileTemplateSettings _form = new();
	private string _downloadUrl
	{
		get
		{
			if (_form.TemplateFileBlobId is not null && _form.TemplateFileBlobId != Guid.Empty)
			{
				return String.Format(TfConstants.BlobDownloadUrl, _form.TemplateFileBlobId, _form.FileName);
			}
			return string.Empty;
		}
	}
	private bool _fileLoading = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<DocumentFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
		RegionContext.Validate = _validate;
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<DocumentFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.FileName))
			errors.Add(new ValidationError(nameof(DocumentFileTemplateSettings.FileName), LOC("required")));

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}

		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}
	private async Task _fileChanged(Tuple<string, string> input)
	{
		_fileLoading = true;
		await InvokeAsync(StateHasChanged);

		if (input is not null)
		{
			var blobId = TfService.CreateBlob(input.Item1, true);
			_form.TemplateFileBlobId = blobId;
			_form.FileName = input.Item2;
		}
		else
		{
			_form.FileName = null;
			_form.TemplateFileBlobId = null;
		}
		await _valueChanged();
		_fileLoading = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _valueChanged()
	{
		RegionContext.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await RegionContext.SettingsJsonChanged.InvokeAsync(RegionContext.Template.SettingsJson);
	}

}