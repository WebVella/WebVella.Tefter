using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfSpaceViewRowPresentationMeta
{
	public bool UnsavedRow { get; set; } = false;
	public bool Selected { get; set; } = false;

	public string RowClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(" tf-grid-tr ");
			if(Selected)
				sb.Append(" tf-grid-tr--selected ");
			if (UnsavedRow)
				sb.Append(" tf-grid-tr--edited ");
			return sb.ToString();
		}
	}
}
