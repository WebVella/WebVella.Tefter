#region <--- USING DIRECTIVES --->
using Fluxor;
using Serilog;
using System.Globalization;
using WebVella.Tefter;
using WebVella.Tefter.Services;
using WebVella.Tefter.Web.Utils;
using WebVella.Tefter.Seeds.Site.Components;
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

		builder.Host.UseSerilog(Log.Logger);

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

		builder.Services.AddControllers();

		//Add Fluxor State Managements
		//NOTE: Register your assemblies if you need states
		builder.Services.AddFluxor(options =>
		{
			//options.UseRouting();
			options.ScanAssemblies(typeof(ITfService).Assembly);
			//options.AddMiddleware<LoggingMiddleware>();
			//options.UseReduxDevTools();
		});



		//IMPORTANT: Do not remove. Required for the application to work
		builder.Services.AddTefter();
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
		app.MapRazorComponents<App>()
			.AddAdditionalAssemblies(new[] { typeof(ITfService).Assembly })
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
		app.Services.UseTefter();
		app.Run();
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

