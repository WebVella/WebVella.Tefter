namespace WebVella.Tefter.Database;

public record DateDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.Date;
    internal override string DatabaseColumnType => "DATE";
}
