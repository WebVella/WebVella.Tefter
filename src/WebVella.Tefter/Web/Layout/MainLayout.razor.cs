namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : LayoutComponentBase
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

	protected override void OnAfterRender(bool firstRender)
	{
		Console.WriteLine("============ MainLayout RERENDER");
		base.OnAfterRender(firstRender);
	}
}