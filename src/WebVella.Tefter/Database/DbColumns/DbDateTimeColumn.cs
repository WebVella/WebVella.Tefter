namespace WebVella.Tefter.Database;

public record DbDateTimeColumn : DbColumn
{
    public override DbType Type => DbType.DateTime;
    public bool AutoDefaultValue { get; set; } = false;
}
