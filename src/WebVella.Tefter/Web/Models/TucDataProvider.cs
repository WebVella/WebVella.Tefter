namespace WebVella.Tefter.Web.Models;

public record TucDataProvider
{
	public Guid Id { get; init; }
	public string Name { get; init; }
	public int Index { get; init; }
	public string SettingsJson { get; init; }
	public List<TucDataProviderSystemColumn> SystemColumns { get; init; }
	public List<TucDataProviderColumn> Columns { get; init; }
	public List<TucSharedColumn> SharedColumns { get; init; } = new();
	public List<TucColumn> ColumnsTotal
	{
		get
		{
			var result = new List<TucColumn>();
			if (Columns is not null)
			{
				foreach (var item in Columns) result.Add(new TucColumn(item));
			}
			if (SharedColumns is not null)
			{
				foreach (var item in SharedColumns) result.Add(new TucColumn(item));
			}
			if (SystemColumns is not null)
			{
				foreach (var item in SystemColumns) result.Add(new TucColumn(item));
			}
			return result.OrderBy(x => x.DbName).ToList();
		}
	}
	public List<TucColumn> ColumnsPublic
	{
		get
		{
			var result = new List<TucColumn>();
			if (Columns is not null)
			{
				foreach (var item in Columns) result.Add(new TucColumn(item));
			}
			if (SharedColumns is not null)
			{
				foreach (var item in SharedColumns) result.Add(new TucColumn(item));
			}
			return result.OrderBy(x => x.DbName).ToList();
		}
	}
	public string ColumnPrefix { get => $"{Code}_"; }
	public string Code { get => $"dp{Index}"; }
	public List<TucDataProviderJoinKey> JoinKeys { get; init; }
	public TucDataProviderTypeInfo ProviderType { get; init; }
	public ReadOnlyCollection<string> SynchPrimaryKeyColumns { get; init; }
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;
	public TucDataProvider() { }
	public TucDataProvider(TfDataProvider model)
	{
		Id = model.Id;
		Name = model.Name;
		Index = model.Index;
		SettingsJson = model.SettingsJson;
		SystemColumns = model.SystemColumns is null ? null : model.SystemColumns.Select(x => new TucDataProviderSystemColumn(x)).ToList();
		Columns = model.Columns is null ? null : model.Columns.Select(x => new TucDataProviderColumn(x)).ToList();
		SharedColumns = model.SharedColumns is null ? null : model.SharedColumns.Select(x => new TucSharedColumn(x)).ToList();
		JoinKeys = model.JoinKeys is null ? null : model.JoinKeys.Select(x => new TucDataProviderJoinKey(x)).ToList();
		ProviderType = new TucDataProviderTypeInfo(model.ProviderType);
		SynchPrimaryKeyColumns = model.SynchPrimaryKeyColumns;
		SynchScheduleMinutes = model.SynchScheduleMinutes;
		SynchScheduleEnabled = model.SynchScheduleEnabled;
	}

}
