using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter DateTime")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.DateTimeDisplayColumnComponent.TfDateTimeDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfDateTimeDisplayColumnComponent : TfBaseViewColumn<TfDateTimeDisplayColumnComponentOptions>
{
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	protected override void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		base.OnValidationRequested(sender, e);
		//Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "problem with json");

	}
	private async Task _optionsTextValueChanged(string value, string propName)
	{
		var valueChanged = false;
		switch (propName)
		{
			case nameof(TfDateTimeDisplayColumnComponentOptions.DateTimeFormat):
				{
					if (options.DateTimeFormat != value)
					{
						options.DateTimeFormat = value;
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

public class TfDateTimeDisplayColumnComponentOptions
{
	[JsonPropertyName("DateTimeFormat")]
	public string DateTimeFormat { get; set; }
}