namespace WebVella.Tefter.UI.Pages;
public partial class ErrorPage : ComponentBase
{
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
	private string? _errorMessage = null;

	protected override async Task OnInitializedAsync()
	{
		_errorMessage = await Navigator.GetErrorMessage(JsRuntime);
	}
}