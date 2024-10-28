
namespace WebVella.Tefter.Web.Models;

public record TucTreeViewItem : ITreeViewItem
{
	public string Id { get; set; }
	public string Text { get; set; }
	public IEnumerable<ITreeViewItem> Items { get; set; }
	public Icon IconCollapsed { get; set; }
	public Icon IconExpanded { get; set; }
	public bool Disabled { get; set; }
	public bool Expanded { get; set; } = false;
	public bool Selected { get; set; } = false;
	public Func<TreeViewItemExpandedEventArgs, Task> OnExpandedAsync { get; set; }
	public object Data { get; set; }
}
