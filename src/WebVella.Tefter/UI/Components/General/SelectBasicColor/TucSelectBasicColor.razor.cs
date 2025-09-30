using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace WebVella.Tefter.UI.Components;

/// <summary>
/// A Blazor component that allows users to select a color using the browser's native color input.
/// This component provides a simple interface for color selection with support for various color formats.
/// </summary>
public partial class TucSelectBasicColor : TfBaseComponent
{
	/// <summary>
	/// Gets or sets the currently selected color value.
	/// </summary>
	[Parameter]
	public string? Value { get; set; }

	/// <summary>
	/// Event callback that is triggered when the selected color changes.
	/// </summary>
	[Parameter]
	public EventCallback<string?> ValueChanged { get; set; }

	/// <summary>
	/// Gets or sets the label text for the color selector.
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
	/// Gets or sets whether the component is required (validates that a color is selected).
	/// </summary>
	[Parameter]
	public bool Required { get; set; } = false;


	/// <summary>
	/// Gets or sets the CSS class for the color input element.
	/// </summary>
	[Parameter]
	public string? InputClass { get; set; }

	/// <summary>
	/// Gets or sets the CSS style for the color input element.
	/// </summary>
	[Parameter]
	public string? InputStyle { get; set; }

	/// <summary>
	/// Gets or sets the type of color picker to use. Default is "color".
	/// </summary>
	[Parameter]
	public string Type { get; set; } = "color";

	/// <summary>
	/// Gets or sets whether to show a preview of the selected color.
	/// </summary>
	[Parameter]
	public bool ShowPreview { get; set; } = true;

	/// <summary>
	/// Gets or sets the CSS class for the preview element.
	/// </summary>
	[Parameter]
	public string? PreviewClass { get; set; }

	/// <summary>
	/// Gets or sets the CSS style for the preview element.
	/// </summary>
	[Parameter]
	public string? PreviewStyle { get; set; }

	/// <summary>
	/// Gets or sets whether to display the color value as text next to the input.
	/// </summary>
	[Parameter]
	public bool ShowColorValue { get; set; } = false;

	/// <summary>
	/// Gets or sets the CSS class for the color value text element.
	/// </summary>
	[Parameter]
	public string? ColorValueClass { get; set; }

	/// <summary>
	/// Gets or sets the CSS style for the color value text element.
	/// </summary>
	[Parameter]
	public string? ColorValueStyle { get; set; }

	private string? _selectedColorHash = null;
	private bool _shouldRender = true;

	protected override async Task OnParametersSetAsync()
	{
		// Create a hash of the current selected color to detect changes
		var selectedColorHash = Value is not null ? JsonSerializer.Serialize(Value) : null;
		var colorChanged = selectedColorHash != _selectedColorHash;

		_selectedColorHash = selectedColorHash;

		// If the color has changed, trigger a re-render
		if (colorChanged)
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task HandleColorChange(ChangeEventArgs e)
	{
		var newColor = e.Value?.ToString();
		Value = newColor;
		
		// Trigger the event callback if it's registered
		if (ValueChanged.HasDelegate)
		{
			await ValueChanged.InvokeAsync(newColor);
		}
	}
}