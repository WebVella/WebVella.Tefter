namespace WebVella.Tefter.Database;

public record TfGuidDatabaseColumn : TfDatabaseColumnWithAutoDefaultValue
{
    internal override string DatabaseColumnType => "UUID";
}
