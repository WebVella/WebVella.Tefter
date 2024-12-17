namespace WebVella.Tefter.Web.Pages;
public partial class AdminTemplatesPage : TfBasePage
{
	[Parameter] public int? ResultId { get; set; }
	[Parameter] public Guid? TemplateId { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if(ResultId is null)
			Navigator.NavigateTo(String.Format(TfConstants.AdminTemplatesResultPageUrl,(int)TfTemplateResultType.File));
	}
}