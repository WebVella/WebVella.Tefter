namespace WebVella.Tefter.UI.Components;

public partial class TucTreeViewItem : ComponentBase
{
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Parameter] public TfMenuItem Item { get; set; } = null!;

	private string _cssClass
	{
		get
		{
			var classList = new List<string>();
			classList.Add("tf-treemenu__item");
			if (Item.Data is not null)
				classList.Add($"tf-treemenu__item--{Item.Data.SpacePageType.ToDescriptionString()}");

			if (Item.OnClick.HasDelegate) classList.Add("tf-clickable");
			if (Item.OnExpand.HasDelegate) classList.Add("tf-expandable");
			if (Item.Expanded) classList.Add("tf-expanded");
			if (Item.Selected) classList.Add("tf-active");
			if (Item.ChildSelected) classList.Add("tf-child-active");
			if (Item.Items.Count > 0) classList.Add("tf-parent");

			return String.Join(" ", classList);
		}
	}

	private async Task _onClick()
	{
		if (!String.IsNullOrWhiteSpace(Item.Url))
		{
			Navigator.NavigateTo(Item.Url);
			return;
		}

		if (Item.OnClick.HasDelegate)
		{
			await Item.OnClick.InvokeAsync();
			return;
		}
		if(Item.Items.Count > 0) 
			await _onExpand();

	}

	private async Task _onExpand()
	{
		Item.Expanded = !Item.Expanded;
		await Item.OnExpand.InvokeAsync(Item.Expanded);
		await InvokeAsync(StateHasChanged);
	}
}