﻿using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using Moq;
using Nito.AsyncEx;
using System;
using System.Text;
using System.Text.Json;
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
using WebVella.Tefter.Web.Utils;


namespace WebVella.Tefter.Web.Tests;
public class BaseTest
{
	public Mock<ITfConfigurationService> TfConfigurationServiceMock;
	public Mock<AuthenticationStateProvider> AuthenticationStateProviderMock;
	public Mock<ILogger> LoggerMock;
	//messaging
	internal Mock<ITfChannelEventRouter> TfChannelEventRouterMock;
	public Mock<ITfEventBus> TfEventBusMock;
	public Mock<TfUserEventProvider> TfUserEventProviderMock;
	public Mock<TfGlobalEventProvider> TfGlobalEventProviderMock;

	public Mock<ITfCryptoService> TfCryptoServiceMock;
	public Mock<ITfCryptoServiceConfiguration> TfCryptoServiceConfigurationMock;
	public Mock<ITfTransactionRollbackNotifyService> TfTransactionRollbackNotifyServiceMock;
	public Mock<ITfDbConfigurationService> TfDbConfigurationServiceMock;
	public Mock<ITfDatabaseService> TfDatabaseServiceMock;
	public Mock<IDatabaseManager> DatabaseManagerMock;
	internal Mock<ITfDboManager> TfDboManagerMock;
	internal Mock<IMigrationManager> MigrationManagerMock;
	public Mock<IIdentityManager> IdentityManagerMock;
	public Mock<ITfDataManager> TfDataManagerMock;
	public Mock<ITfSharedColumnsManager> TfSharedColumnsManagerMock;
	public Mock<ITfDataProviderManager> TfDataProviderManagerMock;
	public Mock<ITfSpaceManager> TfSpaceManagerMock;
	public Mock<ITfMetaProvider> TfMetaProviderMock;
	public Mock<ITfRepositoryService> TfRepositoryServiceMock;
	public Mock<ITfBlobManager> TfBlobManagerMock;
	//localization
	public Mock<IStringLocalizerFactory> StringLocalizerFactoryMock;

	//use cases
	internal Mock<AppStateUseCase> AppStateUseCaseMock;
	internal Mock<UserStateUseCase> UserStateUseCaseMock;
	internal Mock<ExportUseCase> ExportUseCaseMock;
	internal Mock<LoginUseCase> LoginUseCaseMock;

	public IStore Store;
	public IDispatcher Dispatcher;
	public IState<TfUserState> TfUserState;
	public IState<TfAppState> TfAppState;


	public TestContext GetTestContext()
	{
		var Context = new TestContext();

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

		#region << Local Storage >>
		// This string will be the json of the object you want
		// to receive when GetAsync is called from your ProtectedLocalStorage.
		// In my case it was an instance of my UserSession class.
		string openedNodesList = JsonSerializer.Serialize(new List<string>());

		// Base64 used internally on the ProtectedSessionStorage.
		string base64UserSessionJson = Convert.ToBase64String(Encoding.ASCII.GetBytes(openedNodesList));

		// Notice how the mocked methods return the json.
		Mock<IDataProtector> mockDataProtector = new Mock<IDataProtector>();
		_ = mockDataProtector.Setup(sut => sut.Protect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes(base64UserSessionJson));
		_ = mockDataProtector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes(openedNodesList));

		Mock<IDataProtectionProvider> mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
		_ = mockDataProtectionProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockDataProtector.Object);

		// This is where the JSInterop by bunit is used.
		// localStorage might need to be changed to your variable name or
		// purpose? If you execute the test without this setup
		// an exception will tell you the name you need to use.
		_ = Context.JSInterop.Setup<string>("localStorage.getItem", TfConstants.SpaceViewOpenedGroupsLocalStorageKey).SetResult(base64UserSessionJson);

		// Use this instance on your constructor or context.
		ProtectedLocalStorage protectedSessionStorage = new ProtectedLocalStorage(
			Context.JSInterop.JSRuntime,
			mockDataProtectionProvider.Object);

		Context.Services.AddScoped(typeof(ProtectedLocalStorage), Services => protectedSessionStorage);
		#endregion


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

		Store = Context.Services.GetRequiredService<IStore>();
		Store.InitializeAsync().Wait();
		Dispatcher = Context.Services.GetRequiredService<IDispatcher>();
		return Context;
	}
}
