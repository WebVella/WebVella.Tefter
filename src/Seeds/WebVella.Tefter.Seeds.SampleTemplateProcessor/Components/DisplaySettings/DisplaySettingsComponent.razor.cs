namespace WebVella.Tefter.Seeds.SampleTemplateProcessor.Components;

public partial class DisplaySettingsComponent : TfFormBaseComponent, 
	ITfRegionComponent<TfTemplateProcessorDisplaySettingsComponentContext>
{
	public Guid Id { get; init; } = new Guid("d05ea639-b6d2-4f8b-8cc7-307961cf0502");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Text content View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(SampleTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorDisplaySettingsComponentContext Context { get; init; }

	private SampleTemplateSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleTemplateSettings>(Context.Template.SettingsJson);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleTemplateSettings>(Context.Template.SettingsJson);
		}
	}


}