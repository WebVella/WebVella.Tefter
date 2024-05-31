namespace WebVella.Tefter.Database;

public class DbNumberColumn : DbColumn
{
    public virtual DbType Type => DbType.Number;
    public new decimal? DefaultValue { get; set; }
}
