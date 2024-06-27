namespace WebVella.Tefter.Database;

public record IntegerDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "INTEGER";
}
