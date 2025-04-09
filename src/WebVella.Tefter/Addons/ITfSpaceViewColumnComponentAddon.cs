namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnComponentAddon : ITfAddon
{
	public List<Type> SupportedColumnTypes { get; init; }
	void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell);
}
