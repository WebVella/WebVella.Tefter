namespace WebVella.Tefter.Database;

public partial interface IDbManager
{
    void SaveTable(DbTable table);
}

public partial class DbManager : IDbManager
{
    private readonly IDbService _dbService;

    public DbManager(IServiceProvider serviceProvider)
    {
        _dbService = serviceProvider.GetService<IDbService>();
    }

    public void ValidateTable(DbTable table)
    {
    }

    public void SaveTable(DbTable table)
    {
        ValidateTable(table);

        using (var scope = _dbService.CreateTransactionScope())
        {
            var extensionSql = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"; " + 
                               "CREATE EXTENSION IF NOT EXISTS \"pg_trgm\";";
            _dbService.ExecuteSqlNonQueryCommand(extensionSql);

            var tableSql = $"CREATE TABLE \"{table.Name}\"();";
            _dbService.ExecuteSqlNonQueryCommand(tableSql);

            var tableMeta = GetSqlMeta(table);
            _dbService.ExecuteSqlNonQueryCommand($"COMMENT ON TABLE \"{table.Name}\" IS '{tableMeta}'");

            foreach (DbColumn column in table.Columns)
            {
                var sql = GetSqlColumnCreate(column);
                _dbService.ExecuteSqlNonQueryCommand(sql);

                var meta = GetSqlMeta(column);
                _dbService.ExecuteSqlNonQueryCommand($"COMMENT ON COLUMN \"{table.Name}\".\"{column.Name}\" IS '{meta}'");
            }

            foreach (DbConstraint constraint in table.Constraints)
            {
                var sql = GetSqlConstraintCreate(constraint);
                _dbService.ExecuteSqlNonQueryCommand(sql);

                var meta = GetSqlMeta(constraint);
                _dbService.ExecuteSqlNonQueryCommand($"COMMENT ON CONSTRAINT \"{constraint.Name}\" ON \"{table.Name}\" IS '{meta}'");
            }

            foreach (DbIndex index in table.Indexes)
            {
                var sql = GetSqlIndexCreate(index);
                _dbService.ExecuteSqlNonQueryCommand(sql);

                var meta = GetSqlMeta(index);
                _dbService.ExecuteSqlNonQueryCommand($"COMMENT ON INDEX \"{index.Name}\" IS '{meta}'");
            }

            scope.Complete();
        }
    }

    private string GetSqlMeta(DbObject obj)
    {
        return JsonSerializer.Serialize(obj.Meta);
    }
    
    private string GetSqlColumnCreate(DbColumn column)
    {
        Func<DbColumn, string, string> generalFunc = (column, pgType) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"ALTER TABLE \"{column.Table.Name}\" ADD COLUMN \"{column.Name}\" {pgType}");
            if (!column.IsNullable)
                sb.Append(" NOT NULL");
            var defaultValue = GetSqlColumnDefaultValue(column);
            if (defaultValue != null)
                sb.Append($" DEFAULT {defaultValue}");

            sb.Append(";");
            return sb.ToString();
        };

        Func<DbAutoIncrementColumn, string> autoIncFunc = (column) =>
        {
            return $"ALTER TABLE \"{column.Table.Name}\" ADD COLUMN \"{column.Name}\" SERIAL;";
        };

        Func<DbNumberColumn, string> numberFunc = (column) =>
        {
            return generalFunc(column, "NUMERIC");
        };

        Func<DbBooleanColumn, string> booleanFunc = (column) =>
        {
            return generalFunc(column, "BOOLEAN");
        };

        Func<DbDateColumn, string> dateFunc = (column) =>
        {
            return generalFunc(column, "DATE");
        };

        Func<DbDateTimeColumn, string> dateTimeFunc = (column) =>
        {
            return generalFunc(column, "TIMESTAMPTZ");
        };

        Func<DbGuidColumn, string> guidFunc = (column) =>
        {
            return generalFunc(column, "UUID");
        };

        Func<DbTextColumn, string> textFunc = (column) =>
        {
            return generalFunc(column, "TEXT");
        };

        Func<DbTableIdColumn, string> tableIdFunc = (column) =>
        {
            return $"ALTER TABLE \"{column.Table.Name}\" ADD COLUMN \"{column.Name}\" UUID NOT NULL DEFAULT uuid_generate_v1();";
        };

        return column switch
        {
            DbTableIdColumn c => tableIdFunc(c),
            DbAutoIncrementColumn c => autoIncFunc(c),
            DbNumberColumn c => numberFunc(c),
            DbBooleanColumn c => booleanFunc(c),
            DbDateColumn c => dateFunc(c),
            DbDateTimeColumn c => dateTimeFunc(c),
            DbGuidColumn c => guidFunc(c),
            DbTextColumn c => textFunc(c),
            _ => throw new Exception($"Not supported DbColumn type {column.GetType()}")
        };
    }

    private string GetSqlColumnDefaultValue(DbColumn column)
    {
        Func<DbNumberColumn, string> numberDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return ((decimal)column.DefaultValue).ToString();
        };

        Func<DbBooleanColumn, string> booleanDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else if ((bool)column.DefaultValue)
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
                return $"'{((DateOnly)column.DefaultValue).ToString("yyyy-MM-dd")}::date'";
        };

        Func<DbDateTimeColumn, string> dateTimeDefaultValueFunc = (column) =>
        {
            if (column.UseCurrentTimeAsDefaultValue)
                return "now()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{((DateTime)column.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss")}'::timestamp with time zone";
        };

        Func<DbGuidColumn, string> guidDefaultValueFunc = (column) =>
        {
            if (column.GenerateNewIdAsDefaultValue)
                return "uuid_generate_v1()";

            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{(Guid)column.DefaultValue}'";
        };

        Func<DbTextColumn, string> textDefaultValueFunc = (column) =>
        {
            if (column.DefaultValue is null)
                return "NULL";
            else
                return $"'{column.DefaultValue}'::text";
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

    private string GetSqlConstraintCreate(DbConstraint constraint)
    {
        Func<DbUniqueConstraint, string> uniqueFunc = (constraint) =>
        {
            string columns = string.Join(",", constraint.Columns.Select(x => x.Name));
            return $"ALTER TABLE \"{constraint.Table.Name}\" ADD CONSTRAINT  \"{constraint.Name}\" UNIQUE ({columns});";
        };

        Func<DbPrimaryKeyConstraint, string> primaryFunc = (constraint) =>
        {
            string columns = string.Join(",", constraint.Columns.Select(x => x.Name));
            return $"ALTER TABLE \"{constraint.Table.Name}\" ADD CONSTRAINT \"{constraint.Name}\" PRIMARY KEY ({columns});";
        };

        return constraint switch
        {
            DbUniqueConstraint c => uniqueFunc(c),
            DbPrimaryKeyConstraint c => primaryFunc(c),
            _ => throw new Exception($"Not supported DbConstraint type {constraint.GetType()}")
        };
    }

    private string GetSqlIndexCreate(DbIndex index)
    {
        Func<DbBTreeIndex, string> btreeFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns.Select(x => x.Name ));
            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING btree({columns});";
        };

        Func<DbGinIndex, string> ginFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns.Select(x => $"\"{x.Name}\" gin_trgm_ops"));
            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING gin({columns});";
        };

        Func<DbGistIndex, string> gistFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns.Select(x => $"\"{x.Name}\" gist_trgm_ops"));
            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING gist({columns});";
        };

        Func<DbHashIndex, string> hashFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns.Select(x => x.Name));
            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING hash({columns});";
        };

        return index switch
        {
            DbBTreeIndex c => btreeFunc(c),
            DbGinIndex c => ginFunc(c),
            DbGistIndex c => gistFunc(c),
            DbHashIndex c => hashFunc(c),
            _ => throw new Exception($"Not supported DbConstraint type {index.GetType()}")
        };
    }




}
