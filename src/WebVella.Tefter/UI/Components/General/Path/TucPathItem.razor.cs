namespace WebVella.Tefter.UI.Components;

public partial class TucPathItem : TfBaseComponent
{
	[Parameter] public TfMenuItem Item { get; set; } = null!;

	private string _css
	{
		get
		{
			var sb = new StringBuilder();
			if (Item.Selected)
				sb.Append("tf-path-item--selected ");
			
			if (Item.Completed)
				sb.Append("tf-path-item--completed ");

			return sb.ToString();
		}
	}
}