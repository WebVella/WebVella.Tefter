
namespace WebVella.Tefter.Web.ViewColumns;

[Description("Tefter GUID")]
[LocalizationResource("WebVella.Tefter.Web.ViewColumns.GuidViewColumn.TfGuidViewColumn","WebVella.Tefter")]
public partial class TfGuidViewColumn : TfBaseViewColumn
{
	protected override void OnInitialized()
	{
		base.OnInitialized();
	}

	private async Task _changeValue()
	{
		if (!ValueChanged.HasDelegate) return;
		await ValueChanged.InvokeAsync("{time:'" + DateTime.Now.ToString()+ "'}");
	}
}