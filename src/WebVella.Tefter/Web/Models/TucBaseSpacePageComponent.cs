namespace WebVella.Tefter.Web.Models;

public abstract class TucBaseSpacePageComponent : ComponentBase, ITfSpacePageAddon
{
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	public virtual Guid Id { get; init; } = Guid.NewGuid();
	public virtual string Name { get; init; } = null;
	public virtual string Description { get; init; } = null;
	public virtual string FluentIconName { get; init; } = null;
	public virtual TfSpacePageAddonContext Context { get; set; }
	public virtual string GetOptions() => "{}";
	public virtual List<ValidationError> ValidateOptions() => new List<ValidationError>();
	public virtual List<ValidationError> ValidationErrors { get; set; } = new();
	public virtual Task<(TfAppState,TfAuxDataState)> InitState(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState, 
		TfSpacePageAddonContext context) => Task.FromResult((newAppState,newAuxDataState));
	public virtual Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context) => Task.FromResult(context.ComponentOptionsJson);
	public virtual Task<string> OnPageUpdated(IServiceProvider serviceProvider, TfSpacePageAddonContext context) => Task.FromResult(context.ComponentOptionsJson);
	public virtual Task OnPageDeleted(IServiceProvider serviceProvider, TfSpacePageAddonContext context) => Task.CompletedTask;
	

	#region << Private properties >>
	protected IStringLocalizer LC;
	#endregion

	#region << Lifecycle >>
	/// <summary>
	/// If overrided do not forget to call it
	/// </summary>
	/// <returns></returns>
	protected override Task OnInitializedAsync()
	{
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
		return Task.CompletedTask;
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

	protected string GetValidationCssClass(string propName)
	{
		return (ValidationErrors ?? new List<ValidationError>()).Any(x => x.PropertyName == propName) ? "invalid" : "";
	}

	#endregion
}