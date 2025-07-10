namespace WebVella.Tefter.UI.Components;
public partial class TucSelectComponent<TOption> : TfBaseComponent where TOption : notnull
{
	[Parameter] public TOption Value { get; set; }
	[Parameter] public EventCallback<TOption> ValueChanged { get; set; }
	[Parameter] public ICollection<TOption> Items { get; set; }
	[Parameter] public string Placeholder { get; set; }
	[Parameter] public Func<TOption, string> OptionIcon { get; set; }
	[Parameter] public Func<TOption, string> OptionText { get; set; }
	[Parameter] public Func<TOption, string> OptionDescription { get; set; }
	[Parameter] public Func<TOption, TOption, bool> OptionMatch { get; set; }
	[Parameter] public bool Disabled { get; set; } = false;
	private bool _isReadonly { get => Disabled || !ValueChanged.HasDelegate; }

	private string _elementId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private bool _open = false;

	private void _onOpenChanged(bool isOpened)
	{
		_open = isOpened;
	}

	private async Task _optionChanged(TOption option)
	{
		await ValueChanged.InvokeAsync(option);
		_open = false;
	}

	private string _getOptionText(TOption item)
	{
		if (item != null)
		{
			return OptionText.Invoke(item) ?? item.ToString();
		}
		else
		{
			return null;
		}
	}

	private string _getOptionDescription(TOption item)
	{
		if (item != null)
		{
			return OptionDescription.Invoke(item) ?? null;
		}
		else
		{
			return null;
		}
	}

	private string _getOptionIcon(TOption item)
	{
		if (item != null)
		{
			return OptionIcon.Invoke(item) ?? null;
		}
		else
		{
			return null;
		}
	}

	private bool _matchOptions(TOption item1, TOption item2)
	{
		if (OptionMatch is not null)
			return OptionMatch.Invoke(item1, item2);

		return false;
	}

}