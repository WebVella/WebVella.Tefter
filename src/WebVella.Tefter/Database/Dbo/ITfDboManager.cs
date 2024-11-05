using System;

namespace WebVella.Tefter.Database.Dbo;

internal partial interface ITfDboManager
{
	/// <summary>
	/// Returns list of records from database
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	List<T> GetList<T>(int? page = null, int? pageSize = null, TfOrderSettings order = null, string searchQuery = null) where T : class, new();

	/// <summary>
	/// Returns list of records from database filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	List<T> GetList<T>(object propertyValue, string propertyName, int? page = null, int? pageSize = null, TfOrderSettings order = null) where T : class, new();

	/// <summary>
	/// Returns record for specified parameters and where clause in database
	/// </summary>
	List<T> GetList<T>(string whereSql, TfOrderSettings order = null, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Returns records list for specified sql query
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin_id"></param>
	/// <param name="target_id"></param>
	/// <returns></returns>
	T Get<T>(Guid id) where T : class, new();


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
	bool Update<T>(T obj, params string[] updateThesePropsOnly) where T : class, new();

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
	bool Delete<T>(Guid id) where T : class, new();

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
	Task<List<T>> GetListAsync<T>(int? page = null, int? pageSize = null, TfOrderSettings order = null, string searchQuery = null ) where T : class, new();
	
	/// <summary>
	/// Returns list of records from database async  filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="page"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	Task<List<T>> GetListAsync<T>(object propertyValue, string propertyName, int? page = null, int? pageSize = null, TfOrderSettings order = null) where T : class, new();

	/// <summary>
	/// Returns record for specified parameters and where clause in database async
	/// </summary>
	Task<List<T>> GetListAsync<T>(string whereSql, TfOrderSettings order = null, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Gets record by id
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="id"></param>
	/// <returns></returns>
	Task<T> GetAsync<T>(Guid id) where T : class, new();

	/// <summary>
	/// Gets record by composite id
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="compositeKey"></param>
	/// <returns></returns>
	Task<T> GetAsync<T>(Dictionary<string, Guid> compositeKey) where T : class, new();

	/// <summary>
	/// Returns record for specified identifier in database async filtered by single property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="propertyValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	Task<T> GetAsync<T>(object propertyValue, string propertyName) where T : class, new();

	/// <summary>
	///  Returns record for specified identifier in database async filtered by where clause sql async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="whereSql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	Task<T> GetAsync<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Returns record for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	Task<T> GetAsyncBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Return list of records for specified sql and parameters
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="sql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	Task<List<T>> GetListAsyncBySql<T>(string sql, params NpgsqlParameter[] parameters) where T : class, new();

	/// <summary>
	/// Inserts new record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	Task<bool> InsertAsync<T>(T obj) where T : class, new();

	/// <summary>
	/// Updates existing record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	Task<bool> UpdateAsync<T>(T obj, params string[] updateThesePropsOnly) where T : class, new();


	/// <summary>
	/// Updates existing record into database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	Task<bool> UpdateAsync<T>(T obj, Dictionary<string, Guid> compositeKey, params string[] updateThesePropsOnly) where T : class, new();

	/// <summary>
	/// Deletes record by id async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="id"></param>
	/// <returns></returns>
	Task<bool> DeleteAsync<T>(Guid id) where T : class, new();

	/// <summary>
	/// Deletes existing record from relation table async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="origin_id"></param>
	/// <param name="target_id"></param>
	/// <returns></returns>
	Task<bool> DeleteAsync<T>(Dictionary<string,Guid> compositeKey) where T : class, new();

	/// <summary>
	/// Returns true/false if any records exists for specified parameters and where clause in database async
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="whereSql"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	Task<bool> ExistsAnyAsync<T>(string whereSql, params NpgsqlParameter[] parameters) where T : class, new();
}

