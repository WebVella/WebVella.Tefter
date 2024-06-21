namespace WebVella.Tefter.Web.Components.Empty;
public partial class TfEmpty : TfBaseComponent
{
	[Parameter]
	public string Text { get; set; } = "No items found";
}