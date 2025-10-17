
using Nito.AsyncEx;

namespace WebVella.Tefter.Models;

public class TfLocalizedViewColumnComponent : ComponentBase
{
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; } = null!;

	private IStringLocalizer LC  = null!;

	protected override void OnInitialized()
	{
		LC = StringLocalizerFactory.Create(this.GetType());
	}
	protected string LOC(string key, params object[] arguments)
	{
		if (LC[key, arguments] != key) return LC[key, arguments];
		return key;
	}

}
