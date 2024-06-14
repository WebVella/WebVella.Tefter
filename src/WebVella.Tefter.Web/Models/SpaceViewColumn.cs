﻿namespace WebVella.Tefter.Web.Models;

public record SpaceViewColumn
{
	public Guid Id { get; init; }
	public Guid SpaceViewId { get; init; }
	public Guid? AppId { get; init; } // null for system
	public string AppColumnName { get; init; } // for query and as dev name
	public string Title { get; init; }
	public int Position { get; init; }
	public DateTime CreatedOn { get; init; }
	public User CreatedBy { get; init; }
	public DateTime UpdatedOn { get; init; }
	public User UpdatedBy { get; init; }
	public SpaceViewColumnType DataType { get; init; }
	public object Settings { get; init; } //Based on type -> dropdown can have the values here
	public bool IsVisible { get; init; }
	public string Width { get; init; }
	public List<Permission> Permissions { get; init; }
	public string CellComponent { get; init; }  //Temporary for demo

	[JsonIgnore]
	public Type CellType
	{
		get
		{
			if (String.IsNullOrWhiteSpace(CellComponent)) return null;
			return Type.GetType(CellComponent);
		}
	}

	public object CellProperties { get; set; }

	public IDictionary<string, object> GetComponentParameters(DataRow context)
	{
		var result = new Dictionary<string, object>();
		result["Data"] = context.Fields.ContainsKey(AppColumnName) ? context.Fields[AppColumnName] : null;
		result["Meta"] = this;
		result["ValueChanged"] = context.OnCellDataChange;
		return result;
	}
}
