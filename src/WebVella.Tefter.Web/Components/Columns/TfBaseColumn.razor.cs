namespace WebVella.Tefter.Web.Components;
public partial class TfBaseColumn : ComponentBase
{
	[Parameter]
	public SpaceViewColumn Meta { get; set; }

	[Parameter]
	public DataRow Data { get; set; }

	[Parameter]
	public Action<(string,object)> ValueChanged { get; set; }

	
}