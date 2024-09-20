using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;
[Description("Tefter Text Edit")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TextEditColumnComponent.TfTextEditColumnComponent","WebVella.Tefter")]
public partial class TfTextEditColumnComponent : TfBaseViewColumn<TfTextEditColumnComponentOptions>
{
	public TfTextEditColumnComponent()
	{
	}
	public TfTextEditColumnComponent(TfComponentContext context)
	{
		Context = context;
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

	public override TfBaseViewColumnExportData GetExportData(){ 
		return new TfBaseViewColumnExportData
		{
			Value = GetDataObjectByAlias("Value"),
			Format = null
		};
	}

	private async Task _valueChanged(string value) => await ValueChanged.InvokeAsync(value);
}

public class TfTextEditColumnComponentOptions
{

}