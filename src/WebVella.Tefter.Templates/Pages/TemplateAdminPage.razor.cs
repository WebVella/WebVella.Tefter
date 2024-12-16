namespace WebVella.Tefter.Templates.Pages;

public partial class TemplateAdminPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
    public Guid Id { get { return new Guid("23a8eccf-37b6-41e9-a33a-20317a6c00ac"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 1; } }
	public string Name { get { return "Templates"; } }
	public string FluentIconName { get { return "CalendarTemplate"; } }

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var service = serviceProvider.GetRequiredService<ITemplatesService>();        
        var processorsResult = service.GetProcessors();
        if(processorsResult.IsSuccess) 
            newAuxDataState.Data[TemplatesConstants.TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY] = processorsResult.Value.OrderBy(x=> x.Name).ToList();
        else 
            newAuxDataState.Data[TemplatesConstants.TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY] = new List<ITemplateProcessor>();

        var templatesResult = service.GetTemplates();
        if(templatesResult.IsSuccess) 
            newAuxDataState.Data[TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY] = templatesResult.Value.OrderBy(x=> x.Name).ToList();
        else
            newAuxDataState.Data[TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY] = new List<Template>();

        return Task.CompletedTask;
    }
}