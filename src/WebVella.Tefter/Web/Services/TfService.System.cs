namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
	ValueTask<Result<SystemStateObject>> GetSystemState();

}

public partial class TfService : ITfService
{
	/// <summary>
	/// Get the complete user session for init of the UI
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="spaceId"></param>
	/// <param name="spaceDataId"></param>
	/// <param name="spaceViewId"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async ValueTask<Result<SystemStateObject>> GetSystemState()
	{
		var result = new SystemStateObject();
		var rolesResult = await identityManager.GetRolesAsync();
		if (rolesResult.IsSuccess) result.Roles = rolesResult.Value.ToList();

		var columnTypes = dataProviderManager.GetDatabaseColumnTypeInfos();
		if (columnTypes.IsSuccess) result.DataProviderColumnTypes = columnTypes.Value.ToList();

		return Result.Ok(result);
	}

}
