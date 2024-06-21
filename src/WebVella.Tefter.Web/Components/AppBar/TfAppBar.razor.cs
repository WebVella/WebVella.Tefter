namespace WebVella.Tefter.Web.Components.AppBar;
public partial class TfAppBar : TfBaseComponent
{
	[Parameter]
	public List<MenuItem> Items { get; set; } = new();

	[Inject] protected IState<SessionState> SessionState { get; set; }

	[Parameter] public string Style { get; set; }
}