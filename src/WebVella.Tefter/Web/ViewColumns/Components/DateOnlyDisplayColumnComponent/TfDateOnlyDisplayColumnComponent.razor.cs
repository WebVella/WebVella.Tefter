using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter Date")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.DateOnlyDisplayColumnComponent.TfDateOnlyDisplayColumnComponent","WebVella.Tefter")]
public partial class TfDateOnlyDisplayColumnComponent : TfBaseViewColumn<TfDateOnlyDisplayColumnComponentOptions>
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
}

public class TfDateOnlyDisplayColumnComponentOptions
{
}