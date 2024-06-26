using Fluxor;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Globalization;
using WebVella.Tefter;
using WebVella.Tefter.Identity;
using WebVella.Tefter.Site.Components;
using WebVella.Tefter.Web.Utils;

var builder = WebApplication.CreateBuilder(args);

//Enable Blazor Interactive Server
//NOTE: Currently tested and developed only in InteractiveServer mode
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

//Add Fluxor State Managements
//NOTE: Register your assemblies if you need states
builder.Services.AddFluxor(options =>
{
	options.ScanAssemblies(typeof(IIdentityManager).Assembly);
});

//IMPORTANT: Do not remove. Required for the application to work
builder.Services.AddTefter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

//NOTE: if you want to have routable components in your own addon
//You need to register your Assembly here to be scanned
//IMPORTANT: to not forget to add it in the <Route> AdditionalAssemblies 
//parameter also
app.MapRazorComponents<App>()
	.AddAdditionalAssemblies(new[] { typeof(IIdentityManager).Assembly })
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

