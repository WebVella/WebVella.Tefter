namespace WebVella.Tefter.Database;

public class DbDateColumn : DbColumn
{
    public virtual DbType Type => DbType.Date;
    public new DateOnly? DefaultValue { get; set; }
    public bool UseCurrentTimeAsDefaultValue { get; set; } = false;
}
