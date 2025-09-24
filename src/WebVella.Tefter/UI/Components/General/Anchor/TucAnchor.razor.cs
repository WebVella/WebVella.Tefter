namespace WebVella.Tefter.UI.Components;

public partial class TucAnchor : TfBaseComponent
{
	[Parameter] public string? Id { get; set; } = null;
	[Parameter] public string? Href { get; set; } = null;
	[Parameter] public string? Target { get; set; } = null;
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public string? Style { get; set; } = null;
	[Parameter] public EventCallback OnClick { get; set; }
	[Parameter] public Icon? IconStart { get; set; } = null;
	[Parameter] public Icon? IconEnd { get; set; } = null;
	[Parameter] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; } = null;
	[Parameter] public Appearance? Appearance { get; set; } = null;
	[Parameter] public RenderFragment? ChildContent { get; set; } = null;

	IReadOnlyDictionary<string, object>? _additionalAttributes
	{
		get
		{
			var result = new Dictionary<string, object>();
			if (AdditionalAttributes is not null)
			{
				foreach (var key in AdditionalAttributes.Keys)
				{
					result[key] = AdditionalAttributes[key];
				}
			}
			if (!String.IsNullOrWhiteSpace(Title))
				result["title"] = Title;


			if (String.IsNullOrWhiteSpace(Href) || Href == "#")
			{
				result["data-onclick"] = "true";
			}

			return result.Keys.Count == 0 ? null : result;
		}
	}

	async Task _OnClick()
	{
		if(!OnClick.HasDelegate) return;
		Console.WriteLine("Anchor clicked");
		await OnClick.InvokeAsync();
	}
}