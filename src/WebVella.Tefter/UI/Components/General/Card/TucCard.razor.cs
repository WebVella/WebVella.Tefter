namespace WebVella.Tefter.UI.Components;
public partial class TucCard : TfBaseComponent
{
	[Parameter] public RenderFragment? ChildContent { get; set; }

	[Parameter] public string? Title { get; set; }
	[Parameter] public string? SubTitle { get; set; }
	[Parameter] public RenderFragment? TitleTemplate { get; set; }
	[Parameter] public RenderFragment? SubTitleTemplate { get; set; }
	[Parameter] public RenderFragment? ToolbarTemplate { get; set; }
	[Parameter] public RenderFragment? FooterTemplate { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public string? TitleStyle { get; set; }
	[Parameter] public string? Id { get; set; }
	[Parameter] public bool Small { get; set; } = false;
	[Parameter] public string? Class { get; set; }
	[Parameter] public EventCallback OnClick { get; set; }

	private string _class
	{
		get
		{
			var classList = new List<string>(){"tf-card"};
			if(Small) classList.Add("tf-card--small");
			if(ChildContent is null) classList.Add("tf-card--no-body");
			if(OnClick.HasDelegate) classList.Add("tf-card--clickable");
			
			classList.Add(Class);
			return String.Join(" ", classList);
		}
	}
}