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
using WebVella.Tefter.Authentication;
using WebVella.Tefter.Messaging;
using WebVella.Tefter.Migrations;
using WebVella.Tefter.Utility;
using WebVella.Tefter.Services;
using WebVella.BlazorTrace;
using System.Threading;


namespace WebVella.Tefter.UI.Tests;
public class BaseTest
{
	protected static readonly AsyncLock locker = new AsyncLock();
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
	public Mock<ITfDatabaseManager> DatabaseManagerMock;
	internal Mock<ITfDboManager> TfDboManagerMock;
	internal Mock<IMigrationManager> MigrationManagerMock;
	public Mock<ITfService> TfServiceMock;
	public Mock<ITfMetaService> TfMetaServiceMock;
	public Mock<IWvBlazorTraceService> WvBlazorTraceServiceMock;
	//localization
	public Mock<IStringLocalizerFactory> StringLocalizerFactoryMock;

	//use cases
	
	public Dictionary<string, object> ViewData = new Dictionary<string, object>();
	
	public TestContext GetTestContext()
	{
		var Context = new TestContext();

		#region << Fluent >>
		//Context.JSInterop.SetupModule("./_content/Microsoft.FluentUI.AspNetCore.Components/Components/Label/FluentInputLabel.razor.js");
		Context.JSInterop.Mode = JSRuntimeMode.Loose;

		Context.Services.AddFluentUIComponents();
		Context.Services.AddScoped<IToastService, ToastService>();
		Context.Services.AddScoped<IDialogService, DialogService>();
		Context.Services.AddScoped<IMessageService, MessageService>();
		Context.Services.AddBlazorTrace(new WvBlazorTraceConfiguration(){ EnableTracing = false});
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

		TfUserEventProviderMock = new Mock<TfUserEventProvider>(TfEventBusMock.Object, AuthenticationStateProviderMock.Object);
		Context.Services.AddTransient(typeof(TfUserEventProvider), Services => TfUserEventProviderMock.Object);

		TfGlobalEventProviderMock = new Mock<TfGlobalEventProvider>(TfEventBusMock.Object);
		Context.Services.AddSingleton(typeof(TfGlobalEventProvider), Services => TfGlobalEventProviderMock.Object);
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

		DatabaseManagerMock = new Mock<ITfDatabaseManager>();
		Context.Services.AddSingleton(typeof(ITfDatabaseManager), Services => DatabaseManagerMock.Object);

		TfDboManagerMock = new Mock<ITfDboManager>();
		Context.Services.AddSingleton(typeof(ITfDboManager), Services => TfDboManagerMock.Object);

		MigrationManagerMock = new Mock<IMigrationManager>();
		Context.Services.AddSingleton(typeof(IMigrationManager), Services => MigrationManagerMock.Object);

		TfServiceMock = new Mock<ITfService>();
		Context.Services.AddSingleton(typeof(ITfService), Services => TfServiceMock.Object);

		TfMetaServiceMock = new Mock<ITfMetaService>();
		Context.Services.AddSingleton(typeof(ITfMetaService), Services => TfMetaServiceMock.Object);

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

		return Context;
	}
}
