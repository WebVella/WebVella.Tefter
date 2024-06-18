namespace WebVella.Tefter.Database;

public record BooleanDatabaseColumn : DatabaseColumn
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.Boolean;
    internal override string DatabaseColumnType => "BOOLEAN";
}
