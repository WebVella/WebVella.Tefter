﻿namespace WebVella.Tefter.Database.Dbo;

internal partial class DboManager : IDboManager
{
	private IDatabaseService dbService;
	private ILogger logger;

	public DboManager(IDatabaseService dbService, ILogger logger)
	{
		this.dbService = dbService;
		this.logger = logger;
	}

	/// <summary>
	/// Returns list of records from database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async ValueTask<List<T>> GetListAsync<T>(int? page = null, int? pageSize = null, OrderSettings order = null, string searchQuery = null ) where T : class, new()
	{
		string cacheKey = null;
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		var (searchSql, searchParams) = GenerateSearchSql(meta, searchQuery);
		string sql = $"{meta.GetSql} {searchSql} {GenerateOrderSql(meta, order)} {GeneratePagingSql(page, pageSize)} ";
		
		DataTable dt = null;

		if (meta.UseCache && string.IsNullOrWhiteSpace(searchQuery))
		{
			cacheKey = GetHashCode(sql);
			dt = GetFromCache<T>(cacheKey);
		}

		if(dt == null)
			dt = await ExecuteSqlQueryCommandAsync(sql,searchParams);

		var result = ConvertDataTableToModelList<T>(dt, meta);

		if (meta.UseCache && string.IsNullOrWhiteSpace(searchQuery))
			AddToCache<T>(cacheKey, dt);
		else
			logger.LogDebug($"No-Cache:GetListAsync<T>:{typeof(T)}");

		return result;
	}

