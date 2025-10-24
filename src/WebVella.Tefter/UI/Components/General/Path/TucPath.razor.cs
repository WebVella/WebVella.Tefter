namespace WebVella.Tefter.UI.Components;
public partial class TucPath : TfBaseComponent
{
	[Parameter] public string? Style { get; set; } = null;
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public List<TfMenuItem> Items { get; set; } = new();

	private string _css
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tf-path ");
			sb.Append(Class);
			return sb.ToString();
		}
	}

}