using System;

namespace WebVella.Tefter.Database.Dbo;

internal partial interface IDboManager
{
	/// <summary>
	/// Returns list of records from database
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	List<T> GetList<T>(int? page = null, int? pageSize = null, OrderSettings order = null, string searchQuery = null) where T : class, new();

	/// <summary>
	/// Returns list of records from database filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	List<T> GetList<T>(object propertyValue, string propertyName, int? page = null, int? pageSize = null, OrderSettings order = null) where T : class, new();

	/// <summary>
	/// Returns record for specified parameters and where clause in database
	/// </summary>
	List<T> GetList<T>(string whereSql, OrderSettings order = null, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Returns records list for specified sql query
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin_id"></param>
	/// <param name="target_id"></param>
	/// <returns></returns>
	T Get<T>(Dictionary<string, Guid> compositeKey) where T : class, new();

	/// <summary>
	/// Returns record for specified identifier in database async filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="propertyValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	T Get<T>(object propertyValue, string propertyName) where T : class, new();

	/// <summary>
	///  Returns record for specified identifier in database async filtered by where clause sql
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="whereSql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	T Get<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Returns record for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	T GetBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Return list of records for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	List<T> GetListBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Inserts new record into database
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	bool Insert<T>(T obj) where T : class, new();

	/// <summary>
	/// Updates existing record into database
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	bool Update<T>(T obj, Dictionary<string, Guid> compositeKey, params string[] updateThesePropsOnly) where T : class, new();


	/// <summary>
	/// Deletes existing record from relation table
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin_id"></param>
	/// <param name="target_id"></param>
	/// <returns></returns>
	bool Delete<T>(Dictionary<string, Guid> compositeKey) where T : class, new();

	/// <summary>
	/// Returns true/false if any records exists for specified parameters and where clause in database
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="whereSql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	bool ExistsAny<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Returns list of records from database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	ValueTask<List<T>> GetListAsync<T>(int? page = null, int? pageSize = null, OrderSettings order = null, string searchQuery = null ) where T : class, new();
	
	/// <summary>
	/// Returns list of records from database async  filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	ValueTask<List<T>> GetListAsync<T>(object propertyValue, string propertyName, int? page = null, int? pageSize = null, OrderSettings order = null) where T : class, new();

	/// <summary>
	/// Returns record for specified parameters and where clause in database async
	/// </summary>
	ValueTask<List<T>> GetListAsync<T>(string whereSql, OrderSettings order = null, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Deletes existing record from relation table async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin_id"></param>
	/// <param name="target_id"></param>
	/// <returns></returns>
	ValueTask<T> GetAsync<T>(Dictionary<string, Guid> compositeKey) where T : class, new();

	/// <summary>
	/// Returns record for specified identifier in database async filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="propertyValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	ValueTask<T> GetAsync<T>(object propertyValue, string propertyName) where T : class, new();

	/// <summary>
	///  Returns record for specified identifier in database async filtered by where clause sql async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="whereSql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	ValueTask<T> GetAsync<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Returns record for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	ValueTask<T> GetAsyncBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Return list of records for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	ValueTask<List<T>> GetListAsyncBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Inserts new record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	ValueTask<bool> InsertAsync<T>(T obj) where T : class, new();

	/// <summary>
	/// Updates existing record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	ValueTask<bool> UpdateAsync<T>(T obj, Dictionary<string, Guid> compositeKey, params string[] updateThesePropsOnly) where T : class, new();


    /// <summary>
    /// Deletes existing record from relation table async
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="origin_id"></param>
    /// <param name="target_id"></param>
    /// <returns></returns>
    ValueTask<bool> DeleteAsync<T>(Dictionary<string,Guid> compositeKey) where T : class, new();

	/// <summary>
	/// Returns true/false if any records exists for specified parameters and where clause in database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="whereSql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	ValueTask<bool> ExistsAnyAsync<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new();
}