	/// <summary>
	/// Returns  list of records for specified property value in database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="propertyValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async ValueTask<List<T>> GetListAsync<T>(object propertyValue, string propertyName, 
		int? page = null, int? pageSize = null, OrderSettings order = null) where T : class, new()
	{
		string cacheKey = null;
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		var (whereSql, columnName) = GeneratePropertyWhereSql(meta, propertyName);
		string sql = $"{meta.GetSql} {whereSql} {GenerateOrderSql(meta, order)} {GeneratePagingSql(page, pageSize)} ";
		var sqlParam = new NpgsqlParameter(columnName, propertyValue ?? DBNull.Value);

		DataTable dt = null;
        if (meta.UseCache)
        {
            cacheKey = GetHashCode(sql, sqlParam);
            dt = GetFromCache<T>(cacheKey);
        }
        if (dt == null)
            dt = await ExecuteSqlQueryCommandAsync(sql, sqlParam);

		var result = ConvertDataTableToModelList<T>(dt, meta);

		if (meta.UseCache)
			AddToCache<T>(cacheKey, dt);
		else
			logger.LogDebug($"No-Cache:GetListAsync<T>(prop):{typeof(T)}");

		return result;
	}

	/// <summary>
	/// Returns record for specified parameters and where clause in database async
	/// </summary>
	public async ValueTask<List<T>> GetListAsync<T>(string whereSql, OrderSettings order = null, params NpgsqlParameter[] parameters) where T : class, new()
	{
		string cacheKey = null;
		var meta = GetModelMeta<T>();
		
		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		string sql = meta.GetSql + " " + whereSql + " " + GenerateOrderSql(meta, order);

        DataTable dt = null;
        if (meta.UseCache)
        {
            cacheKey = GetHashCode(sql, parameters);
            dt = GetFromCache<T>(cacheKey);
        }
        if (dt == null)
            dt = await ExecuteSqlQueryCommandAsync(sql, parameters);

		var result = ConvertDataTableToModelList<T>(dt, meta);

		if (meta.UseCache)
			AddToCache<T>(cacheKey, dt);
		else
			logger.LogDebug($"No-Cache:GetListAsync<T>(where):{typeof(T)}");

		return result;
	}

	/// <summary>
	/// Returns record for specified identifier in database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="id"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async ValueTask<T> GetAsync<T>(Dictionary<string,Guid> compositeKey) where T : class, new()
	{
		string cacheKey = null;
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
		List<string> clauses = new List<string>();
		foreach (var propName in compositeKey.Keys)
		{
			clauses.Add($" {propName} = @{propName} ");
			parameters.Add(new NpgsqlParameter($@"key", compositeKey[propName]));
		}

		var whereClause = string.Join(" AND ", clauses);

		DataTable dt = null;
        if (meta.UseCache)
        {
            cacheKey = GetHashCode(meta.GetRecordSql.Replace("$$$WHERE$$$", whereClause), parameters);
            dt = GetFromCache<T>(cacheKey);
        }

        if (dt == null)
            dt = await ExecuteSqlQueryCommandAsync(meta.GetRecordSql.Replace("$$$WHERE$$$", whereClause), parameters);

		var result = ConvertDataTableToModel<T>(dt, meta);

		if (meta.UseCache)
			AddToCache<T>(cacheKey, dt);
		else
			logger.LogDebug($"No-Cache:GetAsync<T>:{typeof(T)}");

		return result;
	}

	/// <summary>
	/// Returns record for specified property value in database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="propertyValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async ValueTask<T> GetAsync<T>(object propertyValue, string propertyName) where T : class, new()
	{
		string cacheKey = null;
		var meta = GetModelMeta<T>();
		
		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		var (whereSql, columnName) = GeneratePropertyWhereSql(meta, propertyName);
		string sql = meta.GetSql + " " + whereSql;
		var sqlParam = new NpgsqlParameter(columnName, propertyValue ?? DBNull.Value);
		
        DataTable dt = null;
        if (meta.UseCache)
        {
            cacheKey = GetHashCode(sql, sqlParam);
            dt = GetFromCache<T>(cacheKey);
        }
        if (dt == null)
            dt = await ExecuteSqlQueryCommandAsync(sql, sqlParam);

		var result = ConvertDataTableToModel<T>(dt, meta);

		if (meta.UseCache)
			AddToCache<T>(cacheKey, dt);
		else
			logger.LogDebug($"No-Cache:GetAsync<T>(prop):{typeof(T)}");

		return result;
	}

	/// <summary>
	/// Returns record for specified parameters and where clause in database async
	/// </summary>
	public async ValueTask<T> GetAsync<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new()
	{
		string cacheKey = null;
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		string sql = meta.GetSql + " " + whereSql;
        DataTable dt = null;
        if (meta.UseCache)
        {
            cacheKey = GetHashCode(sql, parameters);
            dt = GetFromCache<T>(cacheKey);
        }
        if (dt == null)
            dt = await ExecuteSqlQueryCommandAsync(sql, parameters);

		var result = ConvertDataTableToModel<T>(dt, meta);

		if (meta.UseCache)
			AddToCache<T>(cacheKey, dt);
		else
			logger.LogDebug($"No-Cache:GetAsync<T>(where):{typeof(T)}");

		return result;
	}

    /// <summary>
    /// Returns true/false if any records exists for specified parameters and where clause in database async
    /// </summary>
    public async ValueTask<bool> ExistsAnyAsync<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new()
    {
        var meta = GetModelMeta<T>();

        if (meta.IsSqlModel)
            throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		string sql = meta.AnyExistsSql.Replace("$$$WHERE$$$", whereSql);
        var dt = await ExecuteSqlQueryCommandAsync(sql, parameters);

        logger.LogDebug($"No-Cache:ExistsAnyAsync<bool>(where):{typeof(T)}");

		return (bool)dt.Rows[0][0];
    }

    /// <summary>
    /// Returns record for specified sql and parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async ValueTask<T> GetAsyncBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new()
	{
		var meta = GetModelMeta<T>();			
		var dt = await ExecuteSqlQueryCommandAsync(sql, parameters);
		return ConvertDataTableToModel<T>(dt, meta);
	}

	
	/// <summary>
	/// Return list of records for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async ValueTask<List<T>> GetListAsyncBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new()
	{
		var meta = GetModelMeta<T>();
		var dt = await ExecuteSqlQueryCommandAsync(sql, parameters);
		return ConvertDataTableToModelList<T>(dt, meta);
	}

	/// <summary>
	/// Inserts new record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	public async ValueTask<bool> InsertAsync<T>(T obj) where T : class, new()
	{
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		List<NpgsqlParameter> parameters = ExractParametersFromObject<T>(obj);
		var affectedRows = await ExecuteSqlNonQueryCommandAsync(meta.InsertRecordSql, parameters);
		if (affectedRows > 0 && meta.UseCache)
			ClearCache<T>();
		return affectedRows == 1;
	}

	/// <summary>
	/// Updates existing record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	public async ValueTask<bool> UpdateAsync<T>(T obj,  Dictionary<string, Guid> compositeKey, 
		params string [] updateThesePropsOnly ) where T : class, new()
	{
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
		List<string> clauses = new List<string>();
		foreach (var propName in compositeKey.Keys)
		{
			clauses.Add($" {propName} = @{propName} ");
			parameters.Add(new NpgsqlParameter($@"key", compositeKey[propName]));
		}

		var whereClause = string.Join(" AND ", clauses);

		List<NpgsqlParameter> extractedParameters = ExractParametersFromObject<T>(obj, updateThesePropsOnly);
		foreach (var param in extractedParameters)
		{
			if (parameters.Any(x => x.ParameterName == param.ParameterName))
				continue;
			else
				parameters.Add(param);
		}


		string sql = string.Empty;
		if (updateThesePropsOnly != null && updateThesePropsOnly.Length > 0)
		{
			var existingPropNames = meta.Properties.Select(x => x.PropertyInfo.Name).ToHashSet();
			var notFoundProps = updateThesePropsOnly.Where(x => !existingPropNames.Contains(x)).ToList();

            if (notFoundProps.Count > 0 )
				throw new Exception("Properties to update contain non existing one or more! [" + string.Join(",", notFoundProps ) + "]");
			
			var columnNamesToUpdate = meta.Properties.Where(x => updateThesePropsOnly.Contains(x.PropertyInfo.Name)).Select(x => x.ColumnName);
			sql = $"UPDATE {meta.TableName} SET {string.Join(", ", columnNamesToUpdate.Where(x => x != "id").Select(x => $"{x}=@{x}"))} WHERE {whereClause} ";
		}
		else
			sql = meta.UpdateRecordSql.Replace("$$$WHERE$$$", whereClause);

		var affectedRows = await ExecuteSqlNonQueryCommandAsync(sql, parameters);
		if (affectedRows > 0 && meta.UseCache)
			ClearCache<T>();
		return affectedRows == 1;
	}

	/// <summary>
	/// Deletes record by composite key
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin_id"></param>
	/// <param name="target_id"></param>
	/// <returns></returns>
	public async ValueTask<bool> DeleteAsync<T>(Dictionary<string,Guid> compositeKey) where T : class, new()
	{
		var meta = GetModelMeta<T>();

		if (meta.IsSqlModel)
			throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

		List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
		List<string>clauses = new List<string>();
		foreach(var propName in compositeKey.Keys)
		{
			clauses.Add($" {propName} = @{propName} ");
			parameters.Add( new NpgsqlParameter($@"key", compositeKey[propName]));
		}

		var whereClause = string.Join(" AND ", clauses);
		var affectedRows = await ExecuteSqlNonQueryCommandAsync(meta.DeleteRecordSql.Replace("$$$WHERE$$$", whereClause), parameters);
		if (affectedRows > 0 && meta.UseCache)
			ClearCache<T>();
		return affectedRows == 1;
	}

	/// <summary>
	/// Deletes all existing records from database table async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
    public async ValueTask DeleteAllAsync<T>() where T : class, new()
    {
        var meta = GetModelMeta<T>();

        if (meta.IsSqlModel)
            throw new Exception("This method cannot be used with model decorated with DboSqlModelAttribute");

        var affectedRows = await ExecuteSqlNonQueryCommandAsync(meta.DeleteAllRecordSql);
        if (affectedRows > 0 && meta.UseCache)
            ClearCache<T>();
    }

    
}
