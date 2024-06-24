namespace WebVella.Tefter.Database.Dbo;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DboAutoIncrementModelAttribute : Attribute
{
    public DboAutoIncrementModelAttribute()
    {

    }


}
