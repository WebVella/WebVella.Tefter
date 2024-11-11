using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;
using Nito.AsyncEx;
using System;
using WebVella.Tefter.Database;
using WebVella.Tefter.Database.Dbo;
using WebVella.Tefter.Identity;
using WebVella.Tefter.Messaging;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.UseCases.AppState;
using WebVella.Tefter.UseCases.Export;
using WebVella.Tefter.UseCases.Login;
using WebVella.Tefter.UseCases.UserState;
using WebVella.Tefter.Utility;
using WebVella.Tefter.Web.Store;


namespace WebVella.Tefter.Web.Tests;
public class BaseTest
{
	protected static readonly AsyncLock locker = new AsyncLock();
	public static Mock<ITfConfigurationService> TfConfigurationServiceMock;
	public static Mock<AuthenticationStateProvider> AuthenticationStateProviderMock;
	public static Mock<ILogger> LoggerMock;
	//messaging
	internal static Mock<ITfChannelEventRouter> TfChannelEventRouterMock;
	internal static Mock<ITfEventBus> TfEventBusMock;
	internal static Mock<TfUserEventProvider> TfUserEventProviderMock;
	internal static Mock<TfGlobalEventProvider> TfGlobalEventProviderMock;

	public static Mock<ITfCryptoService> TfCryptoServiceMock;
	public static Mock<ITfCryptoServiceConfiguration> TfCryptoServiceConfigurationMock;
	public static Mock<ITfTransactionRollbackNotifyService> TfTransactionRollbackNotifyServiceMock;
	public static Mock<ITfDbConfigurationService> TfDbConfigurationServiceMock;
	public static Mock<ITfDatabaseService> TfDatabaseServiceMock;
	public static Mock<IDatabaseManager> DatabaseManagerMock;
	internal static Mock<ITfDboManager> TfDboManagerMock;
	internal static Mock<IMigrationManager> MigrationManagerMock;
	public static Mock<IIdentityManager> IdentityManagerMock;
	public static Mock<ITfDataManager> TfDataManagerMock;
	public static Mock<ITfSharedColumnsManager> TfSharedColumnsManagerMock;
	public static Mock<ITfDataProviderManager> TfDataProviderManagerMock;
	public static Mock<ITfSpaceManager> TfSpaceManagerMock;
	public static Mock<ITfMetaProvider> TfMetaProviderMock;
	public static Mock<ITfRepositoryService> TfRepositoryServiceMock;
	public static Mock<ITfBlobManager> TfBlobManagerMock;

	//localization
	public static Mock<IStringLocalizerFactory> StringLocalizerFactoryMock;

	//use cases
	internal static Mock<AppStateUseCase> AppStateUseCaseMock;
	internal static Mock<UserStateUseCase> UserStateUseCaseMock;
	internal static Mock<ExportUseCase> ExportUseCaseMock;
	internal static Mock<LoginUseCase> LoginUseCaseMock;

	public static TestContext Context { get; }

