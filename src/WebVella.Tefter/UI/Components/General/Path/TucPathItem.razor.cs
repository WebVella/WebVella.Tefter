namespace WebVella.Tefter.UI.Components;

public partial class TucPathItem : TfBaseComponent
{
	[Parameter] public TfMenuItem Item { get; set; } = null!;

	private string _css
	{
		get
		{
			var sb = new StringBuilder();
			sb.AppendLine("tf-path__item ");
			if (Item.Selected)
				sb.Append("tf-path__item--selected ");
			
			if (Item.Completed)
				sb.Append("tf-path__item--completed ");

			return sb.ToString();
		}
	}
}