using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.Tefter.Utility;
internal static class TfTemplateUtility
{
	public static List<TfTemplateTagResult> ProcessTemplateTag(string template, TfDataTable dataSource)
	{
		var result = new List<TfTemplateTagResult>();
		for (int i = 0; i < dataSource.Rows.Count; i++)
		{
			result.Add(GenerateTemplateTagResult(template, dataSource, i));
		}
		return result;
	}

	public static TfTemplateTagResult GenerateTemplateTagResult(string template, TfDataTable dataSource, int? rowIndex)
	{
		var result = new TfTemplateTagResult();
		result.Value = template;
		if(String.IsNullOrWhiteSpace(template)) return result;
		result.Tags = GetTagsFromTemplate(template);
		foreach (var tag in result.Tags)
		{
			result.Value = ProcessTagInTemplate(result.Value, tag, dataSource, rowIndex);
		}

		return result;
	}

	public static string ProcessTagInTemplate(string template, TfTemplateTag tag, TfDataTable dataSource, int? contextRowIndex)
	{
		var result = template;
		//Rules:
		//If tag not found or cannot be used return tag full string (no substitution)
		//if tag has index it is check for applicability if not applicable return tag full string for this template
		//if tag has no index - apply the submitted index
		//if not index is requested get the first if present		
		if (tag.Type == TfTemplateTagType.Data)
		{
			if (String.IsNullOrWhiteSpace(tag.Name)) return result;
			int columnIndex = dataSource.Columns.IndexOf(x => x.Name.ToLowerInvariant() == tag.Name);
			if (columnIndex == -1) return result;
			if (dataSource.Rows.Count == 0) return result;
			int rowIndex = 0;
			if(tag.IndexList is not null && tag.IndexList.Count > 0){ 
				rowIndex = tag.IndexList[0];
			}
			else if (contextRowIndex is not null && dataSource.Rows.Count - 1 >= contextRowIndex)
			{
				rowIndex = contextRowIndex.Value;
			}
			result = result.Replace(tag.FullString, dataSource.Rows[rowIndex][columnIndex]?.ToString());
		}
		else if (tag.Type == TfTemplateTagType.Function)
		{
			throw new NotImplementedException();
		}
		else if (tag.Type == TfTemplateTagType.ExcelFunction)
		{
			throw new NotImplementedException();
		}
		return result;
	}

	public static List<TfTemplateTag> GetTagsFromTemplate(string text)
	{
		var result = new List<TfTemplateTag>();
		if (!text.Contains("{{") && !text.Contains("}}")) return result;

		foreach (Match match in Regex.Matches(text, @"(\{\{[^{}]*\}\})"))
		{
			var tag = ExtractTagFromDefinition(match.Value);
			if (tag is not null) result.Add(tag);
		}

		return result;
	}

	private static TfTemplateTag ExtractTagFromDefinition(string tagDefinition)
	{
		if (String.IsNullOrWhiteSpace(tagDefinition)
		|| !tagDefinition.StartsWith("{{")
		|| !tagDefinition.EndsWith("}}")) return null;
		TfTemplateTag result = new();
		//Remove leading and end {{}}
		var processedDefinition = tagDefinition.Remove(0, 2);
		processedDefinition = processedDefinition.Substring(0, processedDefinition.Length - 2);
		processedDefinition = processedDefinition?.Trim();
		if (String.IsNullOrWhiteSpace(processedDefinition)) return null;
		result.FullString = tagDefinition;
		//Excel function
		if (processedDefinition.StartsWith("=="))
		{
			result.Type = TfTemplateTagType.ExcelFunction;
			processedDefinition = processedDefinition.Remove(0, 2);
		}
		//Function
		else if (processedDefinition.StartsWith("="))
		{
			result.Type = TfTemplateTagType.Function;
			processedDefinition = processedDefinition.Remove(0, 1);
		}
		//Data
		else
		{
			result.Type = TfTemplateTagType.Data;

		}

		//PARAMETERS
		foreach (Match matchGroup in Regex.Matches(processedDefinition, @"(\([^()]*\))"))
		{
			//Replace the first occurance of the group string
			var regex = new Regex(Regex.Escape(matchGroup.Value));
			processedDefinition = regex.Replace(processedDefinition, "", 1);

			var group = new TfTemplateTagParamGroup
			{
				FullString = matchGroup.Value,
				Parameters = ExtractTagParametersFromGroup(matchGroup.Value)
			};
			result.ParamGroups.Add(group);
		}
		processedDefinition = processedDefinition?.Trim();
		//if (String.IsNullOrWhiteSpace(processedDefinition)) return null;

		//INDEX
		foreach (Match matchGroup in Regex.Matches(processedDefinition, @"(\[[^[]]*\]|\[\s*\])"))
		{
			//Replace the first occurance of the group string
			var regex = new Regex(Regex.Escape(matchGroup.Value));
			processedDefinition = regex.Replace(processedDefinition, "", 1);

			var tagIndex = ExtractTagIndexFromGroup(matchGroup.Value);
			if (tagIndex is null || tagIndex < 0)
			{
				//interpred [] or any [invalid] as 0
				result.IndexList.Add(0);
			}
			else
			{
				result.IndexList.Add(tagIndex.Value);
			}
		}

		processedDefinition = processedDefinition?.Trim();
		//if (String.IsNullOrWhiteSpace(processedDefinition)) return null;
		result.Name = processedDefinition.ToLowerInvariant();
		return result;

	}

