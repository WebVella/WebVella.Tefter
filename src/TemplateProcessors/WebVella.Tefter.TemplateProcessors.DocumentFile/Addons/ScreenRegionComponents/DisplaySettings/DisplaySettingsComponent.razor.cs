namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.DocumentFile.Addons.ScreenRegionComponents.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.DocumentFile")]
public partial class DisplaySettingsComponent : TfBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>
{
	public const string ID = "e403ba40-0d75-4ee5-b978-d0490cd8c374";
	public const string NAME = "Document Template View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;

	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorDisplaySettingsScreenRegionContext RegionContext { get; init; }

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