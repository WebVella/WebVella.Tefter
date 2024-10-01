
namespace WebVella.Tefter.Models;

public class TfBaseScreenRegionComponent : ComponentBase, IAsyncDisposable
{
	#region << Injects >>
	[Inject] protected IJSRuntime JSRuntime { get; set; }
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	[Parameter] public TfScreenRegionComponentContext Context { get; set; }
	#endregion

	#region << Properties >>
	protected IStringLocalizer LC;
	#endregion

	#region << Lifecycle >>
	/// <summary>
	/// If overrided do not forget to call it
	/// </summary>
	/// <returns></returns>
	public virtual ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
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
	#endregion
}