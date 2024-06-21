using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Globalization;

namespace WebVella.Tefter.Identity;

public class IdentityCircuitHandler : CircuitHandler
{
	private AuthenticationStateProvider authStateProvider = null;

	private Dictionary<Circuit, Tuple<IDisposable, IDisposable>> contexts = new Dictionary<Circuit, Tuple<IDisposable, IDisposable>>();

	public IdentityCircuitHandler(AuthenticationStateProvider authStateProvider)
	{
		this.authStateProvider = authStateProvider;
	}

	public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
	{

		var authState = await authStateProvider.GetAuthenticationStateAsync();
		if(authState != null && authState.User != null && authState.User.Identity != null && 
			authState.User.Identity.IsAuthenticated )
		{
			var user = ((TfIdentity)authState.User.Identity).User;
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(user.Settings.CultureCode);
			CultureInfo.CurrentCulture = cultureInfo;
			CultureInfo.CurrentUICulture = cultureInfo;
		}
	}


	public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
	{
		var authState = await authStateProvider.GetAuthenticationStateAsync();
		if (authState != null && authState.User != null && authState.User.Identity != null &&
			authState.User.Identity.IsAuthenticated)
		{
			var user = ((TfIdentity)authState.User.Identity).User;
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(user.Settings.CultureCode);
			CultureInfo.CurrentCulture = cultureInfo;
			CultureInfo.CurrentUICulture = cultureInfo;
		}
		
		await base.OnCircuitOpenedAsync(circuit, cancellationToken);
	}
	public int ConnectedCircuits => contexts.Count;
}
