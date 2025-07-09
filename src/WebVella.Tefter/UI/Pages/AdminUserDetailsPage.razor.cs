namespace WebVella.Tefter.UI.Pages;
public partial class AdminUserDetailsPage : TfBasePage
{
	[Parameter] public Guid UserId { get; set; }
	[Parameter] public string? Path { get; set; }
}