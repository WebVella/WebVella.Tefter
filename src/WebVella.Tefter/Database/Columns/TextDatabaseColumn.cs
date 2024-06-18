namespace WebVella.Tefter.Database;

public record TextDatabaseColumn : DatabaseColumn
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.Text;
    internal override string DatabaseColumnType => "TEXT";
}
