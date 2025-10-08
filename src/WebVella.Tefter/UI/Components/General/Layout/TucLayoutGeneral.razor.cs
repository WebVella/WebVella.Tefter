namespace WebVella.Tefter.UI.Components;

public partial class TucLayoutGeneral : TfBaseComponent
{
	[Parameter] public RenderFragment? SpaceHeader { get; set; }	
	[Parameter] public RenderFragment? SpaceActions { get; set; }	
	[Parameter] public RenderFragment? SideNav { get; set; }
	[Parameter] public RenderFragment? PageTopbar { get; set; }
	[Parameter] public RenderFragment? PageHeader { get; set; }
	[Parameter] public RenderFragment? PageTabs { get; set; }	
	[Parameter] public RenderFragment? ChildContent { get; set; }	
	[Parameter] public string Style { get; set; }

	private string _cssClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tflg ");
			if(PageTabs is not null)
				sb.Append("tflg--has-page-tabs ");
			if(_displayReturn)
				sb.Append("tflg--has-return-btn ");
			
			return sb.ToString();
		}
	}
	
	private bool _displayReturn => !String.IsNullOrWhiteSpace(TfState.NavigationState.ReturnUrl);
}