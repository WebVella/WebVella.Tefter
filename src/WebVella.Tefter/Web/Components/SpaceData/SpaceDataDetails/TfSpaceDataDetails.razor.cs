namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceDataDetails : TfBaseComponent
{
	[Parameter] public string Menu { get; set; } = "";
	[Inject] protected IState<TfState> TfState { get; set; }
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

}