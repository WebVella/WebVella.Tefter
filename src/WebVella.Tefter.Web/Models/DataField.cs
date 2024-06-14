namespace WebVella.Tefter.Web.Models;

public record DataField
{
	public DataFieldType Type { get; init; } //Should use DbType in the future
	public object Value { get; init; }
}
