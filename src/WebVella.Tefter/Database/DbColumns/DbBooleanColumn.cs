namespace WebVella.Tefter.Database;

public class DbBooleanColumn : DbColumn
{
    public virtual DbType Type => DbType.Boolean;
    public new bool? DefaultValue { get; set; }

}
