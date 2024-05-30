namespace WebVella.Tefter.Database;

public class DbGuidColumn : DbColumn
{
    public virtual DbType Type => DbType.Guid;
    public new Guid? DefaultValue { get; set; }
    public bool GenerateNewIdAsDefaultValue { get; set; } = false;
}
