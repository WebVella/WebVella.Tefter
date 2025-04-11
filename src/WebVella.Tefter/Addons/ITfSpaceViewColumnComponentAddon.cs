namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnComponentAddon : ITfAddon
{
	public List<Guid> SupportedColumnTypes { get; init; }
	void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell);
}


public class TfSpaceViewColumnComponentAddonMeta
{
	public ITfSpaceViewColumnComponentAddon Instance { get; init; }
}
