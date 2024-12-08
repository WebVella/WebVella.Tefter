namespace WebVella.Tefter.Models;

internal abstract class TfTemplateContext
{
	public Guid Id { get; set; }
	public List<TfTemplateTag> Tags { get; set; }
	public List<Guid> Dependencies { get; set; }
	public List<Guid> Dependants { get; set; }
}

internal class TfExcelTemplateContext : TfTemplateContext
{
	public IXLWorksheet TemplateWorksheet { get; set; }
	public IXLWorksheet ResultWorksheet { get; set; }
	public IXLCells TemplateRange { get; set; }
	public IXLCells ResultRange { get; set; }
}