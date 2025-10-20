namespace WebVella.Tefter.UI.Addons;

public partial class TucSelectViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;	
	[Parameter] public List<TfSelectOption?>? Value { get; set; }
	
	private string _getStyle(TfSelectOption selectedOption)
	{
		var sb = new StringBuilder();
		sb.Append("width:100%;");
		if (!String.IsNullOrWhiteSpace(selectedOption?.Color))
			sb.Append($"color:{selectedOption.Color};");
		if (!String.IsNullOrWhiteSpace(selectedOption?.BackgroundColor))
			sb.Append($"background-color:{selectedOption.BackgroundColor};");

		return sb.ToString();
	}

	private string _getClass(TfSelectOption selectedOption)
	{
		var sb = new StringBuilder();
		sb.Append("tf-select-btn ");
		if (!String.IsNullOrWhiteSpace(selectedOption?.BackgroundColor))
			sb.Append("tf-select-btn--with-background ");

		return sb.ToString();
	}	
}
