
namespace WebVella.Tefter.Web.Models;

public abstract class TucBaseSpaceNodeComponent : ComponentBase, ITfSpaceNodeComponent
{
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	public virtual Guid Id { get; set; } = Guid.NewGuid();

	public virtual string Name { get; set; } = "";

	public virtual string Description { get; set; } = "";

	public virtual TfSpaceNodeComponentContext Context { get; set; }
	public virtual string GetOptions() => "{}";
	public virtual List<ValidationError> ValidateOptions() => new List<ValidationError>();
	public virtual List<ValidationError> ValidationErrors { get; set; } = new();
	public virtual Task OnNodeCreated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context) => Task.CompletedTask;
	public virtual Task OnNodeUpdated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context) => Task.CompletedTask;
	public virtual Task OnNodeDeleted(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context) => Task.CompletedTask;
	

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