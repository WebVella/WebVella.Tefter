namespace WebVella.Tefter.Models;

public class TfBaseViewColumn : ComponentBase
{
	[Parameter]
	public TfComponentContext Context { get; set; }

	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }

	protected IStringLocalizer LC;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		var type = this.GetType();
		var (resourceBaseName,resourceLocation) = type.GetLocalizationResourceInfo();
		if(!String.IsNullOrWhiteSpace(resourceBaseName) && !String.IsNullOrWhiteSpace(resourceLocation)) {
			LC = StringLocalizerFactory.Create(resourceBaseName,resourceLocation);
		}
		else{ 
			LC = StringLocalizerFactory.Create(type);
		}
		
	}

	protected string LOC(string key, params object[] arguments)
	{
		if (LC is not null && LC[key, arguments] != key) return LC[key, arguments];
		return key;
	}

}