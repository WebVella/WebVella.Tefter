namespace WebVella.Tefter.Email.Pages;

public partial class EmailAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 10; } }
	public string Name { get { return "Email Addon"; } }
	public string UrlSlug { get { return "mail-dashboard"; } }

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var mailService = serviceProvider.GetRequiredService<IEmailService>();
       

        return Task.CompletedTask;
    }
}