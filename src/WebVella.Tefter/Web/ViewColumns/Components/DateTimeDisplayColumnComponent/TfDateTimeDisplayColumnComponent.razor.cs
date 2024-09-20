using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter DateTime")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.DateTimeDisplayColumnComponent.TfDateTimeDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfDateTimeDisplayColumnComponent : TfBaseViewColumn<TfDateTimeDisplayColumnComponentOptions>
{
	public TfDateTimeDisplayColumnComponent()
	{
	}
	public TfDateTimeDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	public override TfBaseViewColumnExportData GetExportData()
	{
		var options = GetOptions();
		var format = "dd MMM yyyy HH:mm";
		if (options is not null && !String.IsNullOrWhiteSpace(options.DateTimeFormat))
			format = options.DateTimeFormat;
		return new TfBaseViewColumnExportData
		{
			Value = GetDataObjectByAlias<DateTime>("Value")?.ToString(format),
			Format = format
		};
	}

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