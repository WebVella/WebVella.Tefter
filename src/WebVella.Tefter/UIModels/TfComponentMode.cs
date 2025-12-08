namespace WebVella.Tefter.Models;

public enum TfComponentMode
{
	[Description("Read")]
	Read = 0,
	[Description("Create")]
	Create = 1,
	[Description("Update")]
	Update = 2,
	[Description("Manage")]
	Manage = 3,
	[Description("QuickView")]
	QuickView = 4		
}
