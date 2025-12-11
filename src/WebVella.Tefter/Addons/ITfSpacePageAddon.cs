namespace WebVella.Tefter.Addons;

public interface ITfSpacePageAddon : ITfAddon
{
	public TfSpacePageAddonContext Context { get; set; }
	public Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context);
	public Task<string> OnPageUpdated(IServiceProvider serviceProvider, TfSpacePageAddonContext context);
	public Task OnPageDeleted(IServiceProvider serviceProvider, TfSpacePageAddonContext context);
	public string GetOptions();
	public List<ValidationError> ValidateOptions();
	public List<TfScreenRegionTab> GetManagementTabs();
}

public class TfSpacePageAddonMeta
{
	public Guid ComponentId { get; init; }
	internal ITfSpacePageAddon Instance { get; init; }
}

public class TfSpacePageAddonContext
{
	public TfSpacePage SpacePage { get; set; } = null!;
	public Guid? TemplateId { get; set; } = null;
	public TfSpace? Space { get; set; }
	public TfUser CurrentUser { get; set; } = null!;
	public string? Icon { get; set; }
	public string? ComponentOptionsJson { get; set; }
	public TfRelDataIdentityQueryInfo? RelDataIdentityQueryInfo { get; set; }
	public EventCallback<string> ComponentOptionsJsonChanged { get; set; }
	public TfComponentMode Mode { get; set; }
	public EventCallback EditNode { get; set; }
	public EventCallback DeleteNode { get; set; }
}