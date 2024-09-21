using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter Date")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.DateOnlyDisplayColumnComponent.TfDateOnlyDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfDateOnlyDisplayColumnComponent : TfBaseViewColumn<TfDateOnlyDisplayColumnComponentOptions>
{
	public TfDateOnlyDisplayColumnComponent()
	{
	}
	public TfDateOnlyDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	public override TfBaseViewColumnExportData GetExportData()
	{
		return new TfBaseViewColumnExportData
		{
			Value = GetDataObjectByAlias<DateTime>("Value")?.ToString("dd MMM yyyy"),
			Format = "dd MMM yyyy"
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
}

public class TfDateOnlyDisplayColumnComponentOptions
{
	[JsonPropertyName("Format")]
	public string Format { get; set; }
}