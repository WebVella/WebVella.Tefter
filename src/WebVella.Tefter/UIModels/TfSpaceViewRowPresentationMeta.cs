using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfSpaceViewRowPresentationMeta
{
	public bool EditMode { get; set; } = false;
	public bool Selected { get; set; } = false;
	public TfColor? ForegroudColor { get; set; } = null;
	public TfColor? BackgroundColor { get; set; } = null;

	public string RowClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(" tf-grid-tr ");
			if(Selected)
				sb.Append(" tf-grid-tr--selected ");
			if (EditMode)
				sb.Append(" tf-grid-tr--edited ");
			return sb.ToString();
		}
	}

	public string RowStyles
	{
		get
		{
			var sb = new StringBuilder();
			if(ForegroudColor is not null)
				sb.Append($"color: var(--tf-td-color-{ForegroudColor.GetColor().Name});");
			if (BackgroundColor is not null)
				sb.Append($"background-color: var(--tf-td-fill-{ForegroudColor.GetColor().Name});");
			return sb.ToString();
		}		
	}
}
