namespace WebVella.Tefter.Database;

public record DateTimeDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "TIMESTAMPTZ";
}
