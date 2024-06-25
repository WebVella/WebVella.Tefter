using Fluxor;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Globalization;
using WebVella.Tefter;
using WebVella.Tefter.Identity;
using WebVella.Tefter.Site.Components;
using WebVella.Tefter.Web.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddLocalization();
builder.Services.AddFluxor(options =>
{
	options.ScanAssemblies(typeof(IIdentityManager).Assembly);
#if DEBUG
	//options.UseReduxDevTools();
#endif
});

//because components are not disposed for about 10 min after page is left by browser
//maybe 5 seconds is too low ???
//https://stackoverflow.com/questions/78451698/dispose-is-never-called-for-any-server-side-blazor-components
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
	options.DetailedErrors = true;
	options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(5);
});

builder.Services.AddFluentUIComponents();
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
app.MapRazorComponents<App>()
	.AddAdditionalAssemblies(new[] { typeof(IIdentityManager).Assembly })
	.AddInteractiveServerRenderMode();

app.Services.UseTefter();

string[] supportedCultures = TfConstants.CultureOptions.Select(x => x.CultureCode).ToArray();
var localizationOptions = new RequestLocalizationOptions()
	.SetDefaultCulture(supportedCultures[0])
	.AddSupportedCultures(supportedCultures)
	.AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(supportedCultures[0]);
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(supportedCultures[0]);

app.Run();

