namespace WebVella.Tefter.Web.Store.Base;

public partial class StateEffect
{
	[EffectMethod]
	public async Task GetSystemStateActionEffect(GetSystemStateAction action, IDispatcher dispatcher)
	{
		var setResult = await TefterService.GetSystemState();
		var roles = new List<Role>();
		var columnTypes = new List<DatabaseColumnTypeInfo>();
		if (setResult.IsSuccess)
		{
			roles = setResult.Value.Roles.ToList();
			columnTypes = setResult.Value.DataProviderColumnTypes.ToList();
		}
		dispatcher.Dispatch(new GetSystemStateActionResult(
			isSuccess: setResult.IsSuccess,
			roles: roles,
			dataProviderColumnTypes: columnTypes
			));
	}
}