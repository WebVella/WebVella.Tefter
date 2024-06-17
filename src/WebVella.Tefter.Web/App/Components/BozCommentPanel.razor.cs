namespace Boz.Tefter.Components;
public partial class BozCommentPanel : IDialogContentComponent<string>
{
	[Parameter]
	public string Content { get;set; }
}