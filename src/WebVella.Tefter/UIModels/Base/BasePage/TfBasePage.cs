
using Nito.AsyncEx;

namespace WebVella.Tefter.Models;

public class TfBasePage : ComponentBase
{
	[Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationProvider { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Inject] protected IDialogService DialogService { get; set; } = null!;
	[Inject] protected IMessageService MessageService { get; set; } = null!;
	[Inject] protected ITfConfigurationService ConfigurationService { get; set; } = null!;
	[Inject] protected ITfService TfService { get; set; } = null!;
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; } = null!;
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

	protected IStringLocalizer LC  = null!;
	protected static IStringLocalizer? GL = null;
	private static AsyncLock _lock = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		LC = StringLocalizerFactory.Create(this.GetType());
		if (GL is null)
		{
			using (_lock.Lock())
			{
				GL = StringLocalizerFactory.Create(this.GetType().BaseType);
			}
		}
	}
	protected string LOC(string key, params object[] arguments)
	{
		if (LC[key, arguments] != key) return LC[key, arguments];
		if (GL[key, arguments] != key) return GL[key, arguments];
		return key;
	}

}
