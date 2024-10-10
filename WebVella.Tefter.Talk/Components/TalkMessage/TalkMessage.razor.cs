

namespace WebVella.Tefter.Web.Components;

[LocalizationResource("WebVella.Tefter.Talk.Components.TalkMessage.TalkMessage", "WebVella.Tefter.Talk")]
public partial class TalkMessage : TfBaseComponent
{
	[Parameter] public TalkThread Item { get; set; }
	[Parameter] public TucUser CurrentUser { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public bool IsEdited { get; set; } = false;
	[Parameter] public EventCallback OnDelete { get; set; }
	[Parameter] public EventCallback OnEdit { get; set; }
	[Parameter] public EventCallback OnReply { get; set; }
	[Parameter] public EventCallback<string> OnEditSave { get; set; }
	[Parameter] public EventCallback OnEditCancel { get; set; }
	[Parameter] public bool IsSaving { get; set; } = false;
	[Parameter] public bool HideReplies { get; set; } = false;

	private TucUser _user
	{
		get
		{
			if (Item is null || Item.User is null) return null;
			return new TucUser(Item.User);
		}
	}
	private string _class
	{
		get
		{
			var cssClass = new List<string>();
			cssClass.Add("talk-message");
			cssClass.Add(Class);
			if (IsEdited) cssClass.Add("editor-active");

			return String.Join(" ", cssClass);
		}
	}

	private TfEditor _editor = null;

	private async Task _onUpdate()
	{

		await OnEditSave.InvokeAsync(Item.Content);
	}
}
