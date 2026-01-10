using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfSpaceViewRowPresentationMeta
{
	public bool EditMode { get; set; } = false;
	public bool Selected { get; set; } = false;
	public TfColor? ForegroundColor { get; set; } = null;
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
			if(ForegroundColor is not null)
				sb.Append($"--tf-grid-td-color: var(--tf-{ForegroundColor.GetColor().Name}-500);");
			if (BackgroundColor is not null)
				sb.Append($"--tf-grid-td-fill: color-mix(in srgb, var(--neutral-fill-rest), var(--tf-{BackgroundColor.GetColor().Name}-500) 5%);");
			return sb.ToString();
		}		
	}
}
