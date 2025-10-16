using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.UI.Layout;

namespace WebVella.Tefter.Models;

public abstract class TucBaseViewColumn<TItem> : ComponentBase, IAsyncDisposable//, ITfAuxDataState
{
	#region << Injects >>
	[Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Inject] protected IDialogService DialogService { get; set; } = null!;
	[Inject] protected IMessageService MessageService { get; set; } = null!;
	[Inject] protected ITfService TfService { get; set; } = null!;
	[Inject] protected ITfMetaService TfMetaService { get; set; } = null!;
	[CascadingParameter(Name = "TfAuthLayout")] public TfAuthLayout TfAuthLayout { get; set; } = null!;

	#endregion

	#region << Properties >>
	[Parameter] public TfSpaceViewColumnScreenRegionContext RegionContext { get; set; } = null!;

	//Internal
	protected IStringLocalizer LC;
	protected TItem componentOptions { get; set; }
	protected string optionsSerialized = null;
	#endregion

	#region << Lifecycle >>
	/// <summary>
	/// If overrided do not forget to call it
	/// </summary>
	/// <returns></returns>
	public virtual ValueTask DisposeAsync()
	{
		if (RegionContext is not null && RegionContext.EditContext is not null)
		{
			RegionContext.EditContext.OnValidationRequested -= OnOptionsValidationRequested;
		}
		return ValueTask.CompletedTask;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		componentOptions = GetOptions();
		var type = this.GetType();
		var (resourceBaseName, resourceLocation) = type.GetLocalizationResourceInfo();
		if (!String.IsNullOrWhiteSpace(resourceBaseName) && !String.IsNullOrWhiteSpace(resourceLocation))
		{
			LC = StringLocalizerFactory.Create(resourceBaseName, resourceLocation);
		}
		else
		{
			LC = StringLocalizerFactory.Create(type);
		}
		if (RegionContext.EditContext is not null)
			RegionContext.EditContext.OnValidationRequested += OnOptionsValidationRequested;
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (RegionContext.TypeOptionsJson != optionsSerialized)
		{
			optionsSerialized = RegionContext.TypeOptionsJson;
			componentOptions = GetOptions();
		}
	}
	#endregion

	#region << Base methods >>
	/// <summary>
	/// Base method for dealing with localization, based on localization resources provided in the implementing component
	/// </summary>
	/// <param name="key">text that needs to be matched. Can include replacement tags as {0},{1}...</param>
	/// <param name="arguments">arguments that will replace the tags in the same order</param>
	/// <returns></returns>
	protected virtual string LOC(string key, params object[] arguments)
	{
		if (LC is not null && LC[key, arguments] != key) return LC[key, arguments];
		return key;
	}

	/// <summary>
	/// Gets the data provider column name from component alias data mapping
	/// </summary>
	/// <param name="alias">the name that the component uses to get data. 
	/// Needs to be mapped in configuration to a real data column</param>
	/// <returns></returns>
	protected virtual string GetColumnNameFromAlias(string alias)
	{
		string colName = null;
		if (RegionContext.DataMapping.ContainsKey(alias))
		{
			colName = RegionContext.DataMapping[alias];
		}

		return colName;
	}

	/// <summary>
	/// gets the database column type of the mapped column to the alias
	/// </summary>
	/// <param name="alias"></param>
	/// <returns></returns>
	protected virtual TfDataColumn? GetColumnByAlias(string alias)
	{
		if (RegionContext.DataTable is null) return null;
		var colName = GetColumnNameFromAlias(alias);
		if (String.IsNullOrWhiteSpace(colName)) return null;

		return RegionContext.DataTable.Columns[colName];
	}

	protected virtual object? GetColumnData(TfDataColumn? column)
	{
		if (column is null) return null;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				return GetDataStructByAlias<short>(column);
			case TfDatabaseColumnType.Integer:
				return GetDataStructByAlias<int>(column);
			case TfDatabaseColumnType.LongInteger:
				return GetDataStructByAlias<long>(column);
			case TfDatabaseColumnType.Number:
				return GetDataStructByAlias<decimal>(column);
			case TfDatabaseColumnType.Boolean:
				return GetDataStructByAlias<bool>(column);
			case TfDatabaseColumnType.DateOnly:
				return GetDataStructByAlias<DateOnly>(column);
			case TfDatabaseColumnType.DateTime:
				return GetDataStructByAlias<DateTime>(column);
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return GetDataStringByAlias(column);
			case TfDatabaseColumnType.Guid:
				return GetDataStructByAlias<Guid>(column);
			default:
				throw new Exception("colDbType not supported");
		}
	}

	protected virtual Type? GetColumnObjectType(TfDataColumn? column)
	{
		if (column is null) return null;
		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				return typeof(short);
			case TfDatabaseColumnType.Integer:
				return typeof(int);
			case TfDatabaseColumnType.LongInteger:
				return typeof(long);
			case TfDatabaseColumnType.Number:
				return typeof(decimal);
			case TfDatabaseColumnType.Boolean:
				return typeof(bool);
			case TfDatabaseColumnType.DateOnly:
				return typeof(DateOnly);
			case TfDatabaseColumnType.DateTime:
				return typeof(DateTime);
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return typeof(string);
			case TfDatabaseColumnType.Guid:
				return typeof(Guid);
			default:
				throw new Exception("colDbType not supported");
		}
	}

	protected virtual object? ConvertStringToColumnObject(TfDataColumn? column, string stringValue)
	{
		if (column is null) return null;

		switch (column.DbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				return TfConverters.Convert<short>(stringValue);
			case TfDatabaseColumnType.Integer:
				return TfConverters.Convert<int>(stringValue);
			case TfDatabaseColumnType.LongInteger:
				return TfConverters.Convert<long>(stringValue);
			case TfDatabaseColumnType.Number:
				return TfConverters.Convert<decimal>(stringValue);
			case TfDatabaseColumnType.Boolean:
				return TfConverters.Convert<bool>(stringValue);
			case TfDatabaseColumnType.DateOnly:
				return TfConverters.Convert<DateOnly>(stringValue);
			case TfDatabaseColumnType.DateTime:
				return TfConverters.Convert<DateTime>(stringValue);
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				return stringValue;
			case TfDatabaseColumnType.Guid:
				return TfConverters.Convert<Guid>(stringValue); ;
			default:
				throw new Exception("colDbType not supported");
		}
	}

	/// <summary>
	/// The implementing components are referencing data based on the Data Mapping provided by the user, 
	/// which maps value needed by the component and its corresponding datatable comlumn name.
	/// This method deals when the value needs to be returned as a string
	/// </summary>
	/// <param name="alias">the expected data alias as defined by the implementing component</param>
	/// <param name="defaultValue">what value to return if value is not found in the provided datatable</param>
	/// <returns></returns>
	protected virtual object? GetDataString(string dbName, string? defaultValue = null)
	{

		if (String.IsNullOrWhiteSpace(dbName))
		{
			return defaultValue;
		}
		if (RegionContext.DataTable is null || RegionContext.DataTable.Rows.Count == 0) return defaultValue;

		if (RegionContext.DataTable.Rows[RegionContext.RowId] is null) return defaultValue;
		if (RegionContext.DataTable[RegionContext.RowId,dbName] is null) return defaultValue;
		object value = RegionContext.DataTable.Rows[RegionContext.RowId]![dbName];
		if (value is null) return defaultValue;
		if (value.GetType().ImplementsInterface(typeof(IList))) return (List<string>)value;
		return value!.ToString();
	}

	/// <summary>
	/// The implementing components are referencing data based on the Data Mapping provided by the user, 
	/// which maps value needed by the component and its corresponding datatable comlumn name.
	/// This method deals when the value needs to be returned as a primitive value different than string
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="alias">the expected data alias as defined by the implementing component</param>
	/// <param name="defaultValue">what value to return if value is not found in the provided datatable</param>
	/// <returns></returns>
	protected virtual object? GetDataStruct<T>(string dbName, Nullable<T> defaultValue = null) where T : struct
	{
		if (String.IsNullOrWhiteSpace(dbName))
		{
			return defaultValue;
		}
		if (RegionContext.DataTable is null || RegionContext.DataTable.Rows.Count == 0) return defaultValue;

		if (RegionContext.DataTable.Rows[RegionContext.RowId] is null) return defaultValue;
		if (RegionContext.DataTable[RegionContext.RowId,dbName] is null) return defaultValue;

		object value = RegionContext.DataTable.Rows[RegionContext.RowId]![dbName];
		if (value is null) return defaultValue;

		if (typeof(T) == typeof(String)) return (T)value;
		else if (value is T) return (T)value;
		else if (value.GetType().ImplementsInterface(typeof(IList))) return (List<Nullable<T>>)value;
		return TfConverters.Convert<T>(value!.ToString());
	}


	/// <summary>
	/// The implementing components are referencing data based on the Data Mapping provided by the user, 
	/// which maps value needed by the component and its corresponding datatable comlumn name.
	/// This method deals when the value needs to be returned as a string
	/// </summary>
	/// <param name="alias">the expected data alias as defined by the implementing component</param>
	/// <param name="defaultValue">what value to return if value is not found in the provided datatable</param>
	/// <returns></returns>
	protected virtual object GetDataStringByAlias(TfDataColumn column, string defaultValue = null)
	{
		return GetDataString(column.Name, defaultValue);
	}

	/// <summary>
	/// The implementing components are referencing data based on the Data Mapping provided by the user, 
	/// which maps value needed by the component and its corresponding datatable comlumn name.
	/// This method deals when the value needs to be returned as a primitive value different than string
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="alias">the expected data alias as defined by the implementing component</param>
	/// <param name="defaultValue">what value to return if value is not found in the provided datatable</param>
	/// <returns></returns>
	protected virtual object GetDataStructByAlias<T>(TfDataColumn column, Nullable<T> defaultValue = null) where T : struct
	{
		return GetDataStruct<T>(column.Name, defaultValue);
	}

	/// <summary>
	/// Gets more complex object data which is in JSOn format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="alias"></param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	protected virtual T? GetDataObjectFromJsonByAlias<T>(TfDataColumn column, T defaultValue = null) where T : class
	{
		if (RegionContext.DataTable is null || RegionContext.DataTable.Rows.Count == 0) return defaultValue;

		if (RegionContext.DataTable.Rows[RegionContext.RowId] is null) return null;
		if (RegionContext.DataTable[RegionContext.RowId,column.Name] is null) return null;
		object value = RegionContext.DataTable.Rows[RegionContext.RowId]![column.Name];
		if (value is null) return null;
		if (value is string && String.IsNullOrWhiteSpace((string)value)) return null;
		if (value is T) return (T)value;

		try
		{
			return JsonSerializer.Deserialize<T>(value!.ToString());
		}
		catch { throw new Exception("Value cannot be parsed"); }

	}

	/// <summary>
	/// Base methods for dealing with options
	/// </summary>
	protected virtual TItem GetOptions()
	{
		if (!String.IsNullOrWhiteSpace(RegionContext.TypeOptionsJson))
		{
			componentOptions = JsonSerializer.Deserialize<TItem>(RegionContext.TypeOptionsJson);
			if (componentOptions is not null) return componentOptions;
		}
		return Activator.CreateInstance<TItem>();
	}

	/// <summary>
	/// Base method for dealing with options value changes in the implementing component
	/// </summary>
	/// <param name="propName"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	protected virtual async Task OnOptionsChanged(string propName, object value)
	{

		PropertyInfo propertyInfo = typeof(TItem).GetProperty(propName);
		if (propertyInfo is null) return;
		if (value is null)
			propertyInfo.SetValue(componentOptions, null, null);
		else if (value is IConvertible)
			propertyInfo.SetValue(componentOptions, Convert.ChangeType(value, propertyInfo.PropertyType), null);
		else if (value is Guid)
			propertyInfo.SetValue(componentOptions, (Guid?)value, null);
		else throw new Exception("Not supported object value type");
		if (!RegionContext.OptionsChanged.HasDelegate) return;
		await RegionContext.OptionsChanged.InvokeAsync(JsonSerializer.Serialize(componentOptions));
	}

	/// <summary>
	/// Called when EditContext.Validate is triggered by the parent component 
	/// when component options needs to be saved
	/// Override in child component. Add possible validation errors with:
	/// Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.ComponentOptionsJson)), "your message here");
	/// Note: in the above change only the message text
	/// </summary>
	protected virtual void OnOptionsValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		//Should be overrided in child component if needed
	}

	/// <summary>
	/// This method needs to be overriden in the implementing component,
	/// and will be called by various export services as Excel export in example
	/// </summary>
	public virtual void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell) { }

	/// <summary>
	/// This method needs to be overriden in the implementing component,
	/// and will be called by various export services as CSV export in example
	/// </summary>
	public virtual string? GetValueAsString(IServiceProvider serviceProvider) => null;


	/// <summary>
	/// Base method for dealing with options value changes in the implementing component
	/// </summary>
	/// <param name="propName"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	protected virtual async Task OnDataMappingChanged(string propName, string value)
	{

		await RegionContext.DataMappingChanged.InvokeAsync(new Tuple<string, string>(propName,value));
	}

	/// <summary>
	/// This method expects a datatable with a single row (in most cases) 
	/// with the updated data for that row
	/// </summary>
	/// <param name="dt"></param>
	/// <returns></returns>
	protected virtual async Task OnRowChanged(TfDataTable dt)
	{
		await RegionContext.RowChanged.InvokeAsync(dt);
	}

	/// <summary>
	/// This method expects a datatable with a single row (in most cases) 
	/// with the updated data for that row
	/// </summary>
	protected virtual async Task OnRowColumnChangedByAlias(string alias, object value)
	{
		if (!RegionContext.RowChanged.HasDelegate || RegionContext.DataTable is null) return;

		var dt = RegionContext.DataTable.NewTableFromRowIds(RegionContext.RowId);
		if (dt.Rows.Count == 0)
		{
			ToastService.ShowError(LOC("Row with id {0} is not found", RegionContext.RowId));
			return;
		}
		var colName = GetColumnNameFromAlias(alias);
		if (String.IsNullOrWhiteSpace(colName))
		{
			ToastService.ShowError(LOC("Column for the alias {0} is not found", alias));
			return;
		}
		dt.Rows[0][colName] = value;

		await OnRowChanged(dt);

	}
	#endregion
}