namespace WebVella.Tefter.UI.Components;

public partial class TucSelect<TOption> : TfBaseComponent where TOption : notnull
{

	[Parameter]
	public TOption? SelectedOption { get; set; }

	[Parameter]
	public EventCallback<TOption?> SelectedOptionChanged { get; set; }

	[Parameter]
	public IEnumerable<TOption> Items { get; set; } = Enumerable.Empty<TOption>();

	[Parameter]
	public Func<TOption, string?> OptionText { get; set; } = default!;

	[Parameter]
	public Func<TOption, string?>? OptionValue { get; set; } = null;

	[Parameter]
	public virtual RenderFragment<TOption>? OptionTemplate { get; set; }

	[Parameter] public string? Label { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public string? Class { get; set; }
	[Parameter] public string? Placeholder { get; set; }
	[Parameter] public bool Disabled { get; set; } = false;
	[Parameter] public bool Required { get; set; } = false;


	string? _itemsHash = null;
	string? _selectedHash = null;
	bool _shouldRender = true;

	protected override async Task OnParametersSetAsync()
	{
		if (OptionText is null && OptionTemplate is null)
			OptionText = x => x.ToString();

		var itemsHash = Items is not null ? JsonSerializer.Serialize(Items) : null;
		var selectedHash = SelectedOption is not null ? JsonSerializer.Serialize(SelectedOption) : null;

		var itemsChanged = itemsHash != _itemsHash;
		var selectedChanged = selectedHash != _selectedHash;

		_itemsHash = itemsHash;
		_selectedHash = selectedHash;

		//There is a problem when both parameters are changed at the same time, First items needs to be changed and then selected
		if (itemsChanged && selectedChanged)
		{
			_shouldRender = false;
			await Task.Delay(1);
			await InvokeAsync(StateHasChanged);
			_shouldRender = true;
			await Task.Delay(1);
			await InvokeAsync(StateHasChanged);
		}

	}
}