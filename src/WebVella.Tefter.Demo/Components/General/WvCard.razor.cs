namespace WebVella.Tefter.Demo.Components;
public partial class WvCard : WvBaseComponent
{
	[Parameter] public string Title { get; set; } = null;
	[Parameter] public string Description { get; set; } = null;
	[Parameter] public Icon Icon { get; set; } = null;
	[Parameter] public EventCallback OnClick { get; set; }

	private string _cssClass
	{
		get
		{
			var classList = new List<string>();
			classList.Add("wv-card");
			if(OnClick.HasDelegate) classList.Add("clickable");
			if(Icon is not null) classList.Add("with-icon");

			return String.Join(" ", classList);
		}
	}

	private async Task _onClick(){ 
		await OnClick.InvokeAsync();
	}

}