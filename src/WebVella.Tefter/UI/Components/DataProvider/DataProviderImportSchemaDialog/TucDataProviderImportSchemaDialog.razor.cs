﻿using System;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.DataProviderImportSchemaDialog.TfDataProviderImportSchemaDialog", "WebVella.Tefter")]
public partial class TucDataProviderImportSchemaDialog : TfBaseComponent, IDialogContentComponent<TfDataProvider?>
{
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public TfDataProvider? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private bool _isBusy = true;
	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _activeTab = "new";

	private List<TfDataProviderColumn> _newColumns = new List<TfDataProviderColumn>();
	private List<TfDataProviderColumn> _existingColumns = new List<TfDataProviderColumn>();
	private TfDataProviderSourceSchemaInfo _schemaInfo = new TfDataProviderSourceSchemaInfo();
	public virtual List<ValidationError> ValidationErrors { get; set; } = new();

	private Dictionary<TfDatabaseColumnType, DatabaseColumnTypeInfo> _dbTypeInfoDict = new();


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
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

	private void _loadData()
	{
		if (Content is null) return;
		try
		{
			var dbTypeInfo = TfDataProviderUIService.GetDatabaseColumnTypeInfosList();
			_dbTypeInfoDict = dbTypeInfo.ToDictionary(x=> x.Type);
			_schemaInfo = TfDataProviderUIService.GetDataProviderSourceSchemaInfo(Content);
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
					_newColumns.Add(new TfDataProviderColumn
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
				if ((matchColumn.DbName ?? String.Empty).StartsWith(Content!.ColumnPrefix))
					continue;

				matchColumn.FixPrefix(Content!.ColumnPrefix);
			}

			var provider = TfDataProviderUIService.CreateBulkDataProviderColumn(Content!.Id, _newColumns);
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

	private void _removeRow(TfDataProviderColumn column)
	{
		_newColumns.Remove(column);
	}

	private void _sourceTypeChanged(string option, TfDataProviderColumn column)
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
	private void _dbNameChanged(string option, TfDataProviderColumn column)
	{
		column.DbName = option;
	}

	private void _searchTypeChanged(TfDataProviderColumn column, TfDataProviderColumnSearchType type)
	{
		column.PreferredSearchType = type;
	}

	private void _nullableChanged(TfDataProviderColumn column)
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
		if (!columnName.StartsWith(Content!.ColumnPrefix))
			return columnName;

		return columnName.Substring(Content!.ColumnPrefix.Length); //it is not -1 as it needs to account for the _
	}
}
