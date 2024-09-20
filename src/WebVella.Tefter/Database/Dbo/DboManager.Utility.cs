namespace WebVella.Tefter.Database.Dbo;

internal partial class DboManager
{
    private const int DEFAULT_PAGE = 1;
    private const int DEFAULT_PAGE_SIZE = 10;

    protected static List<T> ConvertDataTableToModelList<T>(DataTable dt, DboModelMeta meta) where T : class, new()
    {
        List<T> result = new List<T>();
        foreach (DataRow dr in dt.Rows)
        {
            result.Add(ConvertDataRowToModel<T>(dr, meta));
        }

        return result;
    }

    protected static T ConvertDataTableToModel<T>(DataTable dt, DboModelMeta meta) where T : class, new()
    {
        if (dt.Rows.Count > 1)
            throw new Exception("More than one DataRows in DataTable");
        if (dt.Rows.Count == 0)
            return null;

        return ConvertDataRowToModel<T>(dt.Rows[0], meta);
    }


    protected static T ConvertDataRowToModel<T>(DataRow dr, DboModelMeta meta) where T : class, new()
    {
        T obj = InstantiateType<T>();
        foreach (var prop in meta.Properties)
        {
            object value = dr[prop.ColumnName] == DBNull.Value ? null : dr[prop.ColumnName];

            if( prop.Converter != null )
                prop.PropertyInfo.SetValue(obj, prop.Converter.ConvertFromDatabaseType(value)); 
            else
                prop.PropertyInfo.SetValue(obj,value);
        }

        return obj;
    }

    protected static List<NpgsqlParameter> ExractParametersFromObject<T>(T obj, params string[] updateThesePropsOnly)
    {
        var meta = GetModelMeta<T>();

        List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
        foreach (var prop in meta.Properties)
        {
            if (prop.ColumnName != "id")
            {
                if (updateThesePropsOnly != null && updateThesePropsOnly.Length > 0 &&
                    !updateThesePropsOnly.Contains(prop.PropertyInfo.Name))
                    continue;
            }


            if (prop.Converter != null)
            {
                var value = prop.Converter.ConvertToDatabaseType(prop.PropertyInfo.GetValue(obj));
                parameters.Add(new NpgsqlParameter($"@{prop.ColumnName}", value ?? DBNull.Value));
            }
            else
            {
                var value = prop.PropertyInfo.GetValue(obj);
                parameters.Add(new NpgsqlParameter($"@{prop.ColumnName}", value ?? DBNull.Value));
            }
        }

        return parameters;
    }

    protected static DboModelMeta GetModelMeta<T>()
    {
        var modelType = typeof(T);
        if (!MetaDict.ContainsKey(modelType))
            throw new Exception($"Specified type '{modelType}' is not decorated with DboModelAttribute or DboSqlModelAttribute.");

        return MetaDict[modelType];
    }

	protected static T InstantiateType<T>() where T : class, new()
    {
        return new T();
    }

    protected static string GeneratePagingSql(int? page, int? pageSize)
    {
        string pagingSql = "";
        if (page.HasValue || pageSize.HasValue)
        {
            if (page == null && pageSize.HasValue)
                page = DEFAULT_PAGE;
            if (page.HasValue && pageSize == null)
                pageSize = DEFAULT_PAGE_SIZE;

            int offset = (page.Value - 1) * pageSize.Value;
            int limit = pageSize.Value;
            pagingSql = $"OFFSET {offset} LIMIT {limit}";
        }

        return pagingSql;
    }

    protected static string GenerateOrderSql(DboModelMeta meta, OrderSettings settings)
    {
        if (settings == null || settings.PropOrderList.Count == 0)
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        foreach (var sortItem in settings.PropOrderList)
        {
            string direction = sortItem.Item2 == OrderDirection.DESC ? "DESC" : "ASC";
            var prop = meta.Properties.Single(x => x.PropertyInfo.Name == sortItem.Item1);

            if (sb.Length == 0)
                sb.Append($"ORDER BY {prop.ColumnName} {direction}");
            else
                sb.Append($", {prop.ColumnName} {direction}");
        }

        return sb.ToString();
    }

//    protected static string GenerateSearchSql(DboModelMeta meta, List<Guid> searchWordIds)
//    {
//        if (meta.Properties.All(x => x.PropertyInfo.Name != "XSearch"))
//            throw new Exception("Trying to do search in database model with no XSearch property");

//        if (searchWordIds.Count == 0)
//            return string.Empty;

//        return $@" 
//WHERE 
//(
//    SELECT COUNT( * ) FROM 
//    (
//        UNNEST 
//        ( 
//            ARRAY(
//            (
//                SELECT DISTINCT search_word_id 
//                FROM rec_search_element 
//                WHERE rec_search_element.search_element_id = ANY( string_to_array ({meta.TableName}.x_search, ',')::uuid[] )
//            )::uuid[]
//        ) 
//    ) 
//    INTERSECT 
//    UNNEST( ARRAY [{string.Join(",", searchWordIds.Select(x => $"'{x.ToString()}'::uuid"))}]::uuid[] )
//) > 0 ";
//    }

	protected static (string, List<NpgsqlParameter>) GenerateSearchSql(DboModelMeta meta, string searchQuery )
    {
        StringBuilder sb = new StringBuilder();
        List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

        if (string.IsNullOrWhiteSpace(searchQuery))
            return (sb.ToString(), parameters);

        if (meta.Properties.All(x => x.PropertyInfo.Name != "XSearch"))
            throw new Exception("Trying to do search in database model with no XSearch property");

        int counter = 1;
        var words = searchQuery.Trim().ToLowerInvariant().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 0)
            sb.Append(" WHERE ");

        foreach (var word in words)
        {
            sb.AppendLine($" x_search ILIKE CONCAT ('%', @x_search_par_{counter}, '%') ");
            if (words.Last() != word)
                sb.AppendLine(" AND ");

            parameters.Add(new NpgsqlParameter($"@x_search_par_{counter}", word));
            counter++;
        }

        return (sb.ToString(), parameters);
    }

	protected static (string, string) GeneratePropertyWhereSql(DboModelMeta meta, string propertyName)
    {
        var prop = meta.Properties.Single(x => x.PropertyInfo.Name == propertyName);
        if (prop == null)
            throw new Exception($"Property '{propertyName}' not found in model {meta.ModelType} ");


        return ($" WHERE {prop.ColumnName} = @{prop.ColumnName} ", prop.ColumnName);
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, params NpgsqlParameter[] parameters)
    {
        return ExecuteSqlQueryCommand(sql, new List<NpgsqlParameter>(parameters));
    }

    protected DataTable ExecuteSqlQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        try
        {
            ProcessNpgsqlParameters(parameters);
            using (var dbCon = dbService.CreateConnection())
            {
                NpgsqlCommand cmd;
                if (parameters != null && parameters.Count > 0)
                    cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
                else
					cmd = dbCon.CreateCommand(sql, CommandType.Text);
				DataTable dataTable = new DataTable();
                new NpgsqlDataAdapter(cmd).Fill(dataTable);
                return dataTable;
            }
        }
        catch
        {
            ClearFullCache();
            throw;
        }
    }

    protected int ExecuteSqlNonQueryCommand(string sql, params NpgsqlParameter[] parameters)
    {
        return ExecuteSqlNonQueryCommand(sql, new List<NpgsqlParameter>(parameters));
    }

    protected int ExecuteSqlNonQueryCommand(string sql, List<NpgsqlParameter> parameters)
    {
        try
        {
            ProcessNpgsqlParameters(parameters);
            using (var dbCon = dbService.CreateConnection())
            {
                return dbCon.CreateCommand(sql, CommandType.Text, parameters).ExecuteNonQuery();
            }
        }
        catch
        {
            ClearFullCache();
            throw;
        }
    }

    protected async ValueTask<DataTable> ExecuteSqlQueryCommandAsync(string sql, params NpgsqlParameter[] parameters)
    {
        return await ExecuteSqlQueryCommandAsync(sql, new List<NpgsqlParameter>(parameters));
    }

#pragma warning disable 1998
    protected async ValueTask<DataTable> ExecuteSqlQueryCommandAsync(string sql, List<NpgsqlParameter> parameters)
    {
        try
        {
            ProcessNpgsqlParameters(parameters);
            //we are not using postsgres driver for async operation because of transaction wrapper library
            using (var dbCon = dbService.CreateConnection())
            {
                NpgsqlCommand cmd = dbCon.CreateCommand(sql, CommandType.Text, parameters);
                DataTable dataTable = new DataTable();
                var reader = cmd.ExecuteReader();
                dataTable.Load(reader);
                return dataTable;
            }
        }
        catch
        {
            ClearFullCache();
            throw;
        }
    }
#pragma warning restore 1998

    protected async ValueTask<int> ExecuteSqlNonQueryCommandAsync(string sql, params NpgsqlParameter[] parameters)
    {
        return await ExecuteSqlNonQueryCommandAsync(sql, new List<NpgsqlParameter>(parameters));
    }

#pragma warning disable 1998
    protected async ValueTask<int> ExecuteSqlNonQueryCommandAsync(string sql, List<NpgsqlParameter> parameters)
    {
        try
        {
            ProcessNpgsqlParameters(parameters);
            //we are not using postsgres driver for async operation because of transaction wrapper library
            using (var dbCon = dbService.CreateConnection())
            {
                var affectedRows = dbCon.CreateCommand(sql, CommandType.Text, parameters).ExecuteNonQuery();
                return affectedRows;
            }
        }
        catch
        {
            ClearFullCache();
            throw;
        }
    }
#pragma warning restore 1998

    protected void ProcessNpgsqlParameters(params NpgsqlParameter[] parameters)
    {
        if (parameters == null)
            return;
        
        foreach(var par in parameters)
        {
            if( par.DbType == DbType.DateTime )
            {
                DateTime? value = (DateTime?)par.Value;
                if( value.HasValue )
                {
                    switch (value.Value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
                            break;
                        case DateTimeKind.Utc:
                            par.Value = value.Value.ToLocalTime();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (par.DbType == DbType.DateTime2)
            {
                DateTime? value = (DateTime?)par.Value;
                if (value.HasValue)
                {
                    switch (value.Value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
                            break;
                        case DateTimeKind.Utc:
                            par.Value = value.Value.ToLocalTime();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (par.DbType == DbType.DateTimeOffset )
            {
                DateTimeOffset? value = (DateTimeOffset?)par.Value;
                if (value.HasValue)
                    par.Value = value.Value.ToLocalTime();
            }
        }
    }

    protected void ProcessNpgsqlParameters( List<NpgsqlParameter> parameters)
    {
        if (parameters == null)
            return;

        foreach (var par in parameters)
        {
			if (par is null)
				continue;

            if (par.DbType == DbType.DateTime)
            {
                DateTime? value = (DateTime?)par.Value;
                if (value.HasValue)
                {
                    switch (value.Value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            par.Value = DateTime.SpecifyKind(value.Value,DateTimeKind.Local);
                            break;
                        case DateTimeKind.Utc:
                            par.Value = value.Value.ToLocalTime();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (par.DbType == DbType.DateTime2)
            {
                DateTime? value = (DateTime?)par.Value;
                if (value.HasValue)
                {
                    switch (value.Value.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            par.Value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
                            break;
                        case DateTimeKind.Utc:
                            par.Value = value.Value.ToLocalTime();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (par.DbType == DbType.DateTimeOffset)
            {
                DateTimeOffset? value = (DateTimeOffset?)par.Value;
                if (value.HasValue)
                    par.Value = value.Value.ToLocalTime();
            }
        }
    }

}