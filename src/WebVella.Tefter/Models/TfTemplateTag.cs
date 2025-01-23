namespace WebVella.Tefter.Models;

public class TfTemplateTagResultList
{
	//What string needs to be replaced in the place of the tag data
	public List<TfTemplateTag> Tags { get; set; } = new();
	public bool AllDataTags
	{
		get
		{
			return Tags.All(x => x.Type == TfTemplateTagType.Data);
		}
	}
	public bool IsMultiValueTemplate
	{
		get
		{
			return Tags.Any(x => x.IndexList.Count == 0 && x.Flow != TfTemplateTagDataFlow.Horizontal);
		}
	}
	public List<object> Values { get; set; } = new();
}

public class TfTemplateTagResult
{
	//What string needs to be replaced in the place of the tag data
	public List<TfTemplateTag> Tags { get; set; }
	public string ValueString { get; set; }
	public object Value { get; set; }
}

public class TfTemplateTag
{
	//What string needs to be replaced in the place of the tag data
	public string FullString { get; set; }
	public string Name { get; set; }
	public TfTemplateTagType Type { get; set; } = TfTemplateTagType.Data;
	//as in the sheet name there cannot be used [] and in all cases that there is a list of data matched
	//in the methods the first one is always 0 by default
	public List<int> IndexList { get; set; } = new();
	public List<TfTemplateTagParamGroup> ParamGroups { get; set; } = new();

	public TfTemplateTagDataFlow Flow
	{
		get
		{
			foreach (var paramGroup in ParamGroups)
			{
				foreach (var param in paramGroup.Parameters)
				{
					if (param.Type == typeof(TfTemplateTagDataFlowParameter)
						&& ((TfTemplateTagDataFlowParameter)param).Value == TfTemplateTagDataFlow.Horizontal)
					{
						return TfTemplateTagDataFlow.Horizontal;
					}
				}
			}
			return TfTemplateTagDataFlow.Vertical;
		}
	}

	public string FlowSeparator
	{
		get
		{
			foreach (var paramGroup in ParamGroups)
			{
				foreach (var param in paramGroup.Parameters)
				{
					if (param.Type == typeof(TfTemplateTagDataSeparatorParameter))
					{
						return param.ValueString;
					}
				}
			}
			return null;
		}
	}

}
public enum TfTemplateTagType
{
	Data = 0,
	Function = 1,
	ExcelFunction = 2
}

public class TfTemplateTagParamGroup
{
	//What string needs to be replaced in the place of the tag data
	public string FullString { get; set; }
	public List<ITfTemplateTagParameterBase> Parameters { get; set; } = new();
}

public interface ITfTemplateTagParameterBase
{
	Type Type { get; }
	string Name { get; set; }
	string ValueString { get; set; }
	TfTemplateTagType TagType { get; }
}

public interface ITfTemplateTagParameterExcel
{
	HashSet<Guid> GetDependencies(TfExcelTemplateProcessResult result,
		TfExcelTemplateContext context,
		TfTemplateTag tag,
		TfTemplateTagParamGroup parameterGroup,
		ITfTemplateTagParameterExcel paramter);
}

#region << Template Tag Definitions >>
internal class TfTemplateTagUnknownParameter : ITfTemplateTagParameterBase
{
	public Type Type { get => this.GetType(); }
	public string Name { get; set; } = "uknown";
	public string ValueString { get; set; } = null;
	public TfTemplateTagType TagType { get => TfTemplateTagType.Data; }
	public string Value { get; set; } = null;
	public TfTemplateTagUnknownParameter() { }
	public TfTemplateTagUnknownParameter(string name, string valueString)
	{
		Name = name;
		ValueString = valueString;
		Value = valueString;
	}
}
internal class TfTemplateTagDataFlowParameter : ITfTemplateTagParameterBase, ITfTemplateTagParameterExcel
{
	public Type Type { get => this.GetType(); }
	public string Name { get; set; } = "f";
	public string ValueString { get; set; }
	public TfTemplateTagType TagType { get => TfTemplateTagType.Data; }
	public TfTemplateTagDataFlow Value { get; set; }

	public TfTemplateTagDataFlowParameter() { }
	public TfTemplateTagDataFlowParameter(string valueString)
	{
		ValueString = valueString;
		Value = StringToValue(valueString);
	}

	public string ValueToString(TfTemplateTagDataFlow? value)
		=> value is null ? TfTemplateTagDataFlow.Vertical.ToDescriptionString() : value.Value.ToDescriptionString();
	public TfTemplateTagDataFlow StringToValue(string valueString)
	{
		foreach (var item in Enum.GetValues<TfTemplateTagDataFlow>())
		{
			if (item.ToDescriptionString().ToLowerInvariant() == valueString.ToLowerInvariant())
				return item;
		}
		return TfTemplateTagDataFlow.Vertical;
	}

	public HashSet<Guid> GetDependencies(TfExcelTemplateProcessResult result, TfExcelTemplateContext context, TfTemplateTag tag, TfTemplateTagParamGroup parameterGroup, ITfTemplateTagParameterExcel paramter)
	 => new HashSet<Guid>();
}
public enum TfTemplateTagDataFlow
{
	[Description("V")]
	Vertical = 0,
	[Description("H")]
	Horizontal = 1
}

internal class TfTemplateTagDataSeparatorParameter : ITfTemplateTagParameterBase, ITfTemplateTagParameterExcel
{
	public Type Type { get => this.GetType(); }
	public string Name { get; set; } = "s";
	public string ValueString { get; set; }
	public TfTemplateTagType TagType { get => TfTemplateTagType.Data; }
	public string Value { get; set; }

	public TfTemplateTagDataSeparatorParameter() { }
	public TfTemplateTagDataSeparatorParameter(string valueString)
	{
		ValueString = valueString;
		Value = valueString;
	}

	public HashSet<Guid> GetDependencies(TfExcelTemplateProcessResult result, TfExcelTemplateContext context, TfTemplateTag tag, TfTemplateTagParamGroup parameterGroup, ITfTemplateTagParameterExcel paramter)
 => new HashSet<Guid>();
}

#endregion