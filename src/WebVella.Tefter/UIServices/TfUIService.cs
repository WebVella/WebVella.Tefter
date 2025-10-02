
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
}
public partial class TfUIService : ITfUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private static readonly Lock _lockObject = new();
	private readonly IJSRuntime _jsRuntime;
	private readonly AuthenticationStateProvider _authStateProvider;
	private readonly IServiceProvider _serviceProvider;
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfUIService> LOC;
	private readonly TfGlobalEventProvider _eventProvider;

	public TfUIService(IServiceProvider serviceProvider,
		IJSRuntime jsRuntime,
		ITfService tfService,
		ITfMetaService metaService,
		NavigationManager navigationManager,
		AuthenticationStateProvider authStateProvider,
		TfGlobalEventProvider globalEventProvider)
	{
		_jsRuntime = jsRuntime;
		_tfService = tfService;
		_metaService = metaService;
		_authStateProvider = authStateProvider;
		_serviceProvider = serviceProvider;
		_eventProvider = globalEventProvider;
		LOC = serviceProvider.GetService<IStringLocalizer<TfUIService>>() ?? null!;
	}
	#endregion
}