	static BaseTest()
	{
		Context = new TestContext();
		#region << Fluent And Flux >>
		//Context.JSInterop.SetupModule("./_content/Microsoft.FluentUI.AspNetCore.Components/Components/Label/FluentInputLabel.razor.js");
		Context.JSInterop.Mode = JSRuntimeMode.Loose;

		Context.Services.AddFluentUIComponents();
		Context.Services.AddFluxor(x =>
					x.ScanAssemblies(typeof(IIdentityManager).Assembly));
		Context.Services.AddScoped<IToastService, ToastService>();
		Context.Services.AddScoped<IDialogService, DialogService>();
		Context.Services.AddScoped<IMessageService, MessageService>();
		#endregion

		AuthenticationStateProviderMock = new Mock<AuthenticationStateProvider>();
		Context.Services.AddScoped(typeof(AuthenticationStateProvider), Services => AuthenticationStateProviderMock.Object);

		LoggerMock = new Mock<ILogger>();
		Context.Services.AddSingleton(typeof(ILogger), Services => LoggerMock.Object);

		TfConfigurationServiceMock = new Mock<ITfConfigurationService>();
		Context.Services.AddSingleton(typeof(ITfConfigurationService), Services => TfConfigurationServiceMock.Object);

		#region << Messaging >>
		TfChannelEventRouterMock = new Mock<ITfChannelEventRouter>();
		Context.Services.AddSingleton(typeof(ITfChannelEventRouter), Services => TfChannelEventRouterMock.Object);

		TfEventBusMock = new Mock<ITfEventBus>();
		Context.Services.AddTransient(typeof(ITfEventBus), Services => TfEventBusMock.Object);

		TfUserEventProviderMock = new Mock<TfUserEventProvider>();
		Context.Services.AddTransient(typeof(TfUserEventProvider), Services => TfUserEventProviderMock.Object);

		TfGlobalEventProviderMock = new Mock<TfGlobalEventProvider>();
		Context.Services.AddTransient(typeof(TfGlobalEventProvider), Services => TfGlobalEventProviderMock.Object);
		#endregion

		TfCryptoServiceMock = new Mock<ITfCryptoService>();
		Context.Services.AddSingleton(typeof(ITfCryptoService), Services => TfCryptoServiceMock.Object);

		TfCryptoServiceConfigurationMock = new Mock<ITfCryptoServiceConfiguration>();
		Context.Services.AddSingleton(typeof(ITfCryptoServiceConfiguration), Services => TfCryptoServiceConfigurationMock.Object);

		TfTransactionRollbackNotifyServiceMock = new Mock<ITfTransactionRollbackNotifyService>();
		Context.Services.AddSingleton(typeof(ITfTransactionRollbackNotifyService), Services => TfTransactionRollbackNotifyServiceMock.Object);

		TfDbConfigurationServiceMock = new Mock<ITfDbConfigurationService>();
		Context.Services.AddSingleton(typeof(ITfDbConfigurationService), Services => TfDbConfigurationServiceMock.Object);

		TfDatabaseServiceMock = new Mock<ITfDatabaseService>();
		Context.Services.AddSingleton(typeof(ITfDatabaseService), Services => TfDatabaseServiceMock.Object);

		DatabaseManagerMock = new Mock<IDatabaseManager>();
		Context.Services.AddSingleton(typeof(IDatabaseManager), Services => DatabaseManagerMock.Object);

		TfDboManagerMock = new Mock<ITfDboManager>();
		Context.Services.AddSingleton(typeof(ITfDboManager), Services => TfDboManagerMock.Object);

		MigrationManagerMock = new Mock<IMigrationManager>();
		Context.Services.AddSingleton(typeof(IMigrationManager), Services => MigrationManagerMock.Object);

		IdentityManagerMock = new Mock<IIdentityManager>();
		Context.Services.AddSingleton(typeof(IIdentityManager), Services => IdentityManagerMock.Object);

		TfDataManagerMock = new Mock<ITfDataManager>();
		Context.Services.AddSingleton(typeof(ITfDataManager), Services => TfDataManagerMock.Object);

		TfSharedColumnsManagerMock = new Mock<ITfSharedColumnsManager>();
		Context.Services.AddSingleton(typeof(ITfSharedColumnsManager), Services => TfSharedColumnsManagerMock.Object);

		TfDataProviderManagerMock = new Mock<ITfDataProviderManager>();
		Context.Services.AddSingleton(typeof(ITfDataProviderManager), Services => TfDataProviderManagerMock.Object);

		TfSpaceManagerMock = new Mock<ITfSpaceManager>();
		Context.Services.AddSingleton(typeof(ITfSpaceManager), Services => TfSpaceManagerMock.Object);

		TfMetaProviderMock = new Mock<ITfMetaProvider>();
		Context.Services.AddSingleton(typeof(ITfMetaProvider), Services => TfMetaProviderMock.Object);

		TfRepositoryServiceMock = new Mock<ITfRepositoryService>();
		Context.Services.AddSingleton(typeof(ITfRepositoryService), Services => TfRepositoryServiceMock.Object);

		TfBlobManagerMock = new Mock<ITfBlobManager>();
		Context.Services.AddSingleton(typeof(ITfBlobManager), Services => TfBlobManagerMock.Object);

		//localization
		StringLocalizerFactoryMock = new Mock<IStringLocalizerFactory>();
		Context.Services.AddSingleton(typeof(IStringLocalizerFactory), Services => StringLocalizerFactoryMock.Object);

		//use cases
		AppStateUseCaseMock = new Mock<AppStateUseCase>(Context.Services);
		Context.Services.AddTransient(typeof(AppStateUseCase), Services => AppStateUseCaseMock.Object);

		UserStateUseCaseMock = new Mock<UserStateUseCase>(Context.Services);
		Context.Services.AddTransient(typeof(UserStateUseCase), Services => UserStateUseCaseMock.Object);

		ExportUseCaseMock = new Mock<ExportUseCase>(Context.Services);
		Context.Services.AddTransient(typeof(ExportUseCase), Services => ExportUseCaseMock.Object);

		LoginUseCaseMock = new Mock<LoginUseCase>(Context.Services);
		Context.Services.AddTransient(typeof(LoginUseCase), Services => LoginUseCaseMock.Object);


	}
}
