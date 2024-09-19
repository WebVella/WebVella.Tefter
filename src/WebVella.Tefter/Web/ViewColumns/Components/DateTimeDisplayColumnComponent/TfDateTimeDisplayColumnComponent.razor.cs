using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter DateTime")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.DateTimeDisplayColumnComponent.TfDateTimeDisplayColumnComponent","WebVella.Tefter")]
public partial class TfDateTimeDisplayColumnComponent : TfBaseViewColumn<TfDateTimeDisplayColumnComponentOptions>
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

public class TfDateTimeDisplayColumnComponentOptions
{
}