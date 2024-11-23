namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderImportSchemaDialog.TfDataProviderImportSchemaDialog", "WebVella.Tefter")]
public partial class TfDataProviderImportSchemaDialog : TfBaseComponent, IDialogContentComponent<TucDataProvider>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] private IState<TfAppState> TfAppState { get; set; }
	[Parameter] public TucDataProvider Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _activeTab = "new";

	private List<TucDataProviderColumn> _newColumns = new List<TucDataProviderColumn>();
	private List<TucDataProviderColumn> _existingColumns = new List<TucDataProviderColumn>();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		if (TfAppState.Value.AdminDataProvider is null) throw new Exception("DataProvider not provided");
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await _loadDataAsync();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _loadDataAsync()
	{
		try
		{
			var schemaInfo = await UC.GetDataProviderSourceSchemaInfo(Content);
			var supportedSourceTypes = Content.ProviderType.SupportedSourceDataTypes;
			foreach (var columnName in schemaInfo.SourceColumnDatabaseType.Keys)
			{
				if (String.IsNullOrWhiteSpace(columnName)) continue;
				var columnString = columnName.Trim().ToLowerInvariant();
				var columnDbType = schemaInfo.SourceColumnDatabaseType[columnName];
				var matchColumn = Content.Columns.FirstOrDefault(x => x.SourceName?.Trim().ToLowerInvariant() == columnString);
				if (matchColumn is null)
				{
					_newColumns.Add(new TucDataProviderColumn
					{
						Id = Guid.NewGuid(),
						SourceName = columnName,
						SourceType = schemaInfo.DatabaseTypeToSourceType[columnDbType],
						CreatedOn = DateTime.Now,
						DataProviderId = Content.Id,
						DbName = TfConverters.GenerateDbNameFromText(columnName),
						DbType = new TucDatabaseColumnTypeInfo(columnDbType),
					});
				}
				else
				{
					_existingColumns.Add(matchColumn);
				}
			}
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
		}
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}


}
