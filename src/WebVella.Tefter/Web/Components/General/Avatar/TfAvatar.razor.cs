namespace WebVella.Tefter.Web.Components;
public partial class TfAvatar
{
	[Parameter] public string Id { get; set; }
	[Parameter] public string Styles { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public TucUser User { get; set; }
	[Parameter] public EventCallback OnClick { get; set; }

	private List<KeyValuePair<string, object>> _attributes = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (!String.IsNullOrWhiteSpace(Id))
			_attributes.Add(new KeyValuePair<string, object>("id", Id));

		if (!String.IsNullOrWhiteSpace(Class))
			_attributes.Add(new KeyValuePair<string, object>("class", Class));
	}

	private string _getStyle()
	{
		var list = new List<string>();
		if (!String.IsNullOrWhiteSpace(Styles)) list.Add(Styles);
		if (User is not null)
		{
			if (User.Settings is not null && User.Settings.ThemeColor != OfficeColor.Default)
				list.Add($"background-color:{User.Settings.ThemeColor.ToAttributeValue()};");
			else
				list.Add("background-color:var(--accent-stroke-control-active);");
		}
		else
		{
			list.Add("background-color:var(--accent-stroke-control-active);");
		}

		if (OnClick.HasDelegate)
			list.Add("cursor:pointer;");

		return String.Join("", list);
	}
	private async Task _onClick()
	{
		await OnClick.InvokeAsync();
	}
}