namespace WebVella.Tefter.UI.Components;
public partial class TucSelectColor : TfBaseComponent
{
	[Parameter] public bool Required { get; set; } = false;
	[Parameter] public bool Readonly { get; set; } = false;
	[Parameter] public bool Disabled { get; set; } = false;
	private bool _isReadonly { get => Readonly || !ValueChanged.HasDelegate; }	
	[Parameter] public string? Placeholder { get; set; }
	[Parameter] public TfColor? Value { get; set; }
	[Parameter] public EventCallback<TfColor?> ValueChanged { get; set; }
	private string _elementId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private bool _open = false;

	private List<TfColor> _colors
	{
		get => TfConverters.GetSelectableColors();
	}

	private void _onOpenChanged(bool isOpened)
	{
		_open = isOpened;
	}

	private async Task _optionChanged(TfColor? option)
	{
		if(Required && option is null) {
			await ValueChanged.InvokeAsync(TfConstants.DefaultThemeColor);
		}
		else {
			await ValueChanged.InvokeAsync(option);
		}
		_open = false;
	}
}