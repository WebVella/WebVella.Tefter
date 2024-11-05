namespace WebVella.Tefter.Database;

public record TfDateTimeDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "TIMESTAMP WITHOUT TIME ZONE";
}
