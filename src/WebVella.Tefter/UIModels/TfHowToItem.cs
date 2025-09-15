using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfHowToItem
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? FluentIconName { get; set; }
	public string? Color { get; set; }
	public string? BackgroundColor { get; set; }
	public string? MoreInfoUrl { get; set; }
	public int? Count { get; set; } = null;
	public string IconStyle
	{
		get
		{
			var sb = new StringBuilder();

			if (!String.IsNullOrWhiteSpace(Color))
				sb.Append($"color: {Color}; --accent-fill-rest: {Color};");

			if (String.IsNullOrWhiteSpace(BackgroundColor))
				sb.Append($"background-color: var(--tf-indigo-100);");
			else
				sb.Append($"background-color: {BackgroundColor};");

			return sb.ToString();
		}
	}
}
