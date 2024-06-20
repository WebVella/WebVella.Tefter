using WebVella.Tefter.Web;
using System.Globalization;
using Microsoft.JSInterop;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddLocalization();
builder.Services.AddTefter();
builder.Services.AddFluxor(options =>
{
	options
	.ScanAssemblies(typeof(Program).Assembly);
});
builder.Services.AddFluentUIComponents();

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
	.AddInteractiveServerRenderMode();

app.Services.UseTefter();

string[] supportedCultures =TfConstants.CultureOptions.Select(x=> x.CultureCode).ToArray();
var localizationOptions = new RequestLocalizationOptions()
	.SetDefaultCulture(supportedCultures[0])
	.AddSupportedCultures(supportedCultures)
	.AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(supportedCultures[0]);
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(supportedCultures[0]);

app.Run();

