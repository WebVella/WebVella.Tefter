using Microsoft.AspNetCore.Localization;
namespace WebVella.Tefter.UI.Components;

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

				var culture = CultureInfo.GetCultureInfo(option.CultureCode);
				await new CookieService(JSRuntime).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
						CookieRequestCultureProvider.MakeCookieValue(
							new RequestCulture(
								culture,
								culture)), DateTimeOffset.Now.AddYears(30));
				//Needs to reload the whole page so the Localizer can reinit from the cookie
				Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}