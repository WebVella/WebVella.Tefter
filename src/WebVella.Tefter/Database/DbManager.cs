using System.Data.Common;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

public partial interface IDbManager
{
}

public partial class DbManager : IDbManager
{
    private readonly IDbService _dbService;

    public DbManager(IServiceProvider serviceProvider)
    {
        _dbService = serviceProvider.GetService<IDbService>();
        LoadTables();
    }

    private void LoadTables()
    {
        List<DbTable> tables = new List<DbTable>();
        using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
        {
            const string tableSql = @"
SELECT t.table_name, pg_catalog.obj_description(pgc.oid, 'pg_class') as meta
FROM information_schema.tables t
    INNER JOIN pg_catalog.pg_class pgc ON t.table_name = pgc.relname 
WHERE t.table_type='BASE TABLE' AND t.table_schema='public'
ORDER BY t.table_name ASC;";
            DataTable dtTables = _dbService.ExecuteSqlQueryCommand(tableSql);
            foreach (DataRow row in dtTables.Rows)
            {
                DbTableMeta meta = null;
                //ignore tables with comments cant deserialize to meta object 
                try { meta = JsonSerializer.Deserialize<DbTableMeta>((string)row["meta"]); } catch { continue; }
                tables.Add(new DbTable
                {
                    Name = (string)row["table_name"],
                    Id = meta.Id,
                    ApplicationId = meta.ApplicationId,
                    DataProviderId = meta.DataProviderId
                });
            }

            const string columnsSql = @"
SELECT table_name, column_name, ordinal_position, column_default, is_nullable, data_type,
    (
        SELECT pg_catalog.col_description(c.oid, cols.ordinal_position::int)
        FROM pg_catalog.pg_class c
        WHERE c.oid = (SELECT ('""' || cols.table_name || '""')::regclass::oid) AND c.relname = cols.table_name
    ) AS meta
FROM information_schema.columns cols
WHERE table_schema = 'public'
ORDER BY table_name, ordinal_position;";
            DataTable dtColumns = _dbService.ExecuteSqlQueryCommand(columnsSql);
            foreach (DataRow row in dtColumns.Rows)
            {
                var tableName = (string)row["table_name"];
                DbTable table = tables.SingleOrDefault(x => x.Name == tableName);
                if (table is null)
                    continue;

                var columnName = (string)row["column_name"];
                var defaultValue = (string)row["column_default"];
                var isNullable = ((string)row["is_nullable"]).ToLower() == "yes";
                var dbType = (string)row["data_type"];

                //CreateDbColumnDatabaseInfo(table, columnName, dbType, isNullable, defaultValue, meta);

            }

            #region <--- Constraints --->

            Dictionary<string, string> constraintTableDict = new Dictionary<string, string>();
            Dictionary<string, List<string>> constraintColumnsDict = new Dictionary<string, List<string>>();
            Dictionary<string, char> constraintTypeDict = new Dictionary<string, char>();
            const string constraintsSql = @"
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
            DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(constraintsSql);
            foreach (DataRow row in dtConstraints.Rows)
            {
                var constraintName = (string)row["constraint_name"];
                if (!constraintTableDict.ContainsKey(constraintName))
                    constraintTableDict[constraintName] = (string)row["table_name"];
                if (!constraintColumnsDict.ContainsKey(constraintName))
                    constraintColumnsDict[constraintName] = new List<string>();
                if (!constraintTypeDict.ContainsKey(constraintName))
                    constraintTypeDict[constraintName] = (char)row["constraint_type"];

                constraintColumnsDict[constraintName].Add((string)row["column_name"]);
            }

            foreach (var constraintName in constraintTableDict.Keys)
            {
                var tableName = constraintTableDict[constraintName];
                DbTable table = tables.SingleOrDefault(x => x.Name == tableName);
                if (table is null)
                    continue;

                //TODO f
                switch (constraintTypeDict[constraintName])
                {
                    case 'p':
                        {
                            var constraint = new DbPrimaryKeyConstraint { Name = constraintName };
                            var columns = constraintColumnsDict[constraintName];
                            foreach (var column in columns)
                            {
                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
                                if (dbColumn is null) continue;
                                constraint.AddColumn(dbColumn.Name);
                            }
                            table.Constraints.Add(constraint);
                        }
                        break;
                    case 'u':
                        {
                            var constraint = new DbUniqueKeyConstraint { Name = constraintName };
                            var columns = constraintColumnsDict[constraintName];
                            foreach (var column in columns)
                            {
                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
                                if (dbColumn is null) continue;
                                constraint.AddColumn(dbColumn.Name);
                            }
                            table.Constraints.Add(constraint);
                        }
                        break;
                    default:
                        continue;
                }
            } 
            #endregion


            #region <--- Indexes --->

            Dictionary<string, string> indexTableDict = new Dictionary<string, string>();
            Dictionary<string, List<string>> indexColumnsDict = new Dictionary<string, List<string>>();
            Dictionary<string, string> indexTypeDict = new Dictionary<string, string>();
            const string indexesSql = @"
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
            DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(indexesSql);
            foreach (DataRow row in dtIndexes.Rows)
            {
                var indexName = (string)row["index_name"];
                if (!indexTableDict.ContainsKey(indexName))
                    indexTableDict[indexName] = (string)row["table_name"];
                if (!indexColumnsDict.ContainsKey(indexName))
                    indexColumnsDict[indexName] = new List<string>();
                if (!indexTypeDict.ContainsKey(indexName))
                    indexTypeDict[indexName] = (string)row["index_type"];

                indexColumnsDict[indexName].Add((string)row["column_name"]);
            }

            foreach (var indexName in indexTableDict.Keys)
            {
                var tableName = indexTableDict[indexName];
                DbTable table = tables.SingleOrDefault(x => x.Name == tableName);
                if (table is null)
                    continue;

                switch (indexTypeDict[indexName])
                {
                    case "btree":
                        {
                            var index = new DbBTreeIndex { Name = indexName };
                            foreach (var column in indexColumnsDict[indexName])
                            {
                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
                                if (dbColumn is null) continue;
                                index.AddColumn(dbColumn.Name);
                            }
                            table.Indexes.Add(index);
                        }
                        break;
                    case "gin":
                        {
                            var index = new DbGinIndex { Name = indexName };
                            foreach (var column in indexColumnsDict[indexName])
                            {
                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
                                if (dbColumn is null) continue;
                                index.AddColumn(dbColumn.Name);
                            }
                            table.Indexes.Add(index);
                        }
                        break;
                    case "gist":
                        {
                            var index = new DbGistIndex { Name = indexName };
                            foreach (var column in indexColumnsDict[indexName])
                            {
                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
                                if (dbColumn is null) continue;
                                index.AddColumn(dbColumn.Name);
                            }
                            table.Indexes.Add(index);
                        }
                        break;
                    case "hash":
                        {
                            var dbColumn = table.Columns.SingleOrDefault(x => x.Name == indexColumnsDict[indexName][0]);
                            if (dbColumn is null) continue;
                            var index = new DbHashIndex { Name = indexName };
                            index.AddColumn(dbColumn.Name);
                            table.Indexes.Add(index);
                        }
                        break;
                    default:
                        continue;
                }
            } 

            #endregion
        }

    }

    #region < OLD CODE >
    //    public void ValidateTable(DbTable table)
    //    {
    //    }

    //    #region <--- Save Table --->

    //    public void SaveTable(DbTable table)
    //    {
    //        ValidateTable(table);

    //        using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
    //        {
    //            var extensionSql = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"; " +
    //                               "CREATE EXTENSION IF NOT EXISTS \"pg_trgm\";";
    //            _dbService.ExecuteSqlNonQueryCommand(extensionSql);

    //            var tableSql = $"CREATE TABLE \"{table.Name}\"();";
    //            _dbService.ExecuteSqlNonQueryCommand(tableSql);
    //            _dbService.ExecuteSqlNonQueryCommand($"COMMENT ON TABLE \"{table.Name}\" IS '{table.GetMetaJson()}'");

    //            foreach (DbColumn column in table.Columns)
    //            {
    //                var sql = GetSqlColumnCreate(column);
    //                _dbService.ExecuteSqlNonQueryCommand(sql);
    //                _dbService.ExecuteSqlNonQueryCommand($"COMMENT ON COLUMN \"{table.Name}\".\"{column.Name}\" IS '{column.GetMetaJson()}'");
    //            }

    //            foreach (DbConstraint constraint in table.Constraints)
    //            {
    //                var sql = GetSqlConstraintCreate(constraint);
    //                _dbService.ExecuteSqlNonQueryCommand(sql);
    //            }

    //            foreach (DbIndex index in table.Indexes)
    //            {
    //                var sql = GetSqlIndexCreate(index);
    //                _dbService.ExecuteSqlNonQueryCommand(sql);
    //            }

    //            scope.Complete();
    //        }
    //    }

    //    private string GetSqlColumnCreate(DbColumn column)
    //    {
    //        Func<DbColumn, string, string> generalFunc = (column, pgType) =>
    //        {
    //            StringBuilder sb = new StringBuilder();
    //            sb.Append($"ALTER TABLE \"{column.Table.Name}\" ADD COLUMN \"{column.Name}\" {pgType}");
    //            if (!column.IsNullable)
    //                sb.Append(" NOT NULL");
    //            var defaultValue = DbUtility.ConvertDbColumnDefaultValueToDatabaseDefaultValue(column);
    //            if (defaultValue != null)
    //                sb.Append($" DEFAULT {defaultValue}");

    //            sb.Append(";");
    //            return sb.ToString();
    //        };

    //        Func<DbAutoIncrementColumn, string> autoIncFunc = (column) =>
    //        {
    //            return $"ALTER TABLE \"{column.Table.Name}\" ADD COLUMN \"{column.Name}\" SERIAL;";
    //        };

    //        Func<DbNumberColumn, string> numberFunc = (column) =>
    //        {
    //            return generalFunc(column, "NUMERIC");
    //        };

    //        Func<DbBooleanColumn, string> booleanFunc = (column) =>
    //        {
    //            return generalFunc(column, "BOOLEAN");
    //        };

    //        Func<DbDateColumn, string> dateFunc = (column) =>
    //        {
    //            return generalFunc(column, "DATE");
    //        };

    //        Func<DbDateTimeColumn, string> dateTimeFunc = (column) =>
    //        {
    //            return generalFunc(column, "TIMESTAMPTZ");
    //        };

    //        Func<DbGuidColumn, string> guidFunc = (column) =>
    //        {
    //            return generalFunc(column, "UUID");
    //        };

    //        Func<DbTextColumn, string> textFunc = (column) =>
    //        {
    //            return generalFunc(column, "TEXT");
    //        };

    //        Func<DbIdColumn, string> tableIdFunc = (column) =>
    //        {
    //            return $"ALTER TABLE \"{column.Table.Name}\" ADD COLUMN \"{column.Name}\" UUID NOT NULL DEFAULT uuid_generate_v1();";
    //        };

    //        return column switch
    //        {
    //            DbIdColumn c => tableIdFunc(c),
    //            DbAutoIncrementColumn c => autoIncFunc(c),
    //            DbNumberColumn c => numberFunc(c),
    //            DbBooleanColumn c => booleanFunc(c),
    //            DbDateColumn c => dateFunc(c),
    //            DbDateTimeColumn c => dateTimeFunc(c),
    //            DbGuidColumn c => guidFunc(c),
    //            DbTextColumn c => textFunc(c),
    //            _ => throw new Exception($"Not supported DbColumn type {column.GetType()}")
    //        };
    //    }

    //    private string GetSqlConstraintCreate(DbConstraint constraint)
    //    {
    //        Func<DbUniqueConstraint, string> uniqueFunc = (constraint) =>
    //        {
    //            string columns = string.Join(",", constraint.Columns.Select(x => x.Name));
    //            return $"ALTER TABLE \"{constraint.Table.Name}\" ADD CONSTRAINT  \"{constraint.Name}\" UNIQUE ({columns});";
    //        };

    //        Func<DbPrimaryKeyConstraint, string> primaryFunc = (constraint) =>
    //        {
    //            string columns = string.Join(",", constraint.Columns.Select(x => x.Name));
    //            return $"ALTER TABLE \"{constraint.Table.Name}\" ADD CONSTRAINT \"{constraint.Name}\" PRIMARY KEY ({columns});";
    //        };

    //        return constraint switch
    //        {
    //            DbUniqueConstraint c => uniqueFunc(c),
    //            DbPrimaryKeyConstraint c => primaryFunc(c),
    //            _ => throw new Exception($"Not supported DbConstraint type {constraint.GetType()}")
    //        };
    //    }

    //    private string GetSqlIndexCreate(DbIndex index)
    //    {
    //        Func<DbBTreeIndex, string> btreeFunc = (index) =>
    //        {
    //            string columns = string.Join(",", index.Columns.Select(x => x.Name));
    //            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING btree({columns});";
    //        };

    //        Func<DbGinIndex, string> ginFunc = (index) =>
    //        {
    //            string columns = string.Join(",", index.Columns.Select(x => $"\"{x.Name}\" gin_trgm_ops"));
    //            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING gin({columns});";
    //        };

    //        Func<DbGistIndex, string> gistFunc = (index) =>
    //        {
    //            string columns = string.Join(",", index.Columns.Select(x => $"\"{x.Name}\" gist_trgm_ops"));
    //            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING gist({columns});";
    //        };

    //        Func<DbHashIndex, string> hashFunc = (index) =>
    //        {
    //            string columns = string.Join(",", index.Columns.Select(x => x.Name));
    //            return $"CREATE INDEX \"{index.Name}\" ON \"{index.Table.Name}\" USING hash({columns});";
    //        };

    //        return index switch
    //        {
    //            DbBTreeIndex c => btreeFunc(c),
    //            DbGinIndex c => ginFunc(c),
    //            DbGistIndex c => gistFunc(c),
    //            DbHashIndex c => hashFunc(c),
    //            _ => throw new Exception($"Not supported DbConstraint type {index.GetType()}")
    //        };
    //    }

    //    #endregion

    //    public List<DbTable> LoadTables()
    //    {
    //        List<DbTable> tables = new List<DbTable>();
    //        using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
    //        {
    //            const string tableSql = @"
    //SELECT t.table_name, pg_catalog.obj_description(pgc.oid, 'pg_class') as meta
    //FROM information_schema.tables t
    //	INNER JOIN pg_catalog.pg_class pgc ON t.table_name = pgc.relname 
    //WHERE t.table_type='BASE TABLE' AND t.table_schema='public'
    //ORDER BY t.table_name ASC;";
    //            DataTable dtTables = _dbService.ExecuteSqlQueryCommand(tableSql);
    //            foreach (DataRow row in dtTables.Rows)
    //            {
    //                DbObjectMeta meta = null;
    //                //ignore tables with comments cant deserialize to meta object 
    //                try { meta = DbObjectWithMeta.GetMetaFromJson((string)row["meta"]); } catch { continue; }
    //                tables.Add(new DbTable
    //                {
    //                    Name = (string)row["table_name"],
    //                    Id = meta.Id,
    //                    ApplicationId = meta.ApplicationId,
    //                    DataProviderId = meta.DataProviderId
    //                });
    //            }


    //            const string columnsSql = @"
    //SELECT table_name, column_name, ordinal_position, column_default, is_nullable, data_type,
    //	(
    //        SELECT pg_catalog.col_description(c.oid, cols.ordinal_position::int)
    //        FROM pg_catalog.pg_class c
    //        WHERE c.oid = (SELECT ('""' || cols.table_name || '""')::regclass::oid) AND c.relname = cols.table_name
    //    ) AS meta
    //FROM information_schema.columns cols
    //WHERE table_schema = 'public'
    //ORDER BY table_name, ordinal_position;";
    //            DataTable dtColumns = _dbService.ExecuteSqlQueryCommand(columnsSql);
    //            foreach (DataRow row in dtColumns.Rows)
    //            {
    //                var tableName = (string)row["table_name"];
    //                DbTable table = tables.SingleOrDefault(x => x.Name == tableName);
    //                if (table is null)
    //                    continue;

    //                DbObjectMeta meta = null;
    //                //ignore columns with comments cant deserialize to meta object 
    //                try { meta = DbObjectWithMeta.GetMetaFromJson((string)row["meta"]); } catch { continue; }

    //                var columnName = (string)row["column_name"];
    //                var defaultValue = (string)row["column_default"];
    //                var isNullable = ((string)row["is_nullable"]).ToLower() == "yes";
    //                var dbType = (string)row["data_type"];

    //                CreateDbColumnDatabaseInfo(table, columnName, dbType, isNullable, defaultValue, meta);

    //            }

    //            Dictionary<string, string> constraintTableDict = new Dictionary<string, string>();
    //            Dictionary<string, List<string>> constraintColumnsDict = new Dictionary<string, List<string>>();
    //            Dictionary<string, char> constraintTypeDict = new Dictionary<string, char>();
    //            const string constraintsSql = @"
    //select 
    //	t.relname as table_name,
    //    ix.conname as constraint_name, 
    //	a.attname as column_name,
    //	ix.contype as constraint_type, 
    //	array_position(ix.conkey, a.attnum) as column_position
    //from pg_catalog.pg_class t
    //	join pg_catalog.pg_attribute a 	ON t.oid    =  a.attrelid 
    //	join pg_catalog.pg_constraint ix   ON t.oid    =  ix.conrelid  
    //	join pg_catalog.pg_class i     	ON a.attnum = ANY(ix.conkey) AND i.oid=ix.conrelid 
    //	join pg_catalog.pg_namespace n 	ON n.oid    = t.relnamespace
    //where t.relkind = 'r' AND n.nspname = 'public'
    //order by t.relname,ix.conname,array_position(ix.conkey, a.attnum);";
    //            DataTable dtConstraints = _dbService.ExecuteSqlQueryCommand(constraintsSql);
    //            foreach (DataRow row in dtConstraints.Rows)
    //            {
    //                var constraintName = (string)row["constraint_name"];
    //                if (!constraintTableDict.ContainsKey(constraintName))
    //                    constraintTableDict[constraintName] = (string)row["table_name"];
    //                if (!constraintColumnsDict.ContainsKey(constraintName))
    //                    constraintColumnsDict[constraintName] = new List<string>();
    //                if (!constraintTypeDict.ContainsKey(constraintName))
    //                    constraintTypeDict[constraintName] = (char)row["constraint_type"];

    //                constraintColumnsDict[constraintName].Add((string)row["column_name"]);
    //            }

    //            foreach (var constraintName in constraintTableDict.Keys)
    //            {
    //                var tableName = constraintTableDict[constraintName];
    //                DbTable table = tables.SingleOrDefault(x => x.Name == tableName);
    //                if (table is null)
    //                    continue;

    //                switch (constraintTypeDict[constraintName])
    //                {
    //                    case 'p':
    //                        {
    //                            var constraint = new DbPrimaryKeyConstraint { Name = constraintName, Table = table };
    //                            var columns = constraintColumnsDict[constraintName];
    //                            foreach (var column in columns)
    //                            {
    //                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
    //                                if (dbColumn is null) continue;
    //                                constraint.Columns.Add(dbColumn);
    //                            }
    //                            table.Constraints.Add(constraint);
    //                        }
    //                        break;
    //                    case 'u':
    //                        {
    //                            var constraint = new DbUniqueConstraint { Name = constraintName, Table = table };
    //                            var columns = constraintColumnsDict[constraintName];
    //                            foreach (var column in columns)
    //                            {
    //                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
    //                                if (dbColumn is null) continue;
    //                                constraint.Columns.Add(dbColumn);
    //                            }
    //                            table.Constraints.Add(constraint);
    //                        }
    //                        break;
    //                    default:
    //                        continue;
    //                }
    //            }

    //            Dictionary<string, string> indexTableDict = new Dictionary<string, string>();
    //            Dictionary<string, List<string>> indexColumnsDict = new Dictionary<string, List<string>>();
    //            Dictionary<string, string> indexTypeDict = new Dictionary<string, string>();
    //            const string indexesSql = @"
    //select 
    //	t.relname as table_name,
    //    i.relname as index_name, 
    //	a.attname as column_name,
    //	am.amname as index_type, 
    //	1 + array_position(ix.indkey, a.attnum) as column_position
    //from pg_catalog.pg_class t
    //	join pg_catalog.pg_attribute a 	ON t.oid    = a.attrelid 
    //	join pg_catalog.pg_index ix    	ON t.oid    = ix.indrelid
    //	join pg_catalog.pg_class i     	ON a.attnum = ANY(ix.indkey) AND i.oid=ix.indexrelid 
    //	join pg_catalog.pg_namespace n 	ON n.oid    = t.relnamespace
    //	JOIN pg_catalog.pg_am am			ON am.oid	= i.relam
    //where t.relkind = 'r' AND n.nspname = 'public' AND ix.indisprimary = FALSE AND ix.indisunique = FALSE
    //order by t.relname,i.relname,array_position(ix.indkey, a.attnum);";
    //            DataTable dtIndexes = _dbService.ExecuteSqlQueryCommand(indexesSql);
    //            foreach (DataRow row in dtIndexes.Rows)
    //            {
    //                var indexName = (string)row["index_name"];
    //                if (!indexTableDict.ContainsKey(indexName))
    //                    indexTableDict[indexName] = (string)row["table_name"];
    //                if (!indexColumnsDict.ContainsKey(indexName))
    //                    indexColumnsDict[indexName] = new List<string>();
    //                if (!indexTypeDict.ContainsKey(indexName))
    //                    indexTypeDict[indexName] = (string)row["index_type"];

    //                indexColumnsDict[indexName].Add((string)row["column_name"]);
    //            }

    //            foreach (var indexName in indexTableDict.Keys)
    //            {
    //                var tableName = indexTableDict[indexName];
    //                DbTable table = tables.SingleOrDefault(x => x.Name == tableName);
    //                if (table is null)
    //                    continue;

    //                switch (indexTypeDict[indexName])
    //                {
    //                    case "btree":
    //                        {
    //                            var index = new DbBTreeIndex { Name = indexName, Table = table };
    //                            foreach (var column in indexColumnsDict[indexName])
    //                            {
    //                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
    //                                if (dbColumn is null) continue;
    //                                index.Columns.Add(dbColumn);
    //                            }
    //                            table.Indexes.Add(index);
    //                        }
    //                        break;
    //                    case "gin":
    //                        {
    //                            var index = new DbGinIndex { Name = indexName, Table = table };
    //                            foreach (var column in indexColumnsDict[indexName])
    //                            {
    //                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
    //                                if (dbColumn is null) continue;
    //                                index.Columns.Add(dbColumn);
    //                            }
    //                            table.Indexes.Add(index);
    //                        }
    //                        break;
    //                    case "gist":
    //                        {
    //                            var index = new DbGistIndex { Name = indexName, Table = table };
    //                            foreach (var column in indexColumnsDict[indexName])
    //                            {
    //                                var dbColumn = table.Columns.SingleOrDefault(x => x.Name == column);
    //                                if (dbColumn is null) continue;
    //                                index.Columns.Add(dbColumn);
    //                            }
    //                            table.Indexes.Add(index);
    //                        }
    //                        break;
    //                    case "hash":
    //                        {
    //                            var dbColumn = table.Columns.SingleOrDefault(x => x.Name == indexColumnsDict[indexName][0]);
    //                            if (dbColumn is null) continue;
    //                            var index = new DbHashIndex { Name = indexName, Table = table };
    //                            index.Columns.Add(dbColumn);
    //                            table.Indexes.Add(index);
    //                        }
    //                        break;
    //                    default:
    //                        continue;
    //                }
    //            }
    //        }

    //        return tables;
    //    }

    //    private DbColumn CreateDbColumnDatabaseInfo(DbTable table, string columnName, string dbType, bool isNullable, string defaultValue, DbObjectMeta meta)
    //    {
    //        switch (dbType)
    //        {
    //            case "uuid":
    //                {
    //                    if (columnName == Constants.DB_TABLE_ID_COLUMN_NAME)
    //                    {
    //                        var column = table.AddTableIdColumn();
    //                        column.SetMeta(meta);
    //                    }
    //                    else
    //                    {
    //                        Guid? guidDefaultValue = (Guid?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbGuidColumn), defaultValue);
    //                        DbGuidColumn column = null;
    //                        if (defaultValue?.Trim() == "uuid_generate_v1()")
    //                            column = table.AddGuidColumn(columnName, isNullable, true);
    //                        else
    //                            column = table.AddGuidColumn(columnName, isNullable, guidDefaultValue);
    //                        column.SetMeta(meta);
    //                    }
    //                }
    //                break;
    //            case "integer":
    //                {
    //                    var column = table.AddAutoIncrementColumn(columnName);
    //                    column.SetMeta(meta);
    //                }
    //                break;
    //            case "boolean":
    //                {
    //                    bool? boolDefaultValue = (bool?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbBooleanColumn), defaultValue);
    //                    var column = table.AddBooleanColumn(columnName, isNullable, boolDefaultValue);
    //                    column.SetMeta(meta);
    //                }
    //                break;
    //            case "numeric":
    //                {
    //                    decimal? decimalDefaultValue = (decimal?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbNumberColumn), defaultValue);
    //                    var column = table.AddNumberColumn(columnName, isNullable, decimalDefaultValue);
    //                    column.SetMeta(meta);
    //                }
    //                break;
    //            case "date":
    //                {
    //                    DbDateColumn column = null;
    //                    if (defaultValue == "now()")
    //                    {
    //                        column = table.AddDateColumn(columnName, isNullable, true);
    //                    }
    //                    else
    //                    {
    //                        DateOnly? dateDefaultValue = (DateOnly?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbDateColumn), defaultValue);
    //                        column = table.AddDateColumn(columnName, isNullable, dateDefaultValue);
    //                    }

    //                    column.SetMeta(meta);
    //                }
    //                break;
    //            case "timestamp with time zone":
    //                {
    //                    DbDateTimeColumn column = null;
    //                    if (defaultValue == "now()")
    //                    {
    //                        column = table.AddDateTimeColumn(columnName, isNullable, true);
    //                    }
    //                    else
    //                    {
    //                        DateTime? datetimeDefaultValue = (DateTime?)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbDateTimeColumn), defaultValue);
    //                        column = table.AddDateTimeColumn(columnName, isNullable, datetimeDefaultValue);
    //                    }
    //                    column.SetMeta(meta);
    //                }
    //                break;
    //            case "text":
    //                {
    //                    string textDefaultValue = (string)DbUtility.ConvertDatabaseDefaultValueToDbColumnDefaultValue(columnName, typeof(DbTextColumn), defaultValue);
    //                    var column = table.AddTextColumn(columnName, isNullable, textDefaultValue);
    //                    column.SetMeta(meta);
    //                }
    //                break;
    //            default:
    //                throw new DbException($"Not supported dbType for column '{columnName}'");
    //        };
    //        return null;
    //    } 
    #endregion
}
