namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal TfDataTable DataTable { get; set; }
	internal List<TfDataColumn> SystemColumns { get; set; } = new();
	internal List<TfDataColumn> SharedKeyColumns { get; set; } = new();
	internal List<TfDataColumn> Columns { get; set; } = new();
	internal bool ShowSystemColumns { get; set; } = false;
	internal bool ShowSharedKeyColumns { get; set; } = false;
	internal bool ShowCustomColumns { get; set; } = true;
	internal int VisibleColumnsCount { get; set; } = 0;
	internal bool IsListBusy { get; set; } = true;

	internal string Search { get; set; } = null;
	internal int Page { get; set; } = 1;
	internal int PageSize { get; set; } = TfConstants.PageSize;

	internal Task InitForData()
	{
		IsBusy = true;
		return Task.CompletedTask;
	}

	internal Task LoadDataProviderDataTable(Guid providerId)
	{
		var result = GetProviderRows(providerId, Search, Page, PageSize);
		if (result.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviderRows failed").CausedBy(result.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.CompletedTask;
		}
		DataTable = result.Value;
		SystemColumns.Clear();
		SharedKeyColumns.Clear();
		Columns.Clear();
		VisibleColumnsCount = 0;
		Page = result.Value.QueryInfo.Page ?? 1;
		foreach (TfDataColumn column in DataTable.Columns)
		{
			if (column.IsSystem)
			{
				if (column.IsShared)
				{
					SharedKeyColumns.Add(column);
					VisibleColumnsCount++;
				}
				else
				{
					SystemColumns.Add(column);
					VisibleColumnsCount++;
				}
			}
			else
			{
				Columns.Add(column);
				VisibleColumnsCount++;
			}
		}

		return Task.CompletedTask;
	}

	internal Task DataProviderDataTableGoPreviousPage(Guid providerId){ 
		if(Page == 1) return Task.CompletedTask;
		Page--;
		LoadDataProviderDataTable(providerId);
		return Task.CompletedTask;
	}
	internal Task DataProviderDataTableGoNextPage(Guid providerId){ 
		Page++;
		LoadDataProviderDataTable(providerId);
		return Task.CompletedTask;
	}
	internal Task DataProviderDataTableGoFirstPage(Guid providerId){ 
		Page = 1;
		LoadDataProviderDataTable(providerId);
		return Task.CompletedTask;
	}
	internal Task DataProviderDataTableGoLastPage(Guid providerId){ 
		Page = -1;
		LoadDataProviderDataTable(providerId);
		return Task.CompletedTask;
	}

	internal Task DataProviderDataTableGoOnPage(Guid providerId){ 
		LoadDataProviderDataTable(providerId);
		return Task.CompletedTask;
	}
}
