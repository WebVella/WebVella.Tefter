
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter GUID")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.GuidViewColumn.TfGuidViewColumn", "WebVella.Tefter")]
public partial class TfGuidViewColumn : TfBaseViewColumn<TfGuidViewColumnOptions>
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

public class TfGuidViewColumnOptions
{

}