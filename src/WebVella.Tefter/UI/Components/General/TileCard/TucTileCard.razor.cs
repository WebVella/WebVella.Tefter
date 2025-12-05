using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.FluentUI.AspNetCore.Components;

namespace WebVella.Tefter.UI.Components;

public partial class TucTileCard : TfBaseComponent
{
	[Parameter] public string? Url { get; set; }
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? Subtitle { get; set; }
	[Parameter] public string? Description { get; set; }
	[Parameter] public string? Footer { get; set; }
	[Parameter] public string? FluentIconName { get; set; } = null;
	[Parameter] public string? IconText { get; set; } = null;
	[Parameter] public TfColor? Color { get; set; } = null;
	[Parameter] public List<TfMenuItem> Menu { get; set; } = new();


	private RenderFragment? _getFooter()
	{ 
		if (!String.IsNullOrWhiteSpace(Footer)) { 
			return builder =>
			{ 
				builder.OpenComponent<TucTileCardFooter>(0);
				builder.AddAttribute(1, "Footer", Footer);
				builder.CloseComponent();
			};
			
		}

		return null;
	}

}