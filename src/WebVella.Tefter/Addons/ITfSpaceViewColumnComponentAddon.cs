namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnComponentAddon : ITfAddon
{
	public List<Type> SupportedColumnTypes { get; init; }
	public List<Guid> SupportedColumnTypeAddons { get; init; }
	void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell);
}


public class TfSpaceViewColumnComponentAddonMeta
{
	public ITfSpaceViewColumnComponentAddon Instance { get; init; }
}
