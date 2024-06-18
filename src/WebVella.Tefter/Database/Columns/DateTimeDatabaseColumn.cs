namespace WebVella.Tefter.Database;

public record DateTimeDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    public override DatabaseColumnType Type => Database.DatabaseColumnType.DateTime;
    internal override string DatabaseColumnType => "TIMESTAMPTZ";
}
