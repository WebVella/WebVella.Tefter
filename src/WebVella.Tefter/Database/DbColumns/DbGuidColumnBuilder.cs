namespace WebVella.Tefter.Database;

public class DbGuidColumnBuilder : DbColumnBuilder
{
    private bool _generateNewIdAsDefaultValue = false;

    public DbGuidColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbGuidColumnBuilder DefaultValue(Guid? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DbGuidColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbGuidColumnBuilder IsNullable(bool isNullable)
    {
        _isNullable = isNullable;
        return this;
    }
    public DbGuidColumnBuilder GenerateNewIdAsDefaultValue(bool generate)
    {
        _generateNewIdAsDefaultValue = generate;

        return this;
    }

    internal override DbGuidColumn Build()
    {
        return new DbGuidColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            GenerateNewIdAsDefaultValue = _generateNewIdAsDefaultValue,
            Name = _name,   
            Type = DbType.Guid
        }; 
    }
}