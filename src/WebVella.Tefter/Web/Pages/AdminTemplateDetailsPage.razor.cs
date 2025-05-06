namespace WebVella.Tefter.Web.Pages;
public partial class AdminTemplateDetailsPage : TfBasePage
{
	[Parameter] public int? ResultId { get; set; }
	[Parameter] public Guid? TemplateId { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if(ResultId is null)
			Navigator.NavigateTo(string.Format(TfConstants.AdminTemplatesTypePageUrl,(int)TfTemplateResultType.File));
	}
}