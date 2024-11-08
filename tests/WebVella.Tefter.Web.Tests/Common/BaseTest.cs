using Fluxor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;
using System;
using WebVella.Tefter.Identity;
using WebVella.Tefter.Web.Store;


namespace WebVella.Tefter.Web.Tests.Common;
public class BaseTest
{
	public static IServiceProvider ServiceProvider;
	public static IStore Store;
	public static IState<TfAppState> TfAppState;
	public static Mock<IKeyCodeService> KeyCodeServiceMock;
	public static Mock<IIdentityManager> IdentityManagerMock;
	public static Mock<ITfDataProviderManager> TfDataProviderManagerMock;
	public static Mock<IStringLocalizerFactory> StringLocalizerFactoryMock;
	public static TestContext Context { get; }

	static BaseTest()
	{
		Context = new TestContext();
		Context.Services.AddFluxor(x =>
					x.ScanAssemblies(typeof(IIdentityManager).Assembly));

		IdentityManagerMock = new Mock<IIdentityManager>();
		Context.Services.AddScoped(typeof(IIdentityManager), Services => IdentityManagerMock.Object);
		TfDataProviderManagerMock = new Mock<ITfDataProviderManager>();
		Context.Services.AddScoped(typeof(ITfDataProviderManager), Services => TfDataProviderManagerMock.Object);
		KeyCodeServiceMock = new Mock<IKeyCodeService>();
		Context.Services.AddScoped(typeof(IKeyCodeService), Services => KeyCodeServiceMock.Object);

		Context.Services.AddScoped<IToastService, ToastService>();
		Context.Services.AddScoped<IDialogService, DialogService>();
		Context.Services.AddScoped<IMessageService, MessageService>();
		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json".ToApplicationPath(), optional: false)
			.Build();
		Context.Services.AddSingleton<IConfiguration>(configuration);
		Context.Services.AddScoped<ITfConfigurationService, TfConfigurationService>();

		StringLocalizerFactoryMock = new Mock<IStringLocalizerFactory>();
		Context.Services.AddScoped(typeof(IStringLocalizerFactory), Services => StringLocalizerFactoryMock.Object);

		Context.JSInterop.SetupModule("./_content/Microsoft.FluentUI.AspNetCore.Components/Components/Label/FluentInputLabel.razor.js");
		Context.JSInterop.Mode = JSRuntimeMode.Loose;

		Context.Services.AddFluentUIComponents();

		ServiceProvider = Context.Services.BuildServiceProvider();

		Store = ServiceProvider.GetRequiredService<IStore>();
		Store.InitializeAsync().Wait();

		TfAppState = ServiceProvider.GetRequiredService<IState<TfAppState>>();
		//UserIdState = ServiceProvider.GetRequiredService<IStateSelection<UserState,Guid>>();
		//UserIdState.Select(x => x?.User?.Id ?? Guid.Empty);
		//ScreenSidebarState = ServiceProvider.GetRequiredService<IStateSelection<ScreenState, bool>>();
		//ScreenSidebarState.Select(x => x?.SidebarExpanded ?? true);

	}
}
