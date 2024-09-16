using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.UseCases.AppStart;

internal partial class StateInitUseCase
{
	internal async Task<TucCultureOption> InitCulture(TucUser user)
	{
		var cultureCookie = await new CookieService(_jsRuntime).GetAsync(CookieRequestCultureProvider.DefaultCookieName);
		CultureInfo cookieCultureInfo = null;
		ProviderCultureResult cultureCookieValue = null;
		if (cultureCookie is not null)
			cultureCookieValue = CookieRequestCultureProvider.ParseCookieValue(cultureCookie.Value);
		if (cultureCookieValue != null && cultureCookieValue.UICultures.Count > 0)
		{
			try
			{
				var cookieCulture = CultureInfo.GetCultureInfo(cultureCookieValue.UICultures.First().ToString());
				if (TfConstants.CultureOptions.Any(x => x.CultureInfo.Name == cookieCulture.Name))
				{
					cookieCultureInfo = cookieCulture;
				}
			}
			//in case there is unrecognized culture in the cookie
			catch { }
		}

		var userCultureInfo = user is null || user.Settings is null || String.IsNullOrWhiteSpace(user.Settings.CultureName)
						? TfConstants.CultureOptions[0].CultureInfo
						: CultureInfo.GetCultureInfo(user.Settings.CultureName);

		if (cookieCultureInfo is null || cookieCultureInfo.Name != userCultureInfo.Name)
		{
			CultureInfo.CurrentCulture = userCultureInfo;
			CultureInfo.CurrentUICulture = userCultureInfo;

			await new CookieService(_jsRuntime).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
					CookieRequestCultureProvider.MakeCookieValue(
						new RequestCulture(
							userCultureInfo,
							userCultureInfo)), DateTimeOffset.Now.AddYears(30));

			//_navigationManager.ReloadCurrentUrl();
			return null;
		}
		else
		{
			var cultureOption = TfConstants.CultureOptions.FirstOrDefault(x => x.CultureCode == userCultureInfo.Name);
			if (cultureOption is null) cultureOption = TfConstants.CultureOptions[0];
			return cultureOption;
		}
	}
}
