using Microsoft.AspNetCore.Localization;
namespace WebVella.Tefter.Web.Components;

[LocalizationResource("WebVella.Tefter.Web.Components.General.LanguageSwitch.TfLanguageSwitch", "WebVella.Tefter")]
public partial class TfLanguageSwitch : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] private UserStateUseCase UC { get; set; }

	private bool _visible = false;

	private async Task _select(TucCultureOption option)
	{
		try
		{
			var resultSrv = await UC.SetUserCulture(
						userId: TfUserState.Value.CurrentUser.Id,
						cultureCode: option.CultureCode);

			ProcessServiceResponse(resultSrv);

			if (resultSrv.IsSuccess)
			{
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
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}