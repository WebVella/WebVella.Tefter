

using HtmlAgilityPack;
using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Talk.Components;

[LocalizationResource("WebVella.Tefter.Talk.Components.TalkMessage.TalkMessage", "WebVella.Tefter.Talk")]
public partial class TalkMessage : TfBaseComponent
{
	[Parameter] public TalkThread Item { get; set; }
	[Parameter] public TfUser CurrentUser { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public bool IsEdited { get; set; } = false;
	[Parameter] public EventCallback OnDelete { get; set; }
	[Parameter] public EventCallback OnEdit { get; set; }
	[Parameter] public EventCallback OnReply { get; set; }
	[Parameter] public EventCallback OnSubThreadView { get; set; }
	[Parameter] public EventCallback<string> OnEditSave { get; set; }
	[Parameter] public EventCallback OnEditCancel { get; set; }
	[Parameter] public bool IsSaving { get; set; } = false;
	[Parameter] public bool IsSubThread { get; set; } = false;

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

	private string _parentContent
	{
		get
		{
			if (Item.ParentThread is null || String.IsNullOrWhiteSpace(Item.ParentThread.Content)) return "";

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(Item.ParentThread.Content);
			var parentContent = doc.DocumentNode.InnerText;
			if (parentContent.Length <= 150) return parentContent;
			return parentContent.Substring(0, 150) + "...";
		}
	}

	private TucEditor _editor = null;

	private async Task _onUpdate()
	{

		await OnEditSave.InvokeAsync(Item.Content);
	}


}
