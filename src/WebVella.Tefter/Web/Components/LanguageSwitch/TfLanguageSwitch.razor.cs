

using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.Web.Components;
public partial class TfLanguageSwitch : TfBaseComponent
{
	[Inject] protected IState<CultureState> CultureState { get; set; }
	[Inject] protected IStateSelection<UserState, Guid> UserIdState { get; set; }

	private bool _visible = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		UserIdState.Select(x => x?.User?.Id ?? Guid.Empty);
	}

	private async Task _select(TucCultureOption option)
	{
		Dispatcher.Dispatch(new SetCultureAction(
		userId:UserIdState.Value,
		culture: option));
		var culture = CultureInfo.GetCultureInfo(option.CultureCode);

		await new CookieService(JSRuntime).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(
					new RequestCulture(
						culture,
						culture)), DateTimeOffset.Now.AddYears(30));

		Navigator.ReloadCurrentUrl();
	}
}