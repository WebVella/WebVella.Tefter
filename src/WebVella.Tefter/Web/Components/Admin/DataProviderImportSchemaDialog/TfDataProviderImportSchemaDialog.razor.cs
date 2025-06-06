﻿using System;
using WebVella.Tefter.Exceptions;

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
	private TucDataProviderSourceSchemaInfo _schemaInfo = new TucDataProviderSourceSchemaInfo();
	public virtual List<ValidationError> ValidationErrors { get; set; } = new();


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
			_schemaInfo = await UC.GetDataProviderSourceSchemaInfo(Content);
			var supportedSourceTypes = Content.ProviderType.SupportedSourceDataTypes;
			foreach (var columnName in _schemaInfo.SourceColumnDefaultDbType.Keys)
			{
				if (String.IsNullOrWhiteSpace(columnName)) continue;
				var matchColumn = Content.Columns.FirstOrDefault(x => x.SourceName?.Trim().ToLowerInvariant() == columnName.Trim().ToLowerInvariant());
				if (matchColumn is null)
				{
					string defaultValue = _schemaInfo.SourceColumnDefaultValue.ContainsKey(columnName) ? _schemaInfo.SourceColumnDefaultValue[columnName] : null;
					_newColumns.Add(new TucDataProviderColumn
					{
						Id = Guid.NewGuid(),
						SourceName = columnName,
						SourceType = _schemaInfo.SourceColumnDefaultSourceType[columnName],
						CreatedOn = DateTime.Now,
						DataProviderId = Content.Id,
						DbName = columnName.GenerateDbNameFromText(),
						DbType = _schemaInfo.SourceColumnDefaultDbType[columnName],
						DefaultValue = defaultValue
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
				if (matchColumn.DbName.StartsWith(TfAppState.Value.AdminDataProvider.ColumnPrefix))
					continue;
				matchColumn.DbName = TfAppState.Value.AdminDataProvider.ColumnPrefix + matchColumn.DbName;
			}

			var provider = UC.CreateBulkDataProviderColumn(Content.Id, _newColumns);
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

	private void _removeRow(TucDataProviderColumn column)
	{
		_newColumns.Remove(column);
	}

	private void _sourceTypeChanged(string option, TucDataProviderColumn column)
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
	private void _dbNameChanged(string option, TucDataProviderColumn column)
	{
		column.DbName = option;
	}

	private void _searchTypeChanged(TucDataProviderColumn column, TfDataProviderColumnSearchType type)
	{
		column.PreferredSearchType = type;
	}

	private void _nullableChanged(TucDataProviderColumn column)
	{
		if (column.IsNullable)
		{
			column.DefaultValue = null;
		}
		else
		{
			if (_schemaInfo.SourceColumnDefaultValue.ContainsKey(column.DbName))
				column.DefaultValue = _schemaInfo.SourceColumnDefaultValue[column.DbName];
			else
				column.DefaultValue = null;
		}
	}

	private string _processColumnName(string columnName)
	{
		if (!columnName.StartsWith(TfAppState.Value.AdminDataProvider.ColumnPrefix))
			return columnName;

		return columnName.Substring(TfAppState.Value.AdminDataProvider.ColumnPrefix.Length); //it is not -1 as it needs to account for the _
	}
}
