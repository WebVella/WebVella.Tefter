namespace WebVella.Tefter.Database;

public class DbDateTimeColumn : DbColumn
{
    public override DbType Type => DbType.DateTime;
    public bool UseCurrentTimeAsDefaultValue { get; set; } = false;
}
