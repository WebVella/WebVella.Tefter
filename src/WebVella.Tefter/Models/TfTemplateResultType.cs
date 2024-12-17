namespace WebVella.Tefter.Models;

public enum TfTemplateResultType
{
	[Description("Files")]
	[FluentIcon("Document")]
	File = 0,
	[Description("Emails")]
	[FluentIcon("Mail")]
	Email = 1,
	[Description("Texts")]
	[FluentIcon("ScanText")]
	Text = 2,
}
