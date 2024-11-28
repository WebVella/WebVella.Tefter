namespace WebVella.Tefter.Web.Components;
public partial class TfSelectColor : TfBaseComponent
{
	[Parameter] public bool Required { get; set; } = false;
	[Parameter] public OfficeColor? Value { get; set; }
	[Parameter] public EventCallback<OfficeColor> ValueChanged { get; set; }
	private string _elementId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private bool _open = false;
	private bool _isReadonly { get => !ValueChanged.HasDelegate; }

	private List<OfficeColor> _colors
	{
		get => Enum.GetValues<OfficeColor>().ToList();
	}

	private void _onOpenChanged(bool isOpened)
	{
		_open = isOpened;
	}

	private async Task _optionChanged(OfficeColor option)
	{
		await ValueChanged.InvokeAsync(option);
		_open = false;
	}
}