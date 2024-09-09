using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter Boolean")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.BooleanViewColumn.TfBooleanViewColumn", "WebVella.Tefter")]
public partial class TfBooleanViewColumn : TfBaseViewColumn<TfBooleanViewColumnOptions>
{
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	protected override void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		base.OnValidationRequested(sender,e);
		//Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "problem with json");

	}

	private async Task _optionsTextValueChanged(string value, string propName)
	{
		var valueChanged = false;
		switch (propName)
		{
			case nameof(TfBooleanViewColumnOptions.TrueValueOverrideText):
				{
					if (options.TrueValueOverrideText != value)
					{
						options.TrueValueOverrideText = value;
						valueChanged = true;
					}
				}
				break;
			case nameof(TfBooleanViewColumnOptions.FalseValueOverrideText):
				{
					if (options.FalseValueOverrideText != value)
					{
						options.FalseValueOverrideText = value;
						valueChanged = true;
					}
				}
				break;
		}
		if (valueChanged)
		{
			if (!ValueChanged.HasDelegate) return;
			await ValueChanged.InvokeAsync(JsonSerializer.Serialize(options));
		}

	}
	private async Task _optionsBooleanValueChanged(bool? value, string propName)
	{
		var valueChanged = false;
		switch (propName)
		{
			case nameof(TfBooleanViewColumnOptions.TrueValueShowAsIcon):
				{
					if (value.HasValue && options.TrueValueShowAsIcon != value.Value)
					{
						options.TrueValueShowAsIcon = value.Value;
						valueChanged = true;
					}
				}
				break;
			case nameof(TfBooleanViewColumnOptions.FalseValueShowAsIcon):
				{
					if (value.HasValue && options.FalseValueShowAsIcon != value.Value)
					{
						options.FalseValueShowAsIcon = value.Value;
						valueChanged = true;
					}
				}
				break;
		}
		if (valueChanged)
		{
			if (!ValueChanged.HasDelegate) return;
			await ValueChanged.InvokeAsync(JsonSerializer.Serialize(options));
		}

	}


}

public class TfBooleanViewColumnOptions
{
	[JsonPropertyName("TrueValueOverrideText")]
	public string TrueValueOverrideText { get; set; }

	[JsonPropertyName("TrueValueShowAsIcon")]
	public bool TrueValueShowAsIcon { get; set; }

	[JsonPropertyName("FalseValueOverrideText")]
	public string FalseValueOverrideText { get; set; }

	[JsonPropertyName("FalseValueShowAsIcon")]
	public bool FalseValueShowAsIcon { get; set; }
}