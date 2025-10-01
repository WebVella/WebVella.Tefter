namespace WebVella.Tefter.UI.Components;

/// <summary>
/// A generic select component that allows users to choose an option from a list of items.
/// </summary>
/// <typeparam name="TOption">The type of the options in the select component.</typeparam>
public partial class TucSelect<TOption> : TfBaseComponent where TOption : notnull
{

	/// <summary>
	/// Gets or sets the currently selected option.
	/// </summary>
	[Parameter]
	public TOption? SelectedOption { get; set; }

	/// <summary>
	/// Event callback that is triggered when the selected option changes.
	/// </summary>
	[Parameter]
	public EventCallback<TOption?> SelectedOptionChanged { get; set; }

	/// <summary>
	/// Gets or sets the collection of items to display in the select dropdown.
	/// </summary>
	[Parameter]
	public IEnumerable<TOption> Items { get; set; } = Enumerable.Empty<TOption>();

	[Parameter]
	public Func<TOption, bool> OptionSelected { get; set; } = null!;

	/// <summary>
	/// Function that defines how to display each option's text. If not provided, ToString() is used by default.
	/// </summary>
	[Parameter]
	public Func<TOption, string?> OptionText { get; set; } = null!;

	/// <summary>
	/// Optional function that defines how to extract the value from each option. If not provided, no value extraction is performed.
	/// </summary>
	[Parameter]
	public Func<TOption, string?>? OptionValue { get; set; } = null;

	/// <summary>
	/// Optional render fragment that allows custom rendering of each option item.
	/// </summary>
	[Parameter]
	public virtual RenderFragment<TOption>? OptionTemplate { get; set; }

	/// <summary>
	/// Gets or sets the label text for the select component.
	/// </summary>
	[Parameter]
	public string? Label { get; set; }

	/// <summary>
	/// Gets or sets the inline CSS style for the component.
	/// </summary>
	[Parameter]
	public string? Style { get; set; }

	/// <summary>
	/// Gets or sets the CSS class for the component.
	/// </summary>
	[Parameter]
	public string? Class { get; set; }

	/// <summary>
	/// Gets or sets the placeholder text to display when no option is selected.
	/// </summary>
	[Parameter]
	public string? Placeholder { get; set; }

	/// <summary>
	/// Gets or sets the HTML Id attribute for the component.
	/// </summary>
	[Parameter]
	public string? Id { get; set; }

	/// <summary>
	/// Gets or sets the height of the component.
	/// </summary>
	[Parameter]
	public string? Height { get; set; }

	/// <summary>
	/// Gets or sets the width of the component.
	/// </summary>
	[Parameter]
	public string? Width { get; set; }

	/// <summary>
	/// Gets or sets whether the component is disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; } = false;

	/// <summary>
	/// Gets or sets whether the component is required (validates that a value is selected).
	/// </summary>
	[Parameter]
	public bool Required { get; set; } = false;


	/// <summary>
	/// Hash of the items collection to track changes in the items list.
	/// </summary>
	int _itemsCount = 0;

	/// <summary>
	/// Flag to control rendering behavior during parameter updates to avoid race conditions.
	/// </summary>
	bool _shouldRender = true;

	/// <summary>
	/// Called after component parameters have been set. This method handles logic for updating the component when items or selected option changes occur, particularly addressing a potential issue where both parameters change simultaneously.
	/// </summary>
	/// <returns>A Task representing the asynchronous operation.</returns>
	protected override async Task OnParametersSetAsync()
	{
		// If OptionText is not provided, default to using ToString() on the option
		if (OptionText is null && OptionTemplate is null)
			OptionText = x => x.ToString();

		// Generate hashes for items and selected option to detect changes
		var itemsCount = Items is not null ? Items.Count() : 0;

		// Handle a potential race condition when both items and selected option change at the same time
		if (itemsCount != _itemsCount)
		{
			_itemsCount = itemsCount;
			_shouldRender = false;
			await Task.Delay(1);
			await InvokeAsync(StateHasChanged);
			_shouldRender = true;
			await Task.Delay(1);
			await InvokeAsync(StateHasChanged);
		}

	}

	async Task _selectedOptionChanged(TOption? option)
	{
		if (option is null && SelectedOption is null) return;
		//Infinite change trigger
		if (JsonSerializer.Serialize(option) == JsonSerializer.Serialize(SelectedOption))
			return;
		Console.WriteLine($"_selectedOptionChanged: {(option is null ? "--NULL" : option)} /// {(SelectedOption is null ? "--NULL" : SelectedOption)}");
		SelectedOption = option;
		if (SelectedOptionChanged.HasDelegate)
			await SelectedOptionChanged.InvokeAsync(option);
	}
}