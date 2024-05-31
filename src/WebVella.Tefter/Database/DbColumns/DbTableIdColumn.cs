namespace WebVella.Tefter.Database;

public class DbTableIdColumn : DbColumn
{
    public new string Name => "tefter_id";
    public virtual DbType Type => DbType.TableId;
}
