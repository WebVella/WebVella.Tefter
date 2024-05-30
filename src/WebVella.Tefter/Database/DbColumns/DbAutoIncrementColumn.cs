namespace WebVella.Tefter.Database;

public class DbAutoIncrementColumn : DbColumn
{
    public virtual DbType Type => DbType.AutoIncrement;
    public new string DefaultValue => null;
    public new bool IsNullable => false;
    
    //public override string GetCreateSql()
    //{
    //    return $"ALTER TABLE \"{Table.Name}\" ADD COLUMN \"{Name}\" SERIAL;";
    //}

    //public override string GetDropSql()
    //{
    //    return $"ALTER TABLE \"{Table.Name}\" DROP COLUMN \"{Name}\";";
    //}
}
