namespace WebVella.Tefter.Database;

public class DbColumnCollectionBuilder
{
    private readonly List<DbColumnBuilder> _builders;
    private DbTableBuilder _tableBuilder;

    internal ReadOnlyCollection<DbColumnBuilder> Builders => _builders.AsReadOnly();

    public DbColumnCollectionBuilder(DbTableBuilder tableBuilder)
    {
        _builders = new List<DbColumnBuilder>();
        _tableBuilder = tableBuilder;
    }


    #region <--- id --->

    public DbColumnCollectionBuilder AddNewTableIdColumn(Guid? id = null)
    {
        var builder = (DbIdColumnBuilder)_builders
             .SingleOrDefault(x => x.GetType() == typeof(DbIdColumnBuilder));

        if (builder is not null)
            throw new DbBuilderException($"Column of type Id already exists in columns. Only one instance can be created.");

        if (id != null)
            _builders.Add(new DbIdColumnBuilder(id.Value, _tableBuilder));
        else
            _builders.Add(new DbIdColumnBuilder(_tableBuilder));

        return this;
    }

    internal DbIdColumnBuilder AddNewTableIdColumnBuilder(Guid? id = null)
    {
        var builder = (DbIdColumnBuilder)_builders
             .SingleOrDefault(x => x.GetType() == typeof(DbIdColumnBuilder));

        if (builder is not null)
            throw new DbBuilderException($"Column of type Id already exists in columns. Only one instance can be created.");

        if (id != null)
            builder = new DbIdColumnBuilder(id.Value, _tableBuilder);
        else
            builder = new DbIdColumnBuilder(_tableBuilder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- auto increment --->

    public DbColumnCollectionBuilder AddNewAutoIncrementColumn(string name)
    {
        return AddNewAutoIncrementColumn(Guid.NewGuid(), name);
    }

    internal DbAutoIncrementColumnBuilder AddNewAutoIncrementColumnBuilder(string name)
    {
        return AddNewAutoIncrementColumnBuilder(Guid.NewGuid(), name);
    }

    public DbColumnCollectionBuilder AddNewAutoIncrementColumn(Guid id, string name)
    {
        _tableBuilder.ValidateColumnName(name);

        DbAutoIncrementColumnBuilder builder = new DbAutoIncrementColumnBuilder(id, name, _tableBuilder);

        _builders.Add(builder);

        return this;
    }

    internal DbAutoIncrementColumnBuilder AddNewAutoIncrementColumnBuilder(Guid id, string name)
    {
        _tableBuilder.ValidateColumnName(name);

        DbAutoIncrementColumnBuilder builder = new DbAutoIncrementColumnBuilder(id, name, _tableBuilder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- guid --->

    public DbColumnCollectionBuilder AddNewGuidColumn(string name, Action<DbGuidColumnBuilder> action)
    {
        return AddNewGuidColumn(Guid.NewGuid(), name, action);
    }

    internal DbGuidColumnBuilder AddNewGuidColumnBuilder(string name, Action<DbGuidColumnBuilder> action = null)
    {
        return AddNewGuidColumnBuilder(Guid.NewGuid(), name, action);
    }

    public DbColumnCollectionBuilder AddNewGuidColumn(Guid id, string name, Action<DbGuidColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);

        DbGuidColumnBuilder builder = new DbGuidColumnBuilder(id, name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbGuidColumnBuilder AddNewGuidColumnBuilder(Guid id, string name, Action<DbGuidColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name);

        DbGuidColumnBuilder builder = new DbGuidColumnBuilder(id, name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);
        return builder;
    }

    public DbColumnCollectionBuilder WithGuidColumn(string name, Action<DbGuidColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbGuidColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbGuidColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type GUID and name '{name}' is not found.");

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbGuidColumnBuilder WithGuidColumnBuilder(string name, Action<DbGuidColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbGuidColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbGuidColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type GUID and name '{name}' is not found.");

        if (action != null)
            action(builder);

        return builder;
    }


    #endregion

    #region <--- number --->

    public DbColumnCollectionBuilder AddNewNumberColumn(string name, Action<DbNumberColumnBuilder> action)
    {
        return AddNewNumberColumn(Guid.NewGuid(), name, action);
    }

    internal DbNumberColumnBuilder AddNewNumberColumnBuilder(string name, Action<DbNumberColumnBuilder> action = null)
    {
        return AddNewNumberColumnBuilder(Guid.NewGuid(), name, action);
    }

    public DbColumnCollectionBuilder AddNewNumberColumn(Guid id, string name, Action<DbNumberColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbNumberColumnBuilder builder = new DbNumberColumnBuilder(id, name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbNumberColumnBuilder AddNewNumberColumnBuilder(Guid id, string name, Action<DbNumberColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name);
        DbNumberColumnBuilder builder = new DbNumberColumnBuilder(id, name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DbColumnCollectionBuilder WithNumerColumn(string name, Action<DbNumberColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbNumberColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbNumberColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Number and name '{name}' is not found.");

        action(builder);
        return this;
    }

    internal DbNumberColumnBuilder WithNumerColumnBuilder(string name, Action<DbNumberColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbNumberColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbNumberColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Number and name '{name}' is not found.");

        if (action != null)
            action(builder);

        return builder;
    }

    #endregion

    #region <--- boolean --->

    public DbColumnCollectionBuilder AddNewBooleanColumn(string name, Action<DbBooleanColumnBuilder> action)
    {
        return AddNewBooleanColumn(Guid.NewGuid(), name, action);
    }

    internal DbBooleanColumnBuilder AddNewBooleanColumnBuilder(string name, Action<DbBooleanColumnBuilder> action = null)
    {
        return AddNewBooleanColumnBuilder(Guid.NewGuid(), name, action);
    }

    public DbColumnCollectionBuilder AddNewBooleanColumn(Guid id, string name, Action<DbBooleanColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);

        DbBooleanColumnBuilder builder = new DbBooleanColumnBuilder(id, name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbBooleanColumnBuilder AddNewBooleanColumnBuilder(Guid id, string name, Action<DbBooleanColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name);

        DbBooleanColumnBuilder builder = new DbBooleanColumnBuilder(id, name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DbColumnCollectionBuilder WithBooleanColumn(string name, Action<DbBooleanColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbBooleanColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbBooleanColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Boolean and name '{name}' is not found.");

        action(builder);
        return this;
    }

    internal DbBooleanColumnBuilder WithBooleanColumnBuilder(string name, Action<DbBooleanColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbBooleanColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbBooleanColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Boolean and name '{name}' is not found.");

        if (action != null)
            action(builder);

        return builder;
    }

    #endregion

    #region <--- date --->

    public DbColumnCollectionBuilder AddNewDateColumn(string name, Action<DbDateColumnBuilder> action)
    {
        return AddNewDateColumn(Guid.NewGuid(), name, action);
    }
    internal DbDateColumnBuilder AddNewDateColumnBuilder(string name, Action<DbDateColumnBuilder> action = null)
    {
        return AddNewDateColumnBuilder(Guid.NewGuid(), name, action);
    }

    public DbColumnCollectionBuilder AddNewDateColumn(Guid id, string name, Action<DbDateColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);

        DbDateColumnBuilder builder = new DbDateColumnBuilder(id, name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbDateColumnBuilder AddNewDateColumnBuilder(Guid id, string name, Action<DbDateColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name);

        DbDateColumnBuilder builder = new DbDateColumnBuilder(id, name, _tableBuilder);

        if (action == null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DbColumnCollectionBuilder WithDateColumn(string name, Action<DbDateColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbDateColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Date and name '{name}' is not found.");

        action(builder);

        return this;
    }

    internal DbDateColumnBuilder WithDateColumnBuilder(string name, Action<DbDateColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbDateColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Date and name '{name}' is not found.");

        if (action != null)
            action(builder);

        return builder;
    }

    #endregion

    #region <--- datetime --->

    public DbColumnCollectionBuilder AddNewDateTimeColumn(string name, Action<DbDateTimeColumnBuilder> action)
    {
        return AddNewDateTimeColumn(Guid.NewGuid(), name, action);
    }

    internal DbDateTimeColumnBuilder AddNewDateTimeColumnBuilder(string name, Action<DbDateTimeColumnBuilder> action = null)
    {
        return AddNewDateTimeColumnBuilder(Guid.NewGuid(), name, action);
    }

    public DbColumnCollectionBuilder AddNewDateTimeColumn(Guid id, string name, Action<DbDateTimeColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);

        DbDateTimeColumnBuilder builder = new DbDateTimeColumnBuilder(id, name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbDateTimeColumnBuilder AddNewDateTimeColumnBuilder(Guid id, string name, Action<DbDateTimeColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name);

        DbDateTimeColumnBuilder builder = new DbDateTimeColumnBuilder(id, name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DbColumnCollectionBuilder WithDateTimeColumn(string name, Action<DbDateTimeColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbDateTimeColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateTimeColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type DateTime and name '{name}' is not found.");

        action(builder);

        return this;
    }

    internal DbDateTimeColumnBuilder WithDateTimeColumnBuilder(string name, Action<DbDateTimeColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbDateTimeColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateTimeColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type DateTime and name '{name}' is not found.");

        if (action != null)
            action(builder);

        return builder;
    }

    #endregion

    #region <--- text --->

    public DbColumnCollectionBuilder AddNewTextColumn(string name, Action<DbTextColumnBuilder> action)
    {
        return AddNewTextColumn(Guid.NewGuid(), name, action);
    }

    internal DbTextColumnBuilder AddNewTextColumnBuilder(string name, Action<DbTextColumnBuilder> action = null)
    {
        return AddNewTextColumnBuilder(Guid.NewGuid(), name, action);
    }

    public DbColumnCollectionBuilder AddNewTextColumn(Guid id, string name, Action<DbTextColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);

        DbTextColumnBuilder builder = new DbTextColumnBuilder(id, name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbTextColumnBuilder AddNewTextColumnBuilder(Guid id, string name, Action<DbTextColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name);

        DbTextColumnBuilder builder = new DbTextColumnBuilder(id, name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DbColumnCollectionBuilder WithTextColumn(string name, Action<DbTextColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbTextColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbTextColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Text and name '{name}' is not found.");

        action(builder);

        return this;
    }

    internal DbTextColumnBuilder WithTextColumnBuilder(string name, Action<DbTextColumnBuilder> action = null)
    {
        _tableBuilder.ValidateColumnName(name, isNew: false);

        var builder = (DbTextColumnBuilder)_builders
            .SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbTextColumnBuilder));

        if (builder is null)
            throw new DbBuilderException($"Column of type Text and name '{name}' is not found.");

        if (action != null)
            action(builder);

        return builder;
    }

    #endregion

    #region <=== remove ===>

    public DbColumnCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);

        if (builder is null)
            throw new DbBuilderException($"Column with name '{name}' is not found.");

        _builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal DbColumnCollection Build()
    {
        var collection = new DbColumnCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion
}