namespace WebVella.Tefter.Models;

internal abstract class TfTemplateContext
{
	public Guid Id { get; set; }
	public List<TfTemplateTag> Tags { get; set; }
	public List<Guid> Dependencies { get; set; } = new();
	public List<Guid> Dependants { get; set; } = new();
}

internal class TfExcelTemplateContext : TfTemplateContext
{
	public IXLWorksheet TemplateWorksheet { get; set; }
	public IXLWorksheet ResultWorksheet { get; set; }
	public IXLRange TemplateRange { get; set; }
	public IXLRange ResultRange { get; set; }
	public List<IXLRange> ResultRangeSlots { get; set; }
}