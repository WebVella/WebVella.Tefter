using Microsoft.AspNetCore.Localization;
namespace WebVella.Tefter.UI.Components;

[LocalizationResource("WebVella.Tefter.UI.Components.User.LanguageSwitch.TucLanguageSwitch", "WebVella.Tefter")]
public partial class TucLanguageSwitch : TfBaseComponent
{
	[Inject] private ITfUserUIService TfUserUIService { get; set; } = default!;
	[Parameter] public TfUser User { get; set; } = default!;

	private bool _visible = false;


	private async Task _select(TfCultureOption option)
	{
		try
		{
			 await TfUserUIService.SetUserCulture(
				userId: User.Id,
				cultureCode: option.CultureCode);

				ToastService.ShowSuccess(LOC("The language is successfully changed!"));
				Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}