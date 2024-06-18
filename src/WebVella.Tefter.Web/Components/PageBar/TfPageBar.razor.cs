namespace WebVella.Tefter.Web.Components;
public partial class TfPageBar : TfBaseComponent
{
	[Parameter]
	public List<MenuItem> Items { get; set; } = new();

	[Inject] protected IState<SessionState> SessionState { get; set; }

	[Parameter] public string Style { get; set; }
}