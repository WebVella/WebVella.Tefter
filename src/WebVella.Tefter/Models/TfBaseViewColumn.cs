
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Models;

public class TfBaseViewColumn<TItem> : ComponentBase, IAsyncDisposable
{
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
		if (Context.EditContext is not null)
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

	/// <summary>
	/// Used to get data by alias and type from the child components
	/// </summary>
	/// <typeparam name="TItem2">result data type</typeparam>
	/// <param name="alias">case sensetive alias as defined in data mapping</param>
	/// <returns></returns>
	protected object GetData<TItem2>(string alias) 
	{
		string dbName = null;
		if(Context.DataMapping.ContainsKey(alias)){ 
			dbName = Context.DataMapping[alias];
		}

		if(String.IsNullOrWhiteSpace(dbName)){ 
			return null;
		}

		return JsonSerializer.Deserialize<TItem2>(Context.DataRow[dbName].ToString());
	}
}