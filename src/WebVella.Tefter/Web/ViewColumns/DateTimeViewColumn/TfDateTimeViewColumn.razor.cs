using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter DateTime")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.DateTimeViewColumn.TfDateTimeViewColumn","WebVella.Tefter")]
public partial class TfDateTimeViewColumn : TfBaseViewColumn<TfDateTimeViewColumnOptions>
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

public class TfDateTimeViewColumnOptions
{
}