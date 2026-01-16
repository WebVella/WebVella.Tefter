using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public enum TfColumnNamePreprocessType
{ 
	[Description("no preprocessing")]
	None = 0,
	[Description("remove data provider prefix (eg. dp_1)")]
	RemoveProviderPrefix = 1
}

public record TfTemplate
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string FluentIconName { get; set; } = "Document";
	public List<string> RequiredColumnsList { get; set; } = new();
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public TfTemplateResultType ResultType { get; set; }
	public string? SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; } = null!;
	public DateTime CreatedOn { get; set; }
	public TfUser CreatedBy { get; set; } = null!;
	public DateTime ModifiedOn { get; set; }
	public TfUser ModifiedBy { get; set; } = null!;
	public TfColumnNamePreprocessType ColumnNamePreprocess { get; set; } = TfColumnNamePreprocessType.None;

	public bool IsDatasetApplicable(TfDataset dataset)
	{
		if(RequiredColumnsList.Count == 0) return true;
		
		var datasetColumns = new List<string>();
		foreach (string column in dataset.Columns)
		{
			if(ColumnNamePreprocess == TfColumnNamePreprocessType.RemoveProviderPrefix)
				datasetColumns.Add(column.GetColumnNameWithoutPrefix());
			else
				datasetColumns.Add(column);
		}
		foreach (var datasetIdentity in dataset.Identities)
		{
			foreach (var columnSuf in datasetIdentity.Columns)
			{
				var column = $"{datasetIdentity.DataIdentity}.{columnSuf}";
				if(ColumnNamePreprocess == TfColumnNamePreprocessType.RemoveProviderPrefix)
					datasetColumns.Add(column.GetColumnNameWithoutPrefix());
				else
					datasetColumns.Add(column);				
			}
		}		
		
		foreach (var reqColumn in RequiredColumnsList)
		{
			if (!datasetColumns.Contains(reqColumn.ToLowerInvariant()))
				return false;
		}
		return true;
	}
}

public record TfManageTemplateModel
{
	public Guid Id { get; set; }
	[Required] public string Name { get; set; } = String.Empty;
	public string? Description { get; set; }
	[Required] public string FluentIconName { get; set; } = "Document";
	public bool IsEnabled { get; set; } = true;
	public bool IsSelectable { get; set; } = true;
	public string? SettingsJson { get; set; }
	public Type ContentProcessorType { get; set; } = null!;
	public TfColumnNamePreprocessType ColumnNamePreprocess { get; set; } = TfColumnNamePreprocessType.None;
	public Guid? UserId { get; set; }
	public List<string> RequiredColumnsList { get; set; } = new();
}
