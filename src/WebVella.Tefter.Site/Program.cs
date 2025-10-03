#region <--- USING DIRECTIVES --->
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.IdentityModel.Abstractions;
using Serilog;
using Serilog.Extensions.Logging;
using System.Globalization;
using System.Text;
using WebVella.BlazorTrace;
using WebVella.Tefter;
using WebVella.Tefter.Database;
using WebVella.Tefter.Services;
using WebVella.Tefter.Utility;

#endregion
var configBuilder = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true);

var configuration = configBuilder.Build();

Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();

try
{
	Log.Information("TEFTER: Starting up");

	var builder = WebApplication.CreateBuilder(args);
	{
		builder.Configuration
		 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		 .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true);

		var config = new ConfigurationBuilder()
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				 .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
					.Build();

		builder.Host.UseSerilog(Log.Logger);

		//builder.Host.ConfigureLogging(logging =>
		//      {
		//          logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
		//          logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
		//      });
		//Enable Blazor Interactive Server
		//NOTE: Currently tested and developed only in InteractiveServer mode

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

		builder.Services.AddControllers()
		.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ITfService).Assembly));

		//Blazor Trace Core Service
		builder.Services.AddBlazorTrace(new WvBlazorTraceConfiguration(){ EnableTracing = false});

		//IMPORTANT: Do not remove. Required for the application to work
		builder.Services.AddTefter(config:config);
	}

	var app = builder.Build();
	{
		app.UseSerilogRequestLogging();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error", createScopeForErrors: true);
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.MapStaticAssets();
		app.UseAntiforgery();
		app.MapControllers();
		//NOTE: if you want to have routable components in your own addon
		//You need to register your Assembly here to be scanned
		//IMPORTANT: to not forget to add it in the <Route> AdditionalAssemblies 
		//parameter also

		app.MapRazorComponents<WebVella.Tefter.UI.Components.App>()
			.AddInteractiveServerRenderMode();
		//This setups the localization of the Application. 
		//It currently support only a selection of languages/cultures. 
		//Contact us if you want to help with the translation in other languages
		string[] supportedCultures = TfConstants.CultureOptions.Select(x => x.CultureCode).ToArray();
		var localizationOptions = new RequestLocalizationOptions()
			.SetDefaultCulture(supportedCultures[0])
			.AddSupportedCultures(supportedCultures)
			.AddSupportedUICultures(supportedCultures);
		app.UseRequestLocalization(localizationOptions);
		CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(supportedCultures[0]);
		CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(supportedCultures[0]);

		//IMPORTANT: Do not remove. Required for the application to work
		app.UseTefter();
		await app.RunAsync();
	}
}
catch (Exception ex)
{
	Log.Fatal(ex, "TEFTER: Application normal start-up failed");
	TfFailedModeHost.CreateAndRun(ex, args);
}
finally
{
	Log.Information("TEFTER: Application is shutting down");
	Log.CloseAndFlush();
}

