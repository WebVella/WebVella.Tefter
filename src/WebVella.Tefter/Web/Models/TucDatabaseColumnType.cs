namespace WebVella.Tefter.Web.Models;

[TucEnumMatch(typeof(DatabaseColumnType))]
public enum TucDatabaseColumnType
{
	ShortInteger = 0,
	Integer = 1,
	LongInteger = 2,
	Number = 3,
	Boolean = 4,
	Date = 5,
	DateTime = 6,
	ShortText = 7,
	Text = 8,
	Guid = 9,
	AutoIncrement = 10
}
