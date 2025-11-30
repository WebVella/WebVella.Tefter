namespace WebVella.Tefter.UI.Components;

public partial class TucLayoutGeneral : TfBaseComponent
{
	[Parameter] public RenderFragment? SpaceHeader { get; set; }
	[Parameter] public RenderFragment? SpaceActions { get; set; }
	[Parameter] public RenderFragment? SideNav { get; set; }
	[Parameter] public RenderFragment? AsideActions { get; set; }
	[Parameter] public RenderFragment? PageTopbar { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public string Style { get; set; }

	private bool _hasSidebar => SpaceHeader is not null 
	                            || SpaceActions is not null || SideNav is not null || AsideActions is not null;

	private string _cssClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tflg ");
			if (!_hasSidebar)
			{
				sb.Append("tflg--no-aside ");
			}
			if (AsideActions is null)
			{
				sb.Append("tflg--no-aside-actions ");
			}
			return sb.ToString();
		}
	}
}