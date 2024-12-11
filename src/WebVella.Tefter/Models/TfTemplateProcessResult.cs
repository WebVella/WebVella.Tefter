namespace WebVella.Tefter.Models;

internal abstract class TfTemplateProcessResult
{
	public HashSet<Guid> ProcessedContexts { get; set; } = new();

	//To find how many attempts were made for a context to be processed
	public Dictionary<Guid,int> ContextProcessLog { get; set; } = new();	
}

internal class TfExcelTemplateProcessResult : TfTemplateProcessResult
{
	public XLWorkbook TemplateWorkbook { get; set; } = null;
	public XLWorkbook ResultWorkbook { get; set; } = new();
	public List<TfExcelTemplateContext> Contexts { get; set; } = new();
}

internal class TfTextTemplateProcessResult : TfTemplateProcessResult
{
	public string TemplateText { get; set; } = null;
	public string ResultText { get; set; } = null;
	public List<TfTemplateContext> Contexts { get; set; } = new();
}
