namespace WebVella.Tefter.Models;

internal abstract class TfTemplateProcessResult
{
	public HashSet<Guid> ProcessedContexts { get; set; }

	//To find how many attempts were made for a context to be processed
	public List<Guid> ContextProcessLog { get; set; }
}

internal class TfExcelTemplateProcessResult : TfTemplateProcessResult
{
	public XLWorkbook TemplateWorkbook { get; set; } = null;
	public XLWorkbook ResultWorkbook { get; set; } = new();
	public List<TfExcelTemplateContext> Contexts { get; set; }
}

