namespace WebVella.Tefter.Models;

public abstract class TfTemplateContext
{
	public Guid Id { get; set; }
	public TfTemplateTagResultList TagProcessResult { get; set; }
	public List<TfTemplateTag> Tags { get; set; }
	public HashSet<Guid> Dependencies { get; set; } = new();
	public HashSet<Guid> Dependants { get; set; } = new();
	//for optimization purpose - when all tags are a data type their values are set during placement
	public bool IsDataSet { get; set; } = false;
}

public class TfExcelTemplateContext : TfTemplateContext
{
	public IXLWorksheet TemplateWorksheet { get; set; }
	public IXLWorksheet ResultWorksheet { get; set; }
	public IXLRange TemplateRange { get; set; }
	public IXLRange ResultRange { get; set; }
	public List<TfExcelTemplateContextRangeAddress> ResultRangeSlots { get; set; }
}

public class TfExcelTemplateContextRangeAddress{
	public int FirstRow { get; set; }
	public int FirstColumn { get; set; }
	public int LastRow { get; set; }
	public int LastColumn { get; set; }

	public TfExcelTemplateContextRangeAddress(){}
	public TfExcelTemplateContextRangeAddress(int firstRow, int firstColumn, int lastRow, int lastColumn){
		FirstRow = firstRow; FirstColumn = firstColumn;
		LastRow = lastRow; LastColumn = lastColumn;
	}
}