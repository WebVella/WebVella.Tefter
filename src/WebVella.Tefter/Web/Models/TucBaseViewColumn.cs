using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.Models;

public interface ITucExportableViewColumn
{
	object GetData(IServiceProvider serviceProvider);
}


public abstract class TucBaseViewColumn<TItem> : ComponentBase, IAsyncDisposable, ITucExportableViewColumn, ITucAuxDataUseComponent
{
	#region << Injects >>
	[Inject] protected IJSRuntime JSRuntime { get; set; }
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	#endregion

	#region << Properties >>
	[Parameter] public TucViewColumnComponentContext Context { get; set; }
	[Parameter] public EventCallback<string> OptionsChanged { get; set; }
	[Parameter] public EventCallback<TfDataTable> RowChanged { get; set; }

	protected IStringLocalizer LC;
	protected virtual TItem componentOptions { get; set; }
	protected string optionsSerialized = null;
	#endregion

	#region << Lifecycle >>
	/// <summary>
	/// If overrided do not forget to call it
	/// </summary>
	/// <returns></returns>
	public virtual ValueTask DisposeAsync()
	{
		if (Context is not null && Context.EditContext is not null)
		{
			Context.EditContext.OnValidationRequested -= OnOptionsValidationRequested;
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
		if (Context.EditContext is not null)
			Context.EditContext.OnValidationRequested += OnOptionsValidationRequested;

	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (Context.CustomOptionsJson != optionsSerialized)
		{
			optionsSerialized = Context.CustomOptionsJson;
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
		if (Context.DataMapping.ContainsKey(alias))
		{
			colName = Context.DataMapping[alias];
		}

		return colName;
	}

	/// <summary>
	/// gets the database column type of the mapped column to the alias
	/// </summary>
	/// <param name="alias"></param>
	/// <returns></returns>
	protected virtual TucDatabaseColumnType? GetColumnDatabaseTypeByAlias(string alias)
	{
		if (Context.DataTable is null) return null;
		var colName = GetColumnNameFromAlias(alias);
		if (colName == null) return null;

		TfDataColumn column = null;
		try
		{
			column = Context.DataTable.Columns[colName];
		}
		catch { }
		if (column == null) return null;

		return column.DbType.ConvertSafeToEnum<TfDatabaseColumnType, TucDatabaseColumnType>();
	}

	protected virtual object GetColumnDataByAlias(string alias)
	{
		var colName = GetColumnNameFromAlias(alias);
		var colDbType = GetColumnDatabaseTypeByAlias(alias);
		if (colName is null || colDbType is null) return null;

		switch (colDbType)
		{
			case TucDatabaseColumnType.ShortInteger:
				return GetDataStructByAlias<short>(alias);
			case TucDatabaseColumnType.AutoIncrement:
			case TucDatabaseColumnType.Integer:
				return GetDataStructByAlias<int>(alias);
			case TucDatabaseColumnType.LongInteger:
				return GetDataStructByAlias<long>(alias);
			case TucDatabaseColumnType.Number:
				return GetDataStructByAlias<decimal>(alias);
			case TucDatabaseColumnType.Boolean:
				return GetDataStructByAlias<bool>(alias);
			case TucDatabaseColumnType.Date:
				return GetDataStructByAlias<DateOnly>(alias);
			case TucDatabaseColumnType.DateTime:
				return GetDataStructByAlias<DateTime>(alias);
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				return GetDataStringByAlias(alias);
			case TucDatabaseColumnType.Guid:
				return GetDataStructByAlias<Guid>(alias);
			default:
				throw new Exception("colDbType not supported");
		}
	}

	protected virtual Type GetColumnObjectTypeByAlias(string alias)
	{
		var colName = GetColumnNameFromAlias(alias);
		var colDbType = GetColumnDatabaseTypeByAlias(alias);
		if (colName is null || colDbType is null) return null;

		switch (colDbType)
		{
			case TucDatabaseColumnType.ShortInteger:
				return typeof(short);
			case TucDatabaseColumnType.AutoIncrement:
			case TucDatabaseColumnType.Integer:
				return typeof(int);
			case TucDatabaseColumnType.LongInteger:
				return typeof(long);
			case TucDatabaseColumnType.Number:
				return typeof(decimal);
			case TucDatabaseColumnType.Boolean:
				return typeof(bool);
			case TucDatabaseColumnType.Date:
				return typeof(DateOnly);
			case TucDatabaseColumnType.DateTime:
				return typeof(DateTime);
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				return typeof(string);
			case TucDatabaseColumnType.Guid:
				return typeof(Guid);
			default:
				throw new Exception("colDbType not supported");
		}
	}

	protected virtual object ConvertStringToColumnObjectByAlias(string alias, string stringValue)
	{
		var colName = GetColumnNameFromAlias(alias);
		var colDbType = GetColumnDatabaseTypeByAlias(alias);
		if (colName is null || colDbType is null) return null;

		switch (colDbType)
		{
			case TucDatabaseColumnType.ShortInteger:
				return TfConverters.Convert<short>(stringValue);
			case TucDatabaseColumnType.AutoIncrement:
			case TucDatabaseColumnType.Integer:
				return TfConverters.Convert<int>(stringValue);
			case TucDatabaseColumnType.LongInteger:
				return TfConverters.Convert<long>(stringValue);
			case TucDatabaseColumnType.Number:
				return TfConverters.Convert<decimal>(stringValue);
			case TucDatabaseColumnType.Boolean:
				return TfConverters.Convert<bool>(stringValue);
			case TucDatabaseColumnType.Date:
				return TfConverters.Convert<DateOnly>(stringValue);
			case TucDatabaseColumnType.DateTime:
				return TfConverters.Convert<DateTime>(stringValue);
			case TucDatabaseColumnType.ShortText:
			case TucDatabaseColumnType.Text:
				return stringValue;
			case TucDatabaseColumnType.Guid:
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
	protected virtual string GetDataStringByAlias(string alias, string defaultValue = null)
	{
		string dbName = GetColumnNameFromAlias(alias);

		if (String.IsNullOrWhiteSpace(dbName))
		{
			return defaultValue;
		}
		if (Context.DataTable is null || Context.DataTable.Rows.Count == 0) return defaultValue;

		if (Context.DataTable.Rows.Count < Context.RowIndex + 1) return defaultValue;
		if (Context.DataTable.Rows[Context.RowIndex][dbName] is null) return defaultValue;

		return Context.DataTable.Rows[Context.RowIndex][dbName]?.ToString();
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
	protected virtual Nullable<T> GetDataStructByAlias<T>(string alias, Nullable<T> defaultValue = null) where T : struct
	{
		string dbName = GetColumnNameFromAlias(alias);
		if (String.IsNullOrWhiteSpace(dbName))
		{
			return null;
		}
		if (Context.DataTable is null || Context.DataTable.Rows.Count == 0) return defaultValue;

		if (Context.DataTable.Rows.Count < Context.RowIndex + 1) return null;
		if (Context.DataTable.Rows[Context.RowIndex][dbName] is null) return null;
		object value = Context.DataTable.Rows[Context.RowIndex][dbName];
		if (value is null) return null;

		if (typeof(T) == typeof(String)) return (T)value;
		else if (value is T) return (T)value;
		return TfConverters.Convert<T>(value.ToString());
	}

	/// <summary>
	/// Gets more complex object data which is in JSOn format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="alias"></param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	protected virtual T GetDataObjectFromJsonByAlias<T>(string alias, T defaultValue = null) where T : class
	{
		string dbName = GetColumnNameFromAlias(alias);
		if (String.IsNullOrWhiteSpace(dbName))
		{
			return null;
		}
		if (Context.DataTable is null || Context.DataTable.Rows.Count == 0) return defaultValue;

		if (Context.DataTable.Rows.Count < Context.RowIndex + 1) return null;
		if (Context.DataTable.Rows[Context.RowIndex][dbName] is null) return null;
		object value = Context.DataTable.Rows[Context.RowIndex][dbName];
		if (value is null) return null;
		if (value is string && String.IsNullOrWhiteSpace((string)value)) return null;
		if (value is T) return (T)value;

		try
		{
			return JsonSerializer.Deserialize<T>(value.ToString());
		}
		catch { throw new Exception("Value cannot be parsed"); }

	}

	/// <summary>
	/// Base methods for dealing with options
	/// </summary>
	protected virtual TItem GetOptions()
	{
		if (!String.IsNullOrWhiteSpace(Context.CustomOptionsJson))
		{
			componentOptions = JsonSerializer.Deserialize<TItem>(Context.CustomOptionsJson);
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
		if (value is IConvertible)
			propertyInfo.SetValue(componentOptions, Convert.ChangeType(value, propertyInfo.PropertyType), null);
		else if (value is Guid)
			propertyInfo.SetValue(componentOptions, (Guid?)value, null);
		else throw new Exception("Not supported object value type");
		if (!OptionsChanged.HasDelegate) return;
		await OptionsChanged.InvokeAsync(JsonSerializer.Serialize(componentOptions));
	}

	/// <summary>
	/// Called when EditContext.Validate is triggered by the parent component 
	/// when component options needs to be saved
	/// Override in child component. Add possible validation errors with:
	/// Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "your message here");
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
	public virtual TfDataColumn GetColumnInfoByAlias(string alias)
	{
		var columnName = GetColumnNameFromAlias(alias);
		if (String.IsNullOrWhiteSpace(columnName)) return null;
		if (Context.DataTable is null) return null;
		return Context.DataTable.Columns[columnName];
	}


	/// <summary>
	/// This method needs to be overriden in the implementing component,
	/// and will be called by various export services as Excel export in example
	/// </summary>
	public virtual object GetData(IServiceProvider serviceProvider)
	{
		return null;
	}

	/// <summary>
	/// This method expects a datatable with a single row (in most cases) 
	/// with the updated data for that row
	/// </summary>
	/// <param name="dt"></param>
	/// <returns></returns>
	protected virtual async Task OnRowChanged(TfDataTable dt)
	{
		await RowChanged.InvokeAsync(dt);
	}

	/// <summary>
	/// This method expects a datatable with a single row (in most cases) 
	/// with the updated data for that row
	/// </summary>
	protected virtual async Task OnRowColumnChangedByAlias(string alias, object value)
	{
		if (!RowChanged.HasDelegate) return;

		var dt = Context.DataTable.NewTable(Context.RowIndex);
		if (dt.Rows.Count == 0)
		{
			ToastService.ShowError(LOC("Row with index {0} is not found", Context.RowIndex));
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

	/// <summary>
	/// This method will be called after all the baseline space view state
	/// is initialized in TfAppState.
	/// Usually used for space view column component initialization of initial data
	/// in TfAuxDataState.
	/// The usual context with all the view meta and data is available when this method is called
	/// </summary>
	/// <param name="appState">the most current complete appState reference</param>
	public virtual Task OnAppStateInit(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		return Task.CompletedTask;
	}
	#endregion
}