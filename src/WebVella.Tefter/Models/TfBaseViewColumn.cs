
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Models;

public interface ITfExportableViewColumn
{
	object GetData();
}

public class TfBaseViewColumn<TItem> : ComponentBase, IAsyncDisposable, ITfExportableViewColumn
{
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	[Parameter] public TfComponentContext Context { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	public TfBaseViewColumn()
	{
	}
	public TfBaseViewColumn(TfComponentContext context)
	{
		Context = context;
	}

	protected IStringLocalizer LC;
	protected virtual TItem options { get; set; } = Activator.CreateInstance<TItem>();
	protected string optionsSerialized = null;

	/// <summary>
	/// If overrided do not forget to call it
	/// </summary>
	/// <returns></returns>
	public virtual ValueTask DisposeAsync()
	{
		if (Context is not null && Context.EditContext is not null)
		{
			Context.EditContext.OnValidationRequested -= OnValidationRequested;
		}
		return ValueTask.CompletedTask;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
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
			Context.EditContext.OnValidationRequested += OnValidationRequested;

	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.CustomOptionsJson != optionsSerialized)
		{
			optionsSerialized = Context.CustomOptionsJson;
			options = GetOptions();
		}
	}

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
	/// Called when EditContext.Validate is triggered by the parent component. Such cases are when component options needs to be saved
	/// or when a component value needs to be updated in the datatable of the component updates it.
	/// Override in child component. Add possible validation errors with:
	/// Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "your message here");
	/// Note: in the above change only the message text
	/// </summary>
	protected virtual void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		//Should be overrided in child component if needed
	}

	/// <summary>
	/// The implementing components are referencing data based on the Data Mapping provided by the user, 
	/// which maps value needed by the component and its corresponding datatable comlumn name.
	/// This method deals when the value needs to be returned as a string
	/// </summary>
	/// <param name="alias">the expected data alias as defined by the implementing component</param>
	/// <param name="defaultValue">what value to return if value is not found in the provided datatable</param>
	/// <returns></returns>
	protected virtual string GetDataObjectByAlias(string alias, string defaultValue = null)
	{
		string dbName = null;
		if (Context.DataMapping.ContainsKey(alias))
		{
			dbName = Context.DataMapping[alias];
		}

		if (String.IsNullOrWhiteSpace(dbName))
		{
			return defaultValue;
		}
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
	protected virtual Nullable<T> GetDataObjectByAlias<T>(string alias, Nullable<T> defaultValue = null) where T : struct
	{
		string dbName = null;
		if (Context.DataMapping.ContainsKey(alias))
		{
			dbName = Context.DataMapping[alias];
		}
		if (String.IsNullOrWhiteSpace(dbName))
		{
			return null;
		}
		if (Context.DataTable.Rows.Count < Context.RowIndex + 1) return null;
		if (Context.DataTable.Rows[Context.RowIndex][dbName] is null) return null;
		object value = Context.DataTable.Rows[Context.RowIndex][dbName];
		if (value is null) return null;

		if (typeof(T) == typeof(String)) return (T)value;
		else if (value is T) return (T)value;
		return TfConverters.Convert<T>(value.ToString());
	}

	/// <summary>
	/// Base methods for dealing with options
	/// </summary>
	protected virtual TItem GetOptions()
	{
		if (!String.IsNullOrWhiteSpace(Context.CustomOptionsJson))
		{
			options = JsonSerializer.Deserialize<TItem>(Context.CustomOptionsJson);
			if (options is not null) return options;
		}
		return Activator.CreateInstance<TItem>();
	}

	/// <summary>
	/// Base method for dealing with options value changes in the implementing component
	/// </summary>
	/// <param name="propName"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	protected virtual async Task OptionsValueChanged(string propName, object value)
	{

		PropertyInfo propertyInfo = typeof(TItem).GetProperty(propName);
		if (propertyInfo is null) return;
		propertyInfo.SetValue(options, Convert.ChangeType(value, propertyInfo.PropertyType), null);
		if (!ValueChanged.HasDelegate) return;
		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(options));
	}

	/// <summary>
	/// This method needs to be overriden in the implementing component,
	/// and will be called by various export services as Excel export in example
	/// </summary>
	public virtual object GetData()
	{
		return null;
	}

	
}