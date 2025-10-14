using System;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.UI.Components;
public partial class TucDataProviderImportSchemaDialog : TfBaseComponent, IDialogContentComponent<TfDataProvider?>
{
	[Parameter] public TfDataProvider? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _activeTab = "new";
	private List<TfMenuItem> _tabs = new();
	
	private List<TfUpsertDataProviderColumn> _newColumns = new List<TfUpsertDataProviderColumn>();
	private List<TfUpsertDataProviderColumn> _existingColumns = new List<TfUpsertDataProviderColumn>();
	private TfDataProviderSourceSchemaInfo _schemaInfo = new TfDataProviderSourceSchemaInfo();
	public virtual List<ValidationError> ValidationErrors { get; set; } = new();

	private Dictionary<TfDatabaseColumnType, DatabaseColumnTypeInfo> _dbTypeInfoDict = new();


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_initMenu();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_loadData();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _initMenu()
	{
		_tabs = new();
		_tabs.Add(new TfMenuItem()
		{
			Id = "new",
			Text = LOC("New Columns"),
			Selected = _activeTab == "new",
			OnClick = EventCallback.Factory.Create(this, ()=> _menuClick("new"))
		});		
		_tabs.Add(new TfMenuItem()
		{
			Id = "existing",
			Text = LOC("Existing Columns"),
			Selected = _activeTab == "existing",
			OnClick = EventCallback.Factory.Create(this, ()=> _menuClick("existing"))
		});				
	}

	private void _menuClick(string menu)
	{
		foreach (var tab in _tabs)
		{
			if (tab.Id == menu)
				tab.Selected = true;
			else
				tab.Selected = false;
		}

		_activeTab = menu;
		StateHasChanged();
	}

	private void _loadData()
	{
		if (Content is null) return;
		try
		{
			var dbTypeInfo = WebVella.Tefter.Services.TfService.GetDatabaseColumnTypeInfosList();
			_dbTypeInfoDict = dbTypeInfo.ToDictionary(x=> x.Type);
			_schemaInfo = TfService.GetDataProviderSourceSchemaInfo(Content.Id);
			if (_schemaInfo is null) 
				throw new Exception("NO _schemaInfo");
			var supportedSourceTypes = Content.SupportedSourceDataTypes;
			foreach (var columnName in _schemaInfo.SourceColumnDefaultDbType.Keys)
			{
				if (String.IsNullOrWhiteSpace(columnName)) continue;
				var matchColumn = Content.Columns.FirstOrDefault(x => x.SourceName?.Trim().ToLowerInvariant() == columnName.Trim().ToLowerInvariant());
				if (matchColumn is null)
				{
					string? defaultValue = _schemaInfo.SourceColumnDefaultValue.ContainsKey(columnName) ? _schemaInfo.SourceColumnDefaultValue[columnName] : null;
					_newColumns.Add(new TfUpsertDataProviderColumn
					{
						Id = Guid.NewGuid(),
						SourceName = columnName,
						SourceType = _schemaInfo.SourceColumnDefaultSourceType[columnName],
						CreatedOn = DateTime.Now,
						DataProviderId = Content.Id,
						DbName = columnName.GenerateDbNameFromText(),
						DbType = _schemaInfo.SourceColumnDefaultDbType[columnName],
						DefaultValue = defaultValue,
						IncludeInTableSearch = false,
						RuleSet = TfDataProviderColumnRuleSet.Nullable
					});
				}
				else
				{
					_existingColumns.Add(matchColumn.ToUpsert());
				}
			}
		}
		catch (Exception ex)
		{
			_error = ProcessException(ex);
		}
	}

	protected string _getValidationCssClass(string sourceName, string propName)
	{
		return ValidationErrors.Any(x => x.PropertyName == $"{sourceName}-{propName}") ? "invalid" : "";
	}
	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);

			foreach (var matchColumn in _newColumns)
			{
				if ((matchColumn.DbName ?? String.Empty).StartsWith(Content!.ColumnPrefix))
					continue;

				matchColumn.FixPrefix(Content!.ColumnPrefix);
			}

			var provider = TfService.CreateBulkDataProviderColumn(Content!.Id, _newColumns);
			await Dialog.CloseAsync(provider);
		}
		catch (TfValidationException ex)
		{
			ValidationErrors = new();

			var valEx = (TfValidationException)ex;
			ValidationErrors.AddRange(valEx.GetDataAsValidationErrorList());

			if (ValidationErrors.Count > 0 && string.IsNullOrWhiteSpace(valEx.Message))
				ToastService.ShowWarning(LOC("Invalid Data"));
			else
				ToastService.ShowWarning(valEx.Message);
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

	private void _removeRow(TfUpsertDataProviderColumn column)
	{
		_newColumns.Remove(column);
	}

	private void _sourceTypeChanged(string option, TfUpsertDataProviderColumn column)
	{
		column.SourceType = option;
		TfDatabaseColumnType defaultDbType = TfDatabaseColumnType.Text;
		bool sourceTypeHasDbType = false;
		if (_schemaInfo.SourceTypeSupportedDbTypes.ContainsKey(column.SourceType)
		&& _schemaInfo.SourceTypeSupportedDbTypes[column.SourceType].Count > 0)
		{
			defaultDbType = _schemaInfo.SourceTypeSupportedDbTypes[column.SourceType][0];
			sourceTypeHasDbType = true;
		}
		if (sourceTypeHasDbType && _schemaInfo.SourceTypeSupportedDbTypes[column.SourceType].Contains(column.DbType))
		{
			//keep current
		}
		else
		{
			column.DbType = defaultDbType;
		}
	}
	private void _dbNameChanged(string option, TfUpsertDataProviderColumn column)
	{
		column.DbName = option;
	}

	private string _processColumnName(string columnName)
	{
		if (!columnName.StartsWith(Content!.ColumnPrefix))
			return columnName;

		return columnName.Substring(Content!.ColumnPrefix.Length); //it is not -1 as it needs to account for the _
	}
}
