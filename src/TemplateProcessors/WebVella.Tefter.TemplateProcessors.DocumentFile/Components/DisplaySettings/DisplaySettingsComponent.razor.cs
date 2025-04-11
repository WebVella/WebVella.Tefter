namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Components.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class DisplaySettingsComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("e403ba40-0d75-4ee5-b978-d0490cd8c374");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Document Template View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	[Parameter] public TfTemplateProcessorDisplaySettingsScreenRegionContext RegionContext { get; init; }
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
			return String.Empty;
		}
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<DocumentFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<DocumentFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
		}
	}

}