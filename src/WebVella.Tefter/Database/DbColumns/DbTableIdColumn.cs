namespace WebVella.Tefter.Database;

public class DbTableIdColumn : DbColumn
{
    public override string Name => "tefter_bg";
    public override DbType Type => DbType.TableId;
}
