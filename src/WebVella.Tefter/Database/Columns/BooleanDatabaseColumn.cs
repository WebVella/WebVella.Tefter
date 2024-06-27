namespace WebVella.Tefter.Database;

public record BooleanDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "BOOLEAN";
}
