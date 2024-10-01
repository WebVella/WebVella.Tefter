namespace WebVella.Tefter.Web.Models;
internal record TucDataProviderDataRow
{
	public ReadOnlyCollection<string> ColumnNames { get; init; }
	public ReadOnlyCollection<object> Values { get; init; }
	public ReadOnlyCollection<string> Warnings { get; init; }
	public ReadOnlyCollection<string> Errors { get; init; }

	public TucDataProviderDataRow() { }
	public TucDataProviderDataRow(TfDataProviderDataRow model)
	{
		ColumnNames = model.ColumnNames;
		Values = model.Values;
		Warnings = model.Warnings;
		Errors = model.Errors;
	}
}
