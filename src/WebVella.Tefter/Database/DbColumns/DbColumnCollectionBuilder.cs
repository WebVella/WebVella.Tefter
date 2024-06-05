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

    #region <=== Add New Columns Methods ===>

    public DbColumnCollectionBuilder AddNewTableIdColumn()
    {
        var builder = (DbIdColumnBuilder)_builders
             .SingleOrDefault(x => x.GetType() == typeof(DbIdColumnBuilder));

        if (builder is not null)
            throw new DbBuilderException($"Column of type Id already exists in columns. Only one instance can be created.");

        _builders.Add(new DbIdColumnBuilder(true, _tableBuilder));
        return this;
    }

    public DbColumnCollectionBuilder AddNewAutoIncrementColumn(string name)
    {
        _tableBuilder.ValidateColumnName(name);
        DbAutoIncrementColumnBuilder builder = new DbAutoIncrementColumnBuilder(name, true, _tableBuilder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNewGuidColumn(string name, Action<DbGuidColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbGuidColumnBuilder builder = new DbGuidColumnBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNewNumberColumn(string name, Action<DbNumberColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbNumberColumnBuilder builder = new DbNumberColumnBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNewBooleanColumn(string name, Action<DbBooleanColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbBooleanColumnBuilder builder = new DbBooleanColumnBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNewDateColumn(string name, Action<DbDateColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbDateColumnBuilder builder = new DbDateColumnBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNewDateTimeColumn(string name, Action<DbDateTimeColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbDateTimeColumnBuilder builder = new DbDateTimeColumnBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNewTextColumn(string name, Action<DbTextColumnBuilder> action)
    {
        _tableBuilder.ValidateColumnName(name);
        DbTextColumnBuilder builder = new DbTextColumnBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    #endregion

    #region <=== Update Columns Methods ===>

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

    #endregion

    #region <=== Build and Remove Methods ===>

    public DbColumnCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Column with name '{name}' is not found.");

        _builders.Remove(builder);
        return this;
    }

    internal DbColumnCollection Build()
    {
        var collection = new DbColumnCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion

    #region <=== Internal Methods ==>

    //used for building new DbTable from existing instance

    internal DbColumnCollectionBuilder InternalAddExistingColumn(DbColumn column)
    {
        if (column is null)
            throw new ArgumentNullException(nameof(column));

        if (column is DbIdColumn)
            _builders.Add(new DbIdColumnBuilder((DbIdColumn)column, _tableBuilder));
        else if (column is DbAutoIncrementColumn)
            _builders.Add(new DbAutoIncrementColumnBuilder((DbAutoIncrementColumn)column, _tableBuilder));
        else if (column is DbGuidColumn)
            _builders.Add(new DbGuidColumnBuilder((DbGuidColumn)column, _tableBuilder));
        else if (column is DbBooleanColumn)
            _builders.Add(new DbBooleanColumnBuilder((DbBooleanColumn)column, _tableBuilder));
        else if (column is DbNumberColumn)
            _builders.Add(new DbNumberColumnBuilder((DbNumberColumn)column, _tableBuilder));
        else if (column is DbDateColumn)
            _builders.Add(new DbDateColumnBuilder((DbDateColumn)column, _tableBuilder));
        else if (column is DbDateTimeColumn)
            _builders.Add(new DbDateTimeColumnBuilder((DbDateTimeColumn)column, _tableBuilder));
        else if (column is DbTextColumn)
            _builders.Add(new DbTextColumnBuilder((DbTextColumn)column, _tableBuilder));
        else
            throw new DbBuilderException($"Not supported db column type {column.GetType()}");

        return this;
    }

    #endregion
}