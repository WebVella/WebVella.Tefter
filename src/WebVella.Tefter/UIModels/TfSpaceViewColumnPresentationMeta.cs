using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfSpaceViewColumnPresentationMeta
{
	public bool IsCheckbox { get; set; } = false;
	public short? Width { get; set; } = null;
	public short? FreezeLeftWidth { get; set; } = null;
	public bool IsLastFreezeLeft { get; set; } = false;
	public short? FreezeRightWidth { get; set; } = null;
	public bool IsFirstFreezeRight { get; set; } = false;
	public TfSortDirection? SortDirection { get; set; } = null;
	public string TableColStyles
	{
		get
		{
			var sb = new StringBuilder();
			if (Width is not null)
			{
				sb.Append($"width: {Width.Value}px;");
				sb.Append($"min-width: {Width.Value}px;");
			}
			return sb.ToString();
		}
	}

	public string HeaderCellClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tf-grid-th ");
			if (IsCheckbox)
				sb.Append("tf-grid-th--check ");

			if(FreezeLeftWidth is not null || FreezeRightWidth is not null || IsCheckbox)
				sb.Append("tf--sticky ");

			if(IsLastFreezeLeft)
				sb.Append("tf--sticky-last ");

			if (IsFirstFreezeRight)
				sb.Append("tf--sticky-first ");

			return sb.ToString();
		}
	}

	public string HeaderCellStyles
	{
		get
		{
			var sb = new StringBuilder();
			if(FreezeLeftWidth is not null)
				sb.Append($"left:{(FreezeLeftWidth.Value)}px;");

			if (FreezeRightWidth is not null)
				sb.Append($"right:{(FreezeRightWidth.Value)}px;");
			return sb.ToString();
		}
	}

	public string HeaderSortActionClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tf-column-action tf-column-sort ");
			if (SortDirection == TfSortDirection.ASC)
			{
				sb.Append("tf-column-sort--ascending ");
			}
			else if (SortDirection == TfSortDirection.DESC)
			{
				sb.Append("tf-column-sort--descending ");
			}
			else
			{
				sb.Append("tf-column-sort--none ");
			}
			return sb.ToString();
		}
	}

	public string BodyCellClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tf-grid-td ");
			if (IsCheckbox)
				sb.Append("tf-grid-td--check ");

			if (FreezeLeftWidth is not null || FreezeRightWidth is not null || IsCheckbox)
				sb.Append("tf--sticky ");

			if (IsLastFreezeLeft)
				sb.Append("tf--sticky-last ");

			if (IsFirstFreezeRight)
				sb.Append("tf--sticky-first ");

			return sb.ToString();
		}
	}
}
