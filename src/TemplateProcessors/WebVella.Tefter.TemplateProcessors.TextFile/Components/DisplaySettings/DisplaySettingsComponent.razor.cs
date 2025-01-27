namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class DisplaySettingsComponent : TfBaseComponent, 
	ITfDynamicComponent<TfTemplateProcessorDisplaySettingsComponentContext>,
	ITfComponentScope<TextFileTemplateProcessor>
{
	public Guid Id { get; init; } = new Guid("a23a91fe-cf4b-4f83-9f43-58484978cdab");
	public int PositionRank { get; init; } = 0;
	public string Name { get; init; } = "Text File Template View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorDisplaySettingsComponentContext Context { get; init; }
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
	private bool _fileLoading = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextFileTemplateSettings>(Context.Template.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextFileTemplateSettings>(Context.Template.SettingsJson);
		}
	}

}