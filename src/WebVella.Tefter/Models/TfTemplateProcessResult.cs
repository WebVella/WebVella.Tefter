namespace WebVella.Tefter.Models;

public abstract class TfTemplateProcessResult
{
	public HashSet<Guid> ProcessedContexts { get; set; } = new();

	//To find how many attempts were made for a context to be processed
	public Dictionary<Guid,int> ContextProcessLog { get; set; } = new();	
}

public class TfExcelTemplateProcessResult : TfTemplateProcessResult
{
	public XLWorkbook TemplateWorkbook { get; set; } = null;
	public XLWorkbook ResultWorkbook { get; set; } = new();
	public List<TfExcelTemplateContext> Contexts { get; set; } = new();
}

public class TfTextTemplateProcessResult : TfTemplateProcessResult
{
	public string TemplateText { get; set; } = null;
	public string ResultText { get; set; } = null;
	public List<TfTemplateContext> Contexts { get; set; } = new();
}
