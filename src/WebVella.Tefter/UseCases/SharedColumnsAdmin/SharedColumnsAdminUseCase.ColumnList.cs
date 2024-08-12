namespace WebVella.Tefter.UseCases.SharedColumnsAdmin;
public partial class SharedColumnsAdminUseCase
{
	internal List<TucSharedColumn> SharedColumns { get; set; } = new();
	internal string Search { get; set; } = null;

	internal Task InitForColumnList()
	{
		return Task.CompletedTask;
	}

	internal Task LoadSharedColumnList()
	{
		var searchLower = Search;
		if (!String.IsNullOrWhiteSpace(Search)) searchLower = Search.Trim().ToLowerInvariant();
		SharedColumns.Clear();
		foreach (var item in GetSharedColumns())
		{
			if (String.IsNullOrWhiteSpace(Search))
			{
				SharedColumns.Add(item);
				continue;
			}
			if (String.IsNullOrWhiteSpace(item.SharedKeyDbName)) continue;

			if (item.SharedKeyDbName.ToLowerInvariant().Contains(searchLower))
			{
				SharedColumns.Add(item);
				continue;
			}

		}
		SharedColumns = SharedColumns.OrderBy(x=> x.DbName).ToList();

		return Task.CompletedTask;
	}
}
