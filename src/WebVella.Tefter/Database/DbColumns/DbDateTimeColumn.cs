namespace WebVella.Tefter.Database;

public class DbDateTimeColumn : DbColumn
{
    public virtual DbType Type => DbType.DateTime;
    public new DateTime? DefaultValue { get; set; }
    public bool UseCurrentTimeAsDefaultValue { get; set; } = false;
}
