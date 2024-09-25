namespace WebVella.Tefter.Database;

public record DateTimeDatabaseColumn : DatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "TIMESTAMP WITHOUT TIME ZONE";
}
