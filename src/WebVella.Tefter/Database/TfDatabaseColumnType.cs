namespace WebVella.Tefter.Database;

public enum TfDatabaseColumnType
{
	[Description("short integer")]
	ShortInteger = 0,
	[Description("integer")]
	Integer = 1,
	[Description("long integer")]
	LongInteger = 2,
	[Description("number")]
	Number = 3,
	[Description("boolean")]
	Boolean = 4,
	[Description("date only")]
	DateOnly = 5,
	[Description("date & time")]
	DateTime = 6,
	[Description("short text")]
	ShortText = 7,
	[Description("text")]
	Text = 8,
	[Description("GUID")]
	Guid = 9
}
