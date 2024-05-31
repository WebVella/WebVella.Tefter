namespace WebVella.Tefter.Database;

public class DbTable : DbObject
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

    public string Name { get; set; }
    public DbColumnCollection Columns { get; init; } = new();
    public DbConstraintCollection Constraints { get; init; } = new();
    public DbIndexCollection Indexes { get; init; } = new();

    #endregion

    #region <=== Columns Management ===>

    public DbAutoIncrementColumn AddAutoIncrementColumn(string name)
    {
        ValidateName(name);
        DbAutoIncrementColumn column = new DbAutoIncrementColumn
        {
            Table = this,
            Name = name,
        };
        Columns.Add(column);
        return column;
    }

    public DbNumberColumn AddNumberColumn(string name, bool isNullable = true, decimal? defaultValue = null)
    {
        ValidateName(name);
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

    public DbBooleanColumn AddBooleanColumn(string name, bool isNullable = true, bool? defaultValue = null)
    {
        ValidateName(name);
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

    public DbDateColumn AddDateColumn(string name, bool isNullable = true, DateOnly? defaultValue = null, bool useCurrentTimeAsDefaultValue = false)
    {
        ValidateName(name);
        DbDateColumn column = new DbDateColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            UseCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue
        };
        Columns.Add(column);
        return column;
    }

    public DbDateTimeColumn AddDateTimeColumn(string name, bool isNullable = true, DateTime? defaultValue = null, bool useCurrentTimeAsDefaultValue = false)
    {
        ValidateName(name);
        DbDateTimeColumn column = new DbDateTimeColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            UseCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue
        };
        Columns.Add(column);
        return column;
    }

    public void AddGuidColumn(string name, DbObjectType objectType, bool isNullable = true, Guid? defaultValue = null, bool generateNewIdAsDefaultValue = false)
    {
        ValidateName(name);
        DbColumn column = new DbGuidColumn
        {
            Table = this,
            Name = name,
            Meta = new DbObjectMeta { Type = objectType },
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            GenerateNewIdAsDefaultValue = generateNewIdAsDefaultValue
        };
        Columns.Add(column);
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

    private string ConvertDefaultValueToString(DbColumn column)
    {
        Func<DbNumberColumn, string> numberDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return column.DefaultValue.Value.ToString();
        };

        Func<DbBooleanColumn, string> booleanDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else if (column.DefaultValue.Value)
                return "TRUE";
            else
                return "FALSE";
        };

        Func<DbDateColumn, string> dateDefaultValueFunc = (column) =>
        {
            if (column.UseCurrentTimeAsDefaultValue)
                return "now()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue.Value.ToString("yyyy-MM-dd")}'";
        };

        Func<DbDateTimeColumn, string> dateTimeDefaultValueFunc = (column) =>
        {
            if (column.UseCurrentTimeAsDefaultValue)
                return "now()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";
        };

        Func<DbGuidColumn, string> guidDefaultValueFunc = (column) =>
        {
            if (column.GenerateNewIdAsDefaultValue)
                return "uuid_generate_v1()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue.Value}'";
        };

        Func<DbTextColumn, string> textDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue.Value}'";
        };

        Func<DbTableIdColumn, string> tableIdDefaultValueFunc = (column) =>
        {
            return "uuid_generate_v1()";
        };

        return column switch
        {
            DbTableIdColumn c => tableIdDefaultValueFunc(c),
            DbAutoIncrementColumn c => null,
            DbNumberColumn c => numberDefaultValueFunc(c),
            DbBooleanColumn c => booleanDefaultValueFunc(c),
            DbDateColumn c => dateDefaultValueFunc(c),
            DbDateTimeColumn c => dateTimeDefaultValueFunc(c),
            DbGuidColumn c => guidDefaultValueFunc(c),
            DbTextColumn c => textDefaultValueFunc(c),
            _ => throw new Exception($"Not supported DbColumn type {column.GetType()} while trying to extract default value")
        };
    }

    #endregion

    #region <=== Constraints Management ===>

    public void AddPrimaryKeyContraint(string name)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        var constraint = new DbPrimaryKeyConstraint { Name = name };
        var idColumn = Columns.Find("tefter_id");
        if (idColumn is null)
            throw new DbException($"Table id column is not found while try to create primary key constraint {name}");

        constraint.Columns.Add(idColumn);
        Constraints.Add(constraint);
    }

    public void AddUniqueContraint(string name, params string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var constraint = new DbUniqueConstraint { Name = name };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create unique constraint {name}");

            constraint.Columns.Add(column);
        }

        Constraints.Add(constraint);
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

    public void AddBTreeIndex(string name, string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbBTreeIndex { Name = name };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create btree index {name}");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
    }

    public void AddGistIndex(string name, string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbGistIndex { Name = name };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create GIST index {name}");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
    }

    public void AddGinIndex(string name, string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbGinIndex { Name = name };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create GIN index {name}");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
    }

    public void AddHashIndex(string name, string[] columns)
    {
        if (name is null)
            throw new ArgumentNullException(name);

        //TODO validate name, columns (1. at least one, 2. exists)

        var index = new DbHashIndex { Name = name };

        foreach (var columnName in columns)
        {
            var column = Columns.Find(columnName);
            if (column is null)
                throw new DbException($"Column with name {columnName} is not found while try to create HASH index {name}");

            index.Columns.Add(column);
        }

        Indexes.Add(index);
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

