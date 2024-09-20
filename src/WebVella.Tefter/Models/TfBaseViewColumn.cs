
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Models;

public interface ITfExportableViewColumn
{
	TfBaseViewColumnExportData GetExportData();
}

public class TfBaseViewColumn<TItem> : ComponentBase, IAsyncDisposable, ITfExportableViewColumn
{
	public TfBaseViewColumn()
	{
	}
	public TfBaseViewColumn(TfComponentContext context)
	{
		Context = context;
	}

	[Parameter]
	public TfComponentContext Context { get; set; }

	[Parameter]
	public EventCallback<string> ValueChanged { get; set; }

	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }

	protected IStringLocalizer LC;

	protected virtual TItem options { get; set; }
	protected string optionsSerialized = null;

	public ValueTask DisposeAsync()
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
		if (String.IsNullOrWhiteSpace(Context.CustomOptionsJson))
		{
			options = default;
			optionsSerialized = JsonSerializer.Serialize(options);
		}
		else if (Context.CustomOptionsJson != optionsSerialized)
		{
			options = JsonSerializer.Deserialize<TItem>(Context.CustomOptionsJson);
		}
	}

	protected string LOC(string key, params object[] arguments)
	{
		if (LC is not null && LC[key, arguments] != key) return LC[key, arguments];
		return key;
	}

	/// <summary>
	/// Called when EditContext.Validate is triggered by the parent component
	/// Override in child component
	/// Add possible validation errors with:
	/// Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "your message here");
	/// Note: in the above change only the message text
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected virtual void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		//Should be overrided in child component if needed
	}

	protected string GetDataObjectByAlias(string alias, string defaultValue = null)
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

	protected Nullable<T> GetDataObjectByAlias<T>(string alias, Nullable<T> defaultValue = null) where T : struct
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
		else if (typeof(T) == typeof(Boolean))
		{
			if (value is Boolean) return (T)value;
			if (Boolean.TryParse(value.ToString(), out Boolean outVal))
			{
				return (T)(object)outVal;
			}
			return null;
		}
		else if (typeof(T) == typeof(DateOnly))
		{
			if (value is DateOnly) return (T)value;
			if (DateOnly.TryParse(value.ToString(), out DateOnly outVal))
			{
				return (T)(object)outVal;
			}
			return null;
		}
		else if (typeof(T) == typeof(DateTime))
		{
			if (value is DateTime) return (T)value;
			if (DateTime.TryParse(value.ToString(), out DateTime outVal))
			{
				return (T)(object)outVal;
			}
			return null;
		}
		else if (typeof(T) == typeof(decimal))
		{
			if (value is decimal) return (T)value;
			if (decimal.TryParse(value.ToString(), out decimal outVal))
			{
				return (T)(object)outVal;
			}
			return null;
		}
		else if (typeof(T) == typeof(int))
		{
			if (value is int) return (T)value;
			if (int.TryParse(value.ToString(), out int outVal))
			{
				return (T)(object)outVal;
			}
			return null;
		}
		else if (typeof(T) == typeof(Guid))
		{
			if (value is Guid) return (T)value;
			if (Guid.TryParse(value.ToString(), out Guid outVal))
			{
				return (T)(object)outVal;
			}
			return null;
		}

		return null;
	}
	public TItem GetOptions()
	{
		if (String.IsNullOrWhiteSpace(Context.CustomOptionsJson))
		{
			return (TItem)default;
		}
		return JsonSerializer.Deserialize<TItem>(Context.CustomOptionsJson);
	}
	public virtual TfBaseViewColumnExportData GetExportData()
	{
		return new TfBaseViewColumnExportData();
	}
}