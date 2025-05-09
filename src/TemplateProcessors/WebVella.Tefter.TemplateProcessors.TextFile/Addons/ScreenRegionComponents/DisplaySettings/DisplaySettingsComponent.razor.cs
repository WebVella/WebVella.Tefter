namespace WebVella.Tefter.TemplateProcessors.TextFile.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Addons.ScreenRegionComponents.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class DisplaySettingsComponent : TfBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>
{
	public const string ID = "a23a91fe-cf4b-4f83-9f43-58484978cdab";
	public const string NAME = "Text File Template View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(TextFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorDisplaySettingsScreenRegionContext RegionContext { get; init; }
	private TextFileTemplateSettings _form = new();
	private string _downloadUrl
	{
		get
		{
			if (_form.TemplateFileBlobId is not null && _form.TemplateFileBlobId != Guid.Empty)
			{
				return String.Format(TfConstants.BlobDownloadUrl, _form.TemplateFileBlobId, _form.FileName);
			}
			return null;
		}
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextFileTemplateSettings>(RegionContext.Template.SettingsJson);
		}
	}

}