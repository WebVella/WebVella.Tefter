namespace WebVella.Tefter.UI.Pages;
public partial class AdminTemplateDetailsPage : TfBasePage
{
	[Parameter] public int? ResultId { get; set; }
	[Parameter] public Guid? TemplateId { get; set; }
}