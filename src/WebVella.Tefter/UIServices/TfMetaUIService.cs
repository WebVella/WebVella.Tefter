namespace WebVella.Tefter.UIServices;

public partial interface ITfMetaUIService
{
	ReadOnlyCollection<ITfDataProviderAddon> GetDataProviderTypes();
	ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList();
	ReadOnlyCollection<TfScreenRegionComponentMeta> GetRegionComponentsMetaForContext(Type context, TfScreenRegionScope? scope = null);
	ReadOnlyCollection<TfScreenRegionComponentMeta> GetAddonAdminPages(string? search = null);
}
public partial class TfMetaUIService : ITfMetaUIService
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfMetaUIService> LOC;

	public TfMetaUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfMetaUIService>>() ?? default!;
	}


	public ReadOnlyCollection<ITfDataProviderAddon> GetDataProviderTypes() => _metaService.GetDataProviderTypes();

	public ReadOnlyCollection<DatabaseColumnTypeInfo> GetDatabaseColumnTypeInfosList()
	{
		var allTypes = TfService.GetDatabaseColumnTypeInfosList();
		return allTypes.Where(x => x.Type != TfDatabaseColumnType.AutoIncrement).ToList().AsReadOnly();
	}

	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetRegionComponentsMetaForContext(Type context, TfScreenRegionScope? scope = null)
	{
		return _metaService.GetRegionComponentsMeta(
			context: context,
			scope: scope);
	}
	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetAddonAdminPages(string? search = null)
		=> _metaService.GetAdminAddonPages(search);

}
