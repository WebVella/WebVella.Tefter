namespace WebVella.Tefter.Database;

internal static class DbSqlProvider
{
    #region <=== GenerateCreateExtensionsScript ===>

    private const string CREATE_EXTENSIONS_SQL = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"; " +
                                                 "CREATE EXTENSION IF NOT EXISTS \"pg_trgm\";";

    public static string GenerateCreateExtensionsScript()
    {
        return CREATE_EXTENSIONS_SQL;
    }

    #endregion

    #region <=== GenerateUpdateCleanupScript ===>

    private const string UPDATE_CLEANUP_SQL = @"
DROP FUNCTION IF EXISTS _tefter_database_update_func;
DROP TABLE IF EXISTS _tefter_update_log;";

    public static string GetUpdateCleanupScript()
    {
        return UPDATE_CLEANUP_SQL;
    }

    #endregion

    #region <=== GenerateUpdateScript ===>

    #region <--- SQL --->

    private const string PREPARE_SYSTEM_OBJECTS = @"
DROP FUNCTION IF EXISTS _tefter_database_update_func;
DROP TABLE IF EXISTS _tefter_update_log;
CREATE TABLE _tefter_update_log (
	created_on TIMESTAMP WITH TIME ZONE DEFAULT clock_timestamp(),
	statement TEXT,
    success BOOLEAN,
	sql_error TEXT
);

";

    private const string FUNCTION_CREATE_WRAPPER = @"
CREATE OR REPLACE FUNCTION _tefter_database_update_func()
RETURNS SETOF _tefter_update_log AS $$
DECLARE
	error_occurred bool;
BEGIN
	error_occurred := 0;

	$$$FUNCTION_BODY$$$
	
	RETURN QUERY SELECT * FROM _tefter_update_log; 
END;
$$ LANGUAGE plpgsql;

";

    private const string FUNCTION_EXECUTE = @"SELECT * FROM _tefter_database_update_func();";

    private const string STATEMENT_WRAPPER = @"
	IF not error_occurred THEN
		BEGIN
		    ----- BEGIN STATEMENT ----
			$$$STATEMENT$$$
            ----- END STATEMENT ----

			INSERT INTO _tefter_update_log(statement,success,sql_error) VALUES('$$$STATEMENT_ENCODED$$$', TRUE, null );
		EXCEPTION WHEN OTHERS THEN
			INSERT INTO _tefter_update_log(statement,success,sql_error) VALUES('$$$STATEMENT_ENCODED$$$', FALSE, SQLERRM );
			error_occurred := 1;
		END;
	END IF;
";

    #endregion

    public static string GenerateUpdateScript(DifferenceCollection differences, bool cascade = true)
    {
        if (differences == null)
            throw new ArgumentNullException(nameof(differences));

        if (differences.Count == 0)
            return string.Empty;

        StringBuilder stmsBuilder = new StringBuilder();

        foreach (var difference in differences)
            BuildStatement(stmsBuilder, difference, cascade);

        StringBuilder sb = new StringBuilder();

        sb.Append(PREPARE_SYSTEM_OBJECTS);

        sb.Append(FUNCTION_CREATE_WRAPPER.Replace("$$$FUNCTION_BODY$$$", stmsBuilder.ToString()));

        sb.Append(FUNCTION_EXECUTE);

        return sb.ToString();
    }

    private static void BuildStatement(StringBuilder commandsBuilder, Difference difference, bool cascade = true)
    {
        if (difference == null)
            throw new ArgumentNullException(nameof(difference));

        if (string.IsNullOrEmpty(difference.TableName))
            throw new DbSqlProviderException("Difference record does not have table name specified");

        if (string.IsNullOrEmpty(difference.ObjectName))
            throw new DbSqlProviderException("Difference record does not have object name specified");

        if (difference.Object is null)
            throw new DbSqlProviderException("Difference record have property Object equals to null");

        string statement = string.Empty;

        switch (difference.Type)
        {
            case DifferenceActionType.Add:
                {
                    switch (difference.ObjectType)
                    {
                        case DifferenceObjectType.Table:
                            statement = GenerateCreateTableStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Column:
                            statement = GenerateCreateColumnStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Constraint:
                            statement = GenerateCreateConstraintStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Index:
                            statement = GenerateCreateIndexStatement(commandsBuilder, difference);
                            break;
                        default:
                            throw new DbSqlProviderException($"Not supported difference object type " +
                                $"{difference.ObjectType} for difference type {difference.Type}");
                    }
                }
                break;
            case DifferenceActionType.Update:
                {
                    switch (difference.ObjectType)
                    {
                        case DifferenceObjectType.Column:
                            statement = GenerateUpdateColumnStatement(commandsBuilder, difference);
                            break;
                        default:
                            throw new DbSqlProviderException($"Not supported difference object type {difference.ObjectType}" +
                                $" for difference type {difference.Type}. Update is supported only for columns");
                    }
                }
                break;
            case DifferenceActionType.Remove:
                {
                    switch (difference.ObjectType)
                    {
                        case DifferenceObjectType.Table:
                            statement = GenerateDropTableStatement(commandsBuilder, difference, cascade);
                            break;
                        case DifferenceObjectType.Column:
                            statement = GenerateDropColumnStatement(commandsBuilder, difference, cascade);
                            break;
                        case DifferenceObjectType.Constraint:
                            statement = GenerateDropConstraintStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Index:
                            statement = GenerateDropIndexStatement(commandsBuilder, difference);
                            break;
                        default:
                            throw new DbSqlProviderException($"Not supported difference object type " +
                                $"{difference.ObjectType} for difference type {difference.Type}");
                    }
                }
                break;
            default:
                throw new DbSqlProviderException($"Not supported difference type {difference.Type}");
        }

        string encodedStatement = statement.Replace("'", "''");

        commandsBuilder.Append(STATEMENT_WRAPPER
            .Replace("$$$STATEMENT$$$", statement)
            .Replace("$$$STATEMENT_ENCODED$$$", encodedStatement));
    }

    #region <--- table --->

    private static string GenerateCreateTableStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        if (difference.ObjectType != DifferenceObjectType.Table)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType}" +
                $" but trying to generate create table statement for DbTable object");

        var dbTable = difference.Object as DbTable;

        if (dbTable == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbTable instance, while trying to generate create table statement.");

        return $"CREATE TABLE \"{dbTable.Name}\"(); COMMENT ON TABLE \"{dbTable.Name}\" IS '{dbTable.GenerateMetaJson(DateTime.Now)}';";
    }

    private static string GenerateDropTableStatement(
        StringBuilder commandsBuilder, Difference difference, bool cascade = true)
    {
        if (difference.ObjectType != DifferenceObjectType.Table)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but trying to generate drop table statement for DbTable object");

        var dbTable = difference.Object as DbTable;

        if (dbTable == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbTable instance, while trying to generate drop table statement.");

        return $"DROP TABLE \"{difference.TableName}\"  {(cascade ? "CASCADE" : "")};";
    }

    #endregion

    #region <--- column --->

    private static string GenerateCreateColumnStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbColumn dbColumn = difference.Object as DbColumn;

        if (dbColumn == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbColumn instance, while trying to generate create column sql statement.");

        Func<DbColumn, string> generalFunc = (column) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"ALTER TABLE \"{tableName}\" ADD COLUMN \"{column.Name}\" {column.DatabaseColumnType}");
            if (!column.IsNullable)
                sb.Append(" NOT NULL");
            var defaultValue = DbUtility.ConvertDbColumnDefaultValueToDatabaseDefaultValue(column);
            if (defaultValue != null)
                sb.Append($" DEFAULT {defaultValue}");

            sb.Append(";");
            return sb.ToString();
        };

        Func<DbAutoIncrementColumn, string> autoIncFunc = (column) =>
        {
            return $"ALTER TABLE \"{tableName}\" ADD COLUMN \"{column.Name}\" {column.DatabaseColumnType};";
        };

        Func<DbIdColumn, string> tableIdFunc = (column) =>
        {
            return $"ALTER TABLE \"{tableName}\" ADD COLUMN \"{column.Name}\" UUID NOT NULL DEFAULT {Constants.DB_GUID_COLUMN_AUTO_DEFAULT_VALUE};";
        };

        string createSql = dbColumn switch
        {
            DbIdColumn c => tableIdFunc(c),
            DbAutoIncrementColumn c => autoIncFunc(c),
            DbNumberColumn c => generalFunc(c),
            DbBooleanColumn c => generalFunc(c),
            DbDateColumn c => generalFunc(c),
            DbDateTimeColumn c => generalFunc(c),
            DbGuidColumn c => generalFunc(c),
            DbTextColumn c => generalFunc(c),
            _ => throw new Exception($"Not supported DbColumn type {dbColumn.GetType()}")
        };

        return createSql + $" COMMENT ON COLUMN \"{difference.TableName}\".\"{dbColumn.Name}\" IS '{dbColumn.GetMetaJson(DateTime.Now)}';";

    }

    private static string GenerateUpdateColumnStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbColumn dbColumn = difference.Object as DbColumn;

        if (dbColumn == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbColumn instance, while trying to generate create column sql statement.");

        Func<DbColumn, string> generalFunc = (column) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{column.Name}\" {column.DatabaseColumnType}");
            if (!column.IsNullable)
                sb.Append(" NOT NULL");
            var defaultValue = DbUtility.ConvertDbColumnDefaultValueToDatabaseDefaultValue(column);
            if (defaultValue != null)
                sb.Append($" DEFAULT {defaultValue}");

            sb.Append(";");
            return sb.ToString();
        };

        Func<DbIdColumn, string> tableIdFunc = (column) =>
        {
            throw new DbSqlProviderException("Trying to generate update statement for table id column. It's not supported.");
        };

        Func<DbAutoIncrementColumn, string> autoIncFunc = (column) =>
        {
            throw new DbSqlProviderException("Trying to generate update statement for auto increment column. It's not supported.");
        };

        string createSql = dbColumn switch
        {
            DbIdColumn c => tableIdFunc(c),
            DbAutoIncrementColumn c => autoIncFunc(c),
            DbNumberColumn c => generalFunc(c),
            DbBooleanColumn c => generalFunc(c),
            DbDateColumn c => generalFunc(c),
            DbDateTimeColumn c => generalFunc(c),
            DbGuidColumn c => generalFunc(c),
            DbTextColumn c => generalFunc(c),
            _ => throw new Exception($"Not supported DbColumn type {dbColumn.GetType()}")
        };

        return createSql + $" COMMENT ON COLUMN \"{difference.TableName}\".\"{dbColumn.Name}\" IS '{dbColumn.GetMetaJson(DateTime.Now)}';";
    }

    private static string GenerateDropColumnStatement(
        StringBuilder commandsBuilder, Difference difference, bool cascade = true)
    {
        string tableName = difference.TableName;
        DbColumn dbColumn = difference.Object as DbColumn;

        if (dbColumn == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbColumn instance, while trying to generate drop column sql statement.");

        return $"ALTER TABLE \"{difference.TableName}\" DROP COLUMN \"{dbColumn.Name}\" {(cascade ? "CASCADE" : "")};";
    }

    #endregion

    #region <--- constraint --->

    private static string GenerateCreateConstraintStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbConstraint dbConstraint = difference.Object as DbConstraint;

        if (dbConstraint == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbIndex instance, while trying to generate create constraint sql statement.");

        Func<DbUniqueKeyConstraint, string> uniqueFunc = (constraint) =>
        {
            string columns = string.Join(",", constraint.Columns.Select(x => x));
            return $"ALTER TABLE \"{tableName}\" ADD CONSTRAINT  \"{constraint.Name}\" UNIQUE ({columns});";
        };

        Func<DbPrimaryKeyConstraint, string> primaryFunc = (constraint) =>
        {
            string columns = string.Join(",", constraint.Columns.Select(x => x));
            return $"ALTER TABLE \"{tableName}\" ADD CONSTRAINT \"{constraint.Name}\" PRIMARY KEY ({columns});";
        };

        Func<DbForeignKeyConstraint, string> fkFunc = (constraint) =>
        {
            string columns = string.Join(",", constraint.Columns.Select(x => x));
            string fkColumns = string.Join(",", constraint.ForeignColumns.Select(x => x));
            return $"ALTER TABLE \"{tableName}\" ADD CONSTRAINT \"{constraint.Name}\" FOREIGN KEY ({columns}) REFERENCES \"{constraint.ForeignTable}\" ({fkColumns}) ;";
        };

        return dbConstraint switch
        {
            DbUniqueKeyConstraint c => uniqueFunc(c),
            DbPrimaryKeyConstraint c => primaryFunc(c),
            DbForeignKeyConstraint c => fkFunc(c),
            _ => throw new Exception($"Not supported DbConstraint type {dbConstraint.GetType()}")
        };
    }
    private static string GenerateDropConstraintStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbConstraint dbConstraint = difference.Object as DbConstraint;

        if (dbConstraint == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbIndex instance, while trying to generate drop constraint sql statement.");


        return $"ALTER TABLE \"{tableName}\" DROP CONSTRAINT \"{dbConstraint.Name}\";";
    }

    #endregion

    #region <--- index --->

    private static string GenerateCreateIndexStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbIndex index = difference.Object as DbIndex;

        if (index == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbIndex instance, while trying to generate create index sql statement.");

        Func<DbBTreeIndex, string> btreeFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns);
            return $"CREATE INDEX \"{index.Name}\" ON \"{tableName}\" USING btree({columns});";
        };

        Func<DbGinIndex, string> ginFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns.Select(x => $"\"{x}\" gin_trgm_ops"));
            return $"CREATE INDEX \"{index.Name}\" ON \"{tableName}\" USING gin({columns});";
        };

        Func<DbGistIndex, string> gistFunc = (index) =>
        {
            string columns = string.Join(",", index.Columns.Select(x => $"\"{x}\" gist_trgm_ops"));
            return $"CREATE INDEX \"{index.Name}\" ON \"{tableName}\" USING gist({columns});";
        };

        Func<DbHashIndex, string> hashFunc = (index) =>
        {
            return $"CREATE INDEX \"{index.Name}\" ON \"{tableName}\" USING hash({index.Columns[0]});";
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

    private static string GenerateDropIndexStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbIndex index = difference.Object as DbIndex;

        if (index == null)
            throw new DbSqlProviderException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbIndex instance, while trying to generate drop index sql statement.");

        return $"DROP INDEX \"{index.Name}\";";
    }

    #endregion

    #endregion


    #region <=== GetTablesMetaSql ===>

    public static string GetTablesMetaSql()
    {
        return @"
SELECT t.table_name, pg_catalog.obj_description(pgc.oid, 'pg_class') as meta
FROM information_schema.tables t
    INNER JOIN pg_catalog.pg_class pgc ON t.table_name = pgc.relname 
WHERE t.table_type='BASE TABLE' AND t.table_schema='public'
ORDER BY t.table_name ASC;";
    }

    #endregion

    #region <=== GetColumnsMetaSql ===>

    public static string GetColumnsMetaSql()
    {
        return @"
SELECT table_name, column_name, ordinal_position, column_default, is_nullable, data_type,
    (
        SELECT pg_catalog.col_description(c.oid, cols.ordinal_position::int)
        FROM pg_catalog.pg_class c
        WHERE c.oid = (SELECT ('""' || cols.table_name || '""')::regclass::oid) AND c.relname = cols.table_name
    ) AS meta
FROM information_schema.columns cols
WHERE table_schema = 'public'
ORDER BY table_name, ordinal_position;";
    }

    #endregion

    #region <=== GetConstraintsMetaSql ===>

    public static string GetConstraintsMetaSql()
    {
        return @"
WITH fk_info AS (
	SELECT
    tc.constraint_name, 
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name 
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
    AND tc.table_schema='public' AND ccu.table_schema='public'
)
SELECT 
	t.relname AS table_name,
    ix.conname AS constraint_name, 
	a.attname AS column_name,
	ix.contype AS constraint_type, 
	array_position(ix.conkey, a.attnum) AS column_position,
	fk.foreign_table_name,
	fk.foreign_column_name
FROM pg_catalog.pg_class t
	JOIN pg_catalog.pg_attribute a 	ON t.oid    =  a.attrelid 
	JOIN pg_catalog.pg_constraint ix   ON t.oid    =  ix.conrelid  
	JOIN pg_catalog.pg_class i     	ON a.attnum = ANY(ix.conkey) AND i.oid=ix.conrelid 
	JOIN pg_catalog.pg_namespace n 	ON n.oid    = t.relnamespace
	LEFT JOIN fk_info fk						ON fk.constraint_name = ix.conname
WHERE t.relkind = 'r' AND n.nspname = 'public'
ORDER BY t.relname,ix.conname,array_position(ix.conkey, a.attnum);";
    }

    #endregion

    #region <=== GetConstraintsMetaSql ===>

    public static string GetIndexesMetaSql()
    {
        return @"
SELECT 
    t.relname AS table_name,
    i.relname AS index_name, 
    a.attname AS column_name,
    am.amname AS index_type, 
    1 + array_position(ix.indkey, a.attnum) AS column_position
FROM pg_catalog.pg_class t
    JOIN pg_catalog.pg_attribute a 	ON t.oid    = a.attrelid 
    JOIN pg_catalog.pg_index ix    	ON t.oid    = ix.indrelid
    JOIN pg_catalog.pg_class i     	ON a.attnum = ANY(ix.indkey) AND i.oid=ix.indexrelid 
    JOIN pg_catalog.pg_namespace n 	ON n.oid    = t.relnamespace
    JOIN pg_catalog.pg_am am		ON am.oid	= i.relam
WHERE t.relkind = 'r' AND n.nspname = 'public' AND ix.indisprimary = FALSE AND ix.indisunique = FALSE
ORDER BY t.relname,i.relname,array_position(ix.indkey, a.attnum);";
    }

    #endregion
}
