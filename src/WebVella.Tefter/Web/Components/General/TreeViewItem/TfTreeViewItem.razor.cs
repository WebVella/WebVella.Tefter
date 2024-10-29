namespace WebVella.Tefter.Web.Components;
public partial class TfTreeViewItem : ComponentBase
{
	[Inject] protected NavigationManager Navigator { get; set; }
	[Parameter] public TucMenuItem Item { get; set; } = null;

	private string _cssClass
	{
		get
		{
			var classList = new List<string>();
			classList.Add("tf-menu-item");
			if(Item.OnClick is not null) classList.Add("tf-clickable");
			if(Item.OnExpand is not null) classList.Add("tf-expandable");
			if(Item.Expanded) classList.Add("tf-expanded");
			if(Item.Selected) classList.Add("tf-selected");
			if(Item.Nodes.Count > 0) classList.Add("tf-parent");

			return String.Join(" ", classList);
		}
	}

	private void _onClick(){ 
		if(Item.OnClick != null) {
			Item.OnClick();
			return;
		}
		if(!String.IsNullOrWhiteSpace(Item.Url))
			Navigator.NavigateTo(Item.Url);
	}
}