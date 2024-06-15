namespace WebVella.Tefter.Web.Components;
public partial class TfLoadMore : ComponentBase
{
	[Parameter]
	public bool Visible { get; set; } = false;
	[Parameter]
	public bool Loading { get; set; } = false;

	[Parameter]
	public EventCallback OnClick { get; set; }

	[Parameter]
	public string Style { get; set; }

	[Parameter]
	public string Class { get; set; }

	private async Task loadMore(){ 
		await OnClick.InvokeAsync();
	}
}