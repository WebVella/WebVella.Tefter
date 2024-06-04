namespace WebVella.Tefter.Database;

public class DbTableIdColumn : DbColumn
{
    public override string Name => Constants.DB_TABLE_ID_NAME;
    public override DbType Type => DbType.TableId;
}
