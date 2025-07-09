namespace WebVella.Tefter.UI.Components;
public partial class TucSelectColor : TfBaseComponent
{
	[Parameter] public bool Required { get; set; } = false;
	[Parameter] public TfColor? Value { get; set; }
	[Parameter] public EventCallback<TfColor> ValueChanged { get; set; }
	private string _elementId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private bool _open = false;
	private bool _isReadonly { get => !ValueChanged.HasDelegate; }

	private List<TfColor> _colors
	{
		get => TfConverters.GetSelectableColors();
	}

	private void _onOpenChanged(bool isOpened)
	{
		_open = isOpened;
	}

	private async Task _optionChanged(TfColor option)
	{
		await ValueChanged.InvokeAsync(option);
		_open = false;
	}
}