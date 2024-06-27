namespace WebVella.Tefter.Database;

public record LongIntegerDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "BIGINT";
}
