namespace WebVella.Tefter.UI.Components;
public partial class TucAvatar :ComponentBase
{
	[Parameter] public string Id { get; set; } = String.Empty;
	[Parameter] public string? Styles { get; set; }
	[Parameter] public string? Class { get; set; }
	[Parameter] public bool IsSmall { get; set; } = false;
	[Parameter] public bool UseUserColor { get; set; } = true;
	[Parameter] public TfUser? User { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public EventCallback OnClick { get; set; }

	private string _styles
	{
		get
		{
			var list = new List<string>();
			if (!String.IsNullOrWhiteSpace(Styles)) list.Add(Styles);
			if (User is not null)
			{
				if (UseUserColor && User.Settings is not null && User.Settings.ThemeColor != TfColor.Black)
					list.Add($"background-color:{User.Settings.ThemeColor.GetAttribute().Value};");
				else
					list.Add("background-color:var(--neutral-fill-layer-hover);");
			}
			else
			{
				list.Add("background-color:var(--neutral-fill-layer-hover);");
			}

			if (OnClick.HasDelegate)
				list.Add("cursor:pointer;");

			return String.Join("", list);
		}
	}

	private string _cssClass
	{
		get
		{
			var list = new List<string>();
			list.Add(Class);
			if (IsSmall) list.Add("tf-small");

			return String.Join(" ", list);
		}
	}

	private string _title
	{
		get
		{
			if (User is not null) return User.Names;
			return "";
		}
	}

	private async Task _onClick()
	{
		await OnClick.InvokeAsync();
	}
}