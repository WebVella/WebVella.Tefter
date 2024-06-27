namespace WebVella.Tefter.Database;

public record DateDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "DATE";
}
