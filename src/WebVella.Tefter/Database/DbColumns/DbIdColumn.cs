namespace WebVella.Tefter.Database;

public record DbIdColumn : DbColumn
{
    public override string Name => Constants.DB_TABLE_ID_COLUMN_NAME;
    public override DbType Type => DbType.Id;
    internal override string DatabaseColumnType => "UUID";
}
