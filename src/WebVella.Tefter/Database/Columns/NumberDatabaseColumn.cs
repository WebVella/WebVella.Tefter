namespace WebVella.Tefter.Database;

public record NumberDatabaseColumn : DatabaseColumn
{
    internal override string DatabaseColumnType => "NUMERIC";
}
