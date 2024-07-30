namespace WebVella.Tefter.Web.Components.Pager;
public partial class TfPager : TfBaseComponent
{
	[Parameter] public int Page { get; set; }

	[Parameter] public EventCallback GoLast { get; set; }
	[Parameter] public EventCallback GoNext { get; set; }
	[Parameter] public EventCallback GoFirst { get; set; }
	[Parameter] public EventCallback GoPrevious { get; set; }
	[Parameter] public EventCallback<int> GoOnPage { get; set; }

}