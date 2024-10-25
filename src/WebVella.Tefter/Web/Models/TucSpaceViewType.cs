namespace WebVella.Tefter.Web.Models;

[TucEnumMatch(typeof(TfSpaceViewType))]
public enum TucSpaceViewType
{
	[Description("Datagrid")]
	DataGrid = 0,
	[Description("Form")]
	Form = 1,
	[Description("Chart")]
	Chart = 2,
	[Description("Dashboard")]
	Dashboard = 3
}
