using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;
[Description("Tefter Text Display")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.TextDisplayColumnComponent.TfTextDisplayColumnComponent","WebVella.Tefter")]
public partial class TfTextDisplayColumnComponent : TfBaseViewColumn<TfTextDisplayColumnComponentOptions>
{
	public TfTextDisplayColumnComponent()
	{
	}
	public TfTextDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	
	public override TfBaseViewColumnExportData GetExportData(){ 
		return new TfBaseViewColumnExportData
		{
			Value = GetDataObjectByAlias("Value"),
			Format = null
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

public class TfTextDisplayColumnComponentOptions
{

}