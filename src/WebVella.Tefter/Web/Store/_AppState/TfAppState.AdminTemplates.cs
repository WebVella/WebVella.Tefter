namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<ITfTemplateProcessor> AdminTemplateProcessors { get; init; } = new();
}
