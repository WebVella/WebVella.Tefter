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
			classList.Add("tf-treemenu__item");
			if (Item.Data is not null)
				classList.Add($"tf-treemenu__item--{Item.Data.SpacePageType.ToDescriptionString()}");

			if (Item.OnClick is not null) classList.Add("tf-clickable");
			if (Item.OnExpand is not null) classList.Add("tf-expandable");
			if (Item.Expanded) classList.Add("tf-expanded");
			if (Item.Selected) classList.Add("tf-active");
			if (Item.Items.Count > 0) classList.Add("tf-parent");

			return String.Join(" ", classList);
		}
	}

	private void _onClick()
	{
		if (Item.OnClick != null)
		{
			Item.OnClick();
			return;
		}
		if (!String.IsNullOrWhiteSpace(Item.Url))
			Navigator.NavigateTo(Item.Url);
	}
}