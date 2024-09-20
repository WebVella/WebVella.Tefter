﻿
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter GUID")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.Components.GuidDisplayColumnComponent.TfGuidDisplayColumnComponent", "WebVella.Tefter")]
public partial class TfGuidDisplayColumnComponent : TfBaseViewColumn<TfGuidDisplayColumnComponentOptions>
{
	public TfGuidDisplayColumnComponent()
	{
	}
	public TfGuidDisplayColumnComponent(TfComponentContext context)
	{
		Context = context;
	}
	public override TfBaseViewColumnExportData GetExportData(){ 
		return new TfBaseViewColumnExportData
		{
			Value = GetDataObjectByAlias<Guid>("Value")?.ToString(),
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

public class TfGuidDisplayColumnComponentOptions
{

}