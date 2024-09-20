using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;
[Description("Tefter Number")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.NumberDisplayColumnComponent.TfNumberDisplayColumnComponent","WebVella.Tefter")]
public partial class TfNumberDisplayColumnComponent : TfBaseViewColumn<TfNumberDisplayColumnComponentOptions>
{
	public TfNumberDisplayColumnComponent()
	{
	}
	public TfNumberDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}

	public override TfBaseViewColumnExportData GetExportData(){ 
		return new TfBaseViewColumnExportData
		{
			Value = GetDataObjectByAlias<decimal>("Value")?.ToString("N"),
			Format = "0.000"
		};
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	protected override void OnValidationRequested(object sender, ValidationRequestedEventArgs e)
	{
		base.OnValidationRequested(sender,e);
		//Context.ValidationMessageStore.Add(Context.EditContext.Field(nameof(TucSpaceViewColumn.CustomOptionsJson)), "problem with json");

	}
}

public class TfNumberDisplayColumnComponentOptions
{

}