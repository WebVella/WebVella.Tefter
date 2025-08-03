namespace WebVella.Tefter.UI.Components;
public partial class TucTreeViewItem : ComponentBase
{
	[Inject] protected NavigationManager Navigator { get; set; } = default!;
	[Parameter] public TfMenuItem Item { get; set; } = default!;

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
			if(Item.Items.Count > 0) classList.Add("tf-parent");

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