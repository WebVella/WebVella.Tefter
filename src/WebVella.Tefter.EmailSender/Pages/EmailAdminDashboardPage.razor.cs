namespace WebVella.Tefter.EmailSender.Pages;

public partial class EmailAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
    public Guid Id { get { return new Guid("1f6e544e-6a53-4fa1-98ef-9c51a569c2b5"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 1000; } }
	public string Name { get { return "Email Sender"; } }
	public string FluentIconName => "Mail";

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var mailService = serviceProvider.GetRequiredService<IEmailService>();
       

        return Task.CompletedTask;
    }
}