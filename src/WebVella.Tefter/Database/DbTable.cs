using System;

namespace WebVella.Tefter.Database;

public class DbTable : DbObjectWithMeta
{
    //because the postgres NUMERIC type can hold a value of up to 131,072 digits
    //before the decimal point 16,383 digits after the decimal point.
    //we need to fit value in C# decimal which allow only 28 numbers,
    //we limit default precision to 28 and scale only to 8 numbers
    private const int DEFAULT_NUMBER_PRECISION = 28;
    private const int DEFAULT_NUMBER_SCALE = 8;

    //postgres name boundaries for length
    //maximum length is 63 but we reserve 13 for prefixes
    private const int MIN_COLUMN_NAME_LENGTH = 2;
    private const int MAX_COLUMN_NAME_LENGTH = 50;
    private const string NAME_VALIDATION_PATTERN = @"^[a-z](?!.*__)[a-z0-9_]*[a-z0-9]$";

    #region <=== Properties ===>

    public DbColumnCollection Columns { get; init; } = new();
    public DbConstraintCollection Constraints { get; init; } = new();
    public DbIndexCollection Indexes { get; init; } = new();

    #endregion

    #region <=== Columns Management ===>

    public DbAutoIncrementColumn AddAutoIncrementColumn(string name)
    {
        DbAutoIncrementColumn column = new DbAutoIncrementColumn
        {
            Table = this,
            Name = name,
        };
        Columns.Add(column);
        return column;
    }

    public DbNumberColumn AddNumberColumn(string name, bool isNullable, decimal? defaultValue = null)
    {
        DbNumberColumn column = new DbNumberColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue
        };
        Columns.Add(column);
        return column;
    }

    public DbBooleanColumn AddBooleanColumn(string name, bool isNullable, bool? defaultValue = null)
    {
        DbBooleanColumn column = new DbBooleanColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue
        };
        Columns.Add(column);
        return column;
    }

    public DbTextColumn AddTextColumn(string name, bool isNullable, string defaultValue = null)
    {
        DbTextColumn column = new DbTextColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue
        };
        Columns.Add(column);
        return column;
    }

    public DbDateColumn AddDateColumn(string name, bool isNullable, DateOnly? defaultValue)
    {
        DbDateColumn column = new DbDateColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            UseCurrentTimeAsDefaultValue = false
        };
        Columns.Add(column);
        return column;
    }

    public DbDateColumn AddDateColumn(string name, bool isNullable, bool useCurrentTimeAsDefaultValue)
    {
        DbDateColumn column = new DbDateColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = null,
            UseCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue
        };
        Columns.Add(column);
        return column;
    }

    public DbDateTimeColumn AddDateTimeColumn(string name, bool isNullable, DateTime? defaultValue)
    {
        //we remove milliseconds because, they are stored in database with different scale and
        //later when we restore default value there is always difference 
        var roundedDefaultValue = defaultValue;
        if (defaultValue is not null)
        {
            roundedDefaultValue = new DateTime(
                defaultValue.Value.Ticks - (defaultValue.Value.Ticks % TimeSpan.TicksPerSecond),
                defaultValue.Value.Kind
            );
        }

        DbDateTimeColumn column = new DbDateTimeColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = roundedDefaultValue,
            UseCurrentTimeAsDefaultValue = false
        };
        Columns.Add(column);
        return column;
    }

    public DbDateTimeColumn AddDateTimeColumn(string name, bool isNullable, bool useCurrentTimeAsDefaultValue)
    {
        DbDateTimeColumn column = new DbDateTimeColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = null,
            UseCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue
        };
        Columns.Add(column);
        return column;
    }

    public DbGuidColumn AddGuidColumn(string name, bool isNullable, Guid? defaultValue)
    {
        DbGuidColumn column = new DbGuidColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            GenerateNewIdAsDefaultValue = false
        };
        Columns.Add(column);
        return column;
    }

    public DbGuidColumn AddGuidColumn(string name, bool isNullable, bool generateNewIdAsDefaultValue)
    {
        DbGuidColumn column = new DbGuidColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = null,
            GenerateNewIdAsDefaultValue = generateNewIdAsDefaultValue
        };
        Columns.Add(column);
        return column;
    }

    internal DbTableIdColumn AddTableIdColumn()
    {
        DbTableIdColumn column = new DbTableIdColumn { Table = this };
        Columns.Add(column);
        return column;
    }

    public void RemoveColumn(string name)
    {
        var column = Columns.Find(name);

        if (column is null)
            throw new DbException($"Trying to remove non existent column '{name}' from table '{Name}'");

        Columns.Remove(column);
    }

    public void ClearColumns()
    {
        Columns.Clear();
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new DbException("Column name is required and cannot be empty");

        if (name.Length < MIN_COLUMN_NAME_LENGTH)
            throw new DbException($"The column name must be at least {MIN_COLUMN_NAME_LENGTH} characters long");

        if (name.Length > MAX_COLUMN_NAME_LENGTH)
            throw new DbException($"The length of Name must be less or equal than {MAX_COLUMN_NAME_LENGTH} characters");

        Match match = Regex.Match(name, NAME_VALIDATION_PATTERN);
        if (!match.Success || match.Value != name.Trim())
            throw new DbException($"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, not include spaces, not end with an underscore, and not contain two consecutive underscores");

        var column = Columns.Find(name.Trim());
        if (column is not null)
            throw new DbException($"A column with same name {name} already exists in table");
    }

    #endregion

    #region <=== Constraints Management ===>

    public DbPrimaryKeyConstraint AddPrimaryKeyContraint()
    {
        string name = $"primary_key_{this.Name}";
        var constraint = new DbPrimaryKeyConstraint { Name = name , Table = this };
        var idColumn = Columns.Find(Constants.DB_TABLE_ID_NAME);
        if (idColumn is null)
            throw new DbException($"Table id column is not found while try to create primary key constraint {name}");

        constraint.Columns.Add(idColumn);
        Constraints.Add(constraint);
        return constraint;
    }

    public DbUniqueConstraint AddUniqueContraint(string name, params string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var constraint = new DbUniqueConstraint { Name = name, Table = this };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create unique constraint {name}");

            constraint.Columns.Add(column);
        }

        Constraints.Add(constraint);
        return constraint;
    }

    public void RemoveConstraint(string name)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        var constraint = Constraints.Find(name);

        if (constraint is null)
            throw new DbException($"Trying to remove non existent constraint '{name}' from table '{Name}'");

        Constraints.Remove(constraint);
    }

    public void ClearConstraints()
    {
        Constraints.Clear();
    }

    #endregion

    #region <=== Index Management ===>

    public DbBTreeIndex AddBTreeIndex(string name, params string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbBTreeIndex { Name = name, Table = this };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create btree index {name}");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
        return index;
    }

    public DbGistIndex AddGistIndex(string name, params string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbGistIndex { Name = name, Table = this };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create GIST index {name}");
            if (column is not DbTextColumn)
                throw new DbException($"Column with name {columnName} is not a text column. GIST index can be created only on text columns");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
        return index;
    }

    public DbGinIndex AddGinIndex(string name, params string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbGinIndex { Name = name, Table = this };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create GIN index {name}");
            if (column is not DbTextColumn)
                throw new DbException($"Column with name {columnName} is not a text column. GIN index can be created only on text columns");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
        return index;
    }

    public DbHashIndex AddHashIndex(string name, string columnName)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbHashIndex { Name = name, Table = this };

        var column = Columns.Find(columnName);
        if (column is null)
            throw new DbException($"Column with name {columnName} is not found while try to create HASH index {name}");

        index.Columns.Add(column);

        Indexes.Add(index);
        return index;
    }

    public void RemoveIndex(string name)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        var index = Indexes.Find(name);

        if (index is null)
            throw new DbException($"Trying to remove non existent index '{name}' from table '{Name}'");

        Indexes.Remove(index);
    }

    public void ClearIndexes()
    {
        Indexes.Clear();
    }

    #endregion
}

