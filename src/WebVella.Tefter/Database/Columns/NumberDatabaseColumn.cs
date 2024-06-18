namespace WebVella.Tefter.Database;

public record NumberDatabaseColumn : DatabaseColumn
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.Number;
    internal override string DatabaseColumnType => "NUMERIC";
}
