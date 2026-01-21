namespace WebVella.Tefter.Models;

public enum TfTemplateResultType
{
	[Description("File")]
	[FluentIcon("Document")]
	File = 0,
	[Description("Email")]
	[FluentIcon("Mail")]
	Email = 1,
	[Description("Text")]
	[FluentIcon("ScanText")]
	Text = 2,
	[Description("File Group")]
	[FluentIcon("Folder")]
	FileGroup = 3,	
}
