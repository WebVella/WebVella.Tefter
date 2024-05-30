namespace WebVella.Tefter.Database;

public class DbTextColumn : DbColumn
{
    public virtual DbType Type => DbType.Text;
    public new Guid? DefaultValue { get; set; }
}
