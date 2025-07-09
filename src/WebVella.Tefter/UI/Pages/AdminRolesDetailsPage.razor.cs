namespace WebVella.Tefter.UI.Pages;
public partial class AdminRolesDetailsPage : TfBasePage
{
	[Parameter] public Guid RoleId { get; set; }
	[Parameter] public string? Path { get; set; }
}