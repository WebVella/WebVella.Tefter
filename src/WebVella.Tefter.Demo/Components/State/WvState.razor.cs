namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }

	[Inject] protected NavigationManager Navigator { get; set; }

	public Guid ComponentId { get; set; } = Guid.NewGuid();

	public CultureInfo Culture { get; set; } = new CultureInfo("en-US");

	private string _errorMessage = "";
	private bool _isLoading = true;

	private List<Space> _spaces = new();

	//LC
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			_spaces = SpaceData.GetSpaces();
			_isLoading = false;
			StateHasChanged();
		}
	}
	//Pulbic
	public List<Space> GetSpaces() => _spaces;

}
