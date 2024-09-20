namespace WebVella.Tefter.Web.Components;
public partial class TfAppBar : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Parameter] public List<MenuItem> Items { get; set; } = new();
	[Parameter] public string Style { get; set; }

}