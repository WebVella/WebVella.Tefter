namespace WebVella.Tefter.Database;

public class DbTable
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

    public Guid Id { get; internal set; }
    public string Name { get; set; }
    public DbColumnCollection Columns { get; init; } = new();

    public void AddAutoIncrementColumn(string name)
    {
        ValidateName(name);
        DbColumn column = new DbAutoIncrementColumn
        {
            Table = this,
            Name = name
        };
        Columns.Add(column);
    }

    public void AddNumberColumn(string name, bool isNullable = true, decimal? defaultValue = null)
    {
        ValidateName(name);
        DbColumn column = new DbNumberColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue
        };
        Columns.Add(column);
    }

    public void AddBooleanColumn(string name, bool isNullable = true, bool? defaultValue = null)
    {
        ValidateName(name);
        DbColumn column = new DbBooleanColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue
        };
        Columns.Add(column);
    }

    public void AddDateColumn(string name, bool isNullable = true, DateOnly? defaultValue = null, bool useCurrentTimeAsDefaultValue = false)
    {
        ValidateName(name);
        DbColumn column = new DbDateColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            UseCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue
        };
        Columns.Add(column);
    }

    public void AddDateTimeColumn(string name, bool isNullable = true, DateTime? defaultValue = null, bool useCurrentTimeAsDefaultValue = false)
    {
        ValidateName(name);
        DbColumn column = new DbDateTimeColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            UseCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue
        };
        Columns.Add(column);
    }

    public void AddGuidColumn(string name, bool isNullable = true, Guid? defaultValue = null, bool generateNewIdAsDefaultValue = false)
    {
        ValidateName(name);
        DbColumn column = new DbGuidColumn
        {
            Table = this,
            Name = name,
            IsNullable = isNullable,
            DefaultValue = defaultValue,
            GenerateNewIdAsDefaultValue = generateNewIdAsDefaultValue
        };
        Columns.Add(column);
    }

    internal void AddTableIdColumn()
    {
        DbColumn column = new DbTableIdColumn { Table = this };
        Columns.Add(column);
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
}

