namespace WebVella.Tefter.Web.Components;
public partial class TfAppBar : TfBaseComponent
{
	[Parameter] public List<TucMenuItem> Items { get; set; } = new();
	[Parameter] public string Style { get; set; }

	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

}