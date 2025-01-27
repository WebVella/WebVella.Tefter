namespace WebVella.Tefter.Models;

public interface ITfSpaceViewColumnComponent
{
	public Guid Id { get; init; }
	public List<Type> SupportedColumnTypes { get; init; }
	void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell);
}
