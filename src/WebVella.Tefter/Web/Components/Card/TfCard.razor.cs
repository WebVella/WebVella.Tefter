namespace WebVella.Tefter.Web.Components.Card;
public partial class TfCard : TfBaseComponent
{
	[Parameter] public RenderFragment ChildContent { get; set; }

	[Parameter] public string Title { get; set; }
	[Parameter] public string Style { get; set; }

	[Parameter] public bool Small { get; set; } = false;

	private string Class
	{
		get
		{
			var classList = new List<string>();
			classList.Add("tf-card");
			if(Small) classList.Add("tf-card--small");

			return String.Join(" ", classList);
		}
	}
}