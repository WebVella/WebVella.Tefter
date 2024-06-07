using System;
using System.Data;
using System.Data.Common;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

internal static class DbSqlGenerator
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

    public static string GenerateUpdateCleanupScript()
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

    public static string GenerateUpdateScript(DifferenceCollection differences)
    {
        if (differences == null)
            throw new ArgumentNullException(nameof(differences));

        if (differences.Count == 0)
            return string.Empty;

        StringBuilder stmsBuilder = new StringBuilder();

        foreach (var difference in differences)
            BuildStatement(stmsBuilder, difference);

        StringBuilder sb = new StringBuilder();

        sb.Append(PREPARE_SYSTEM_OBJECTS);

        sb.Append(FUNCTION_CREATE_WRAPPER.Replace("$$$FUNCTION_BODY$$$", stmsBuilder.ToString()));

        sb.Append(FUNCTION_EXECUTE);

        return sb.ToString();
    }

    private static void BuildStatement(StringBuilder commandsBuilder, Difference difference)
    {
        if (difference == null)
            throw new ArgumentNullException(nameof(difference));

        if (string.IsNullOrEmpty(difference.TableName))
            throw new DbSqlGeneratorException("Difference record does not have table name specified");

        if (string.IsNullOrEmpty(difference.ObjectName))
            throw new DbSqlGeneratorException("Difference record does not have object name specified");

        if (difference.Object is null)
            throw new DbSqlGeneratorException("Difference record have property Object equals to null");

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
                            throw new DbSqlGeneratorException($"Not supported difference object type " +
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
                            throw new DbSqlGeneratorException($"Not supported difference object type {difference.ObjectType}" +
                                $" for difference type {difference.Type}. Update is supported only for columns");
                    }
                }
                break;
            case DifferenceActionType.Remove:
                {
                    switch (difference.ObjectType)
                    {
                        case DifferenceObjectType.Table:
                            statement = GenerateDropTableStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Column:
                            statement = GenerateDropColumnStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Constraint:
                            statement = GenerateDropConstraintStatement(commandsBuilder, difference);
                            break;
                        case DifferenceObjectType.Index:
                            statement = GenerateDropIndexStatement(commandsBuilder, difference);
                            break;
                        default:
                            throw new DbSqlGeneratorException($"Not supported difference object type " +
                                $"{difference.ObjectType} for difference type {difference.Type}");
                    }
                }
                break;
            default:
                throw new DbSqlGeneratorException($"Not supported difference type {difference.Type}");
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
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType}" +
                $" but trying to generate create table statement for DbTable object");

        var dbTable = difference.Object as DbTable;

        if (dbTable == null)
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbTable instance, while trying to generate create table statement.");

        return $"CREATE TABLE \"{dbTable.Name}\"(); COMMENT ON TABLE \"{dbTable.Name}\" IS '{dbTable.GetMetaJson(DateTime.Now)}';";
    }

    private static string GenerateDropTableStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        if (difference.ObjectType != DifferenceObjectType.Table)
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but trying to generate drop table statement for DbTable object");

        var dbTable = difference.Object as DbTable;

        if (dbTable == null)
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbTable instance, while trying to generate drop table statement.");

        return $"DROP TABLE \"{difference.TableName}\";";
    }

    #endregion

    #region <--- column --->

    private static string GenerateCreateColumnStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbColumn dbColumn = difference.Object as DbColumn;

        if (dbColumn == null)
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
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
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
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
            throw new DbSqlGeneratorException("Trying to generate update statement for table id column. It's not supported.");
        };

        Func<DbAutoIncrementColumn, string> autoIncFunc = (column) =>
        {
            throw new DbSqlGeneratorException("Trying to generate update statement for auto increment column. It's not supported.");
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
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbColumn dbColumn = difference.Object as DbColumn;

        if (dbColumn == null)
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbColumn instance, while trying to generate drop column sql statement.");

        return $"ALTER TABLE \"{difference.TableName}\" DROP COLUMN \"{dbColumn.Name}\";";
    }

    #endregion

    #region <--- constraint --->

    private static string GenerateCreateConstraintStatement(
        StringBuilder commandsBuilder, Difference difference)
    {
        string tableName = difference.TableName;
        DbConstraint dbConstraint = difference.Object as DbConstraint;

        if (dbConstraint == null)
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
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
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
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
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
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
            throw new DbSqlGeneratorException($"Incorrect difference object type. Difference object type is {difference.ObjectType} " +
                $"but object is not DbIndex instance, while trying to generate drop index sql statement.");

        return $"DROP INDEX \"{index.Name}\";";
    }

    #endregion 

    #endregion
}
