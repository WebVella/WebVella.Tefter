namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class DisplaySettingsComponent : TfBaseComponent, 
	ITfDynamicComponent<TfTemplateProcessorDisplaySettingsComponentContext>,
	ITfComponentScope<ExcelFileTemplateProcessor>
{
	public Guid Id { get; init; } = new Guid("e403ba40-0d75-4ee5-b978-d0490cd8c374");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Excel Template View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfTemplateProcessorDisplaySettingsComponentContext Context { get; init; }
	private ExcelFileTemplateSettings _form = new();
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
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<ExcelFileTemplateSettings>(Context.Template.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<ExcelFileTemplateSettings>(Context.Template.SettingsJson);
		}
	}

}