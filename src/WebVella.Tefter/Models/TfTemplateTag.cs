namespace WebVella.Tefter.Models;

internal class TfTemplateTagResult
{
	//What string needs to be replaced in the place of the tag data
	public List<TfTemplateTag> Tags { get; set; }
	public string ValueString { get; set; }
	public object Value { get; set; }
}

internal class TfTemplateTag
{
	//What string needs to be replaced in the place of the tag data
	public string FullString { get; set; }
	public string Name { get; set; }
	public TfTemplateTagType Type { get; set; } = TfTemplateTagType.Data;
	//as in the sheet name there cannot be used [] and in all cases that there is a list of data matched
	//in the methods the first one is always 0 by default
	public List<int> IndexList { get; set; } = new();
	public List<TfTemplateTagParamGroup> ParamGroups { get; set; } = new();
}
internal enum TfTemplateTagType{ 
	Data = 0,
	Function = 1,
	ExcelFunction = 2
}

internal class TfTemplateTagParamGroup
{
	//What string needs to be replaced in the place of the tag data
	public string FullString { get; set; }
	public List<TfTemplateTagParameter> Parameters { get; set; } = new();
}

internal class TfTemplateTagParameter
{
	//What string needs to be replaced in the place of the tag data
	public string Name { get; set; } = null;
	public string ValueString { get; set; } = null;
}