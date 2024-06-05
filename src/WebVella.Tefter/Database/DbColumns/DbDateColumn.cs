namespace WebVella.Tefter.Database;

public record DbDateColumn : DbColumn
{
    public override DbType Type => DbType.Date;
    public bool AutoDefaultValue { get; set; } = false;
}