	private static List<TfTemplateTagParameter> ExtractTagParametersFromGroup(string parameterGroup)
	{
		var result = new List<TfTemplateTagParameter>();
		if (String.IsNullOrWhiteSpace(parameterGroup)
		|| !parameterGroup.StartsWith("(")
		|| !parameterGroup.EndsWith(")")) return result;
		//Remove leading and end ()
		var processedParameterGroup = parameterGroup.Remove(0, 1);
		processedParameterGroup = processedParameterGroup.Substring(0, processedParameterGroup.Length - 1);
		processedParameterGroup = processedParameterGroup?.Trim();
		if (String.IsNullOrWhiteSpace(processedParameterGroup)) return result;

		foreach (var parameterString in processedParameterGroup.Split(",", StringSplitOptions.RemoveEmptyEntries))
		{
			var parameter = ExtractTagParameterFromDefinition(parameterString);
			if (parameter is not null) result.Add(parameter);
		}


		return result;
	}

	private static TfTemplateTagParameter ExtractTagParameterFromDefinition(string parameterDefinition)
	{
		if (String.IsNullOrWhiteSpace(parameterDefinition)) return null;
		TfTemplateTagParameter result = new();
		string paramName = null;
		string paramValue = null;
		var firstEqualSignIndex = parameterDefinition.IndexOf('=');
		//if not a named parameter
		if (firstEqualSignIndex == -1)
		{
			paramValue = parameterDefinition;
		}
		else
		{
			paramName = parameterDefinition.Substring(0, firstEqualSignIndex);
			paramValue = parameterDefinition.Substring(firstEqualSignIndex + 1); //Remove the =
		}

		paramName = paramName?.Trim()?.ToLowerInvariant(); //names are always lowered
		paramValue = paramValue?.Trim();

		//Check if it is a string value
		if (
		(paramValue.StartsWith("\"") && paramValue.EndsWith("\""))
		|| (paramValue.StartsWith("'") && paramValue.EndsWith("'"))
		)
		{
			paramValue = paramValue.Remove(0, 1);
			paramValue = paramValue.Substring(0, paramValue.Length - 1);
		}


		result.Name = paramName;
		result.Value = paramValue;

		return result;
	}

	private static int? ExtractTagIndexFromGroup(string indexGroup)
	{
		if (String.IsNullOrWhiteSpace(indexGroup)
		|| !indexGroup.StartsWith("[")
		|| !indexGroup.EndsWith("]")) return null;

		//Remove leading and end []
		var processedIndexGroup = indexGroup.Remove(0, 1);
		processedIndexGroup = processedIndexGroup.Substring(0, processedIndexGroup.Length - 1);
		processedIndexGroup = processedIndexGroup?.Trim();
		if (String.IsNullOrWhiteSpace(processedIndexGroup)) return null;

		if (int.TryParse(processedIndexGroup, out int outInt))
			return outInt;

		return null;
	}

}
