namespace WebVella.Tefter.UI.Components;

public partial class TucLayoutGeneral : TfBaseComponent
{
	[Parameter] public RenderFragment? SpaceHeader { get; set; }	
	[Parameter] public RenderFragment? SpaceActions { get; set; }	
	[Parameter] public RenderFragment? SideNav { get; set; }
	[Parameter] public RenderFragment? ContentTopbar { get; set; }
	[Parameter] public RenderFragment? ContentHeader { get; set; }
	[Parameter] public RenderFragment? ContentTabs { get; set; }	
	[Parameter] public RenderFragment? ChildContent { get; set; }	
	[Parameter] public string Style { get; set; }	

}