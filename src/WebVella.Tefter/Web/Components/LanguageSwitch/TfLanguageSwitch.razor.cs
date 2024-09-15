

using Microsoft.AspNetCore.Localization;

namespace WebVella.Tefter.Web.Components;
public partial class TfLanguageSwitch : TfBaseComponent
{
	[Inject] protected IState<TfState> TfState { get; set; }
	[Inject] protected IStateSelection<TfState, Guid> UserIdState { get; set; }

	private bool _visible = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		UserIdState.Select(x => x?.CurrentUser?.Id ?? Guid.Empty);
	}

	private async Task _select(TucCultureOption option)
	{
		Dispatcher.Dispatch(new SetCultureAction(
		component:this,
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