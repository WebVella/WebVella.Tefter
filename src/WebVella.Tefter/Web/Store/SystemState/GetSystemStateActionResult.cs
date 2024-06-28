namespace WebVella.Tefter.Web.Store.ThemeState;

public record GetSystemStateActionResult
{
	public bool IsSuccess { get; }
	public List<Role> Roles { get; }

	public List<DatabaseColumnTypeInfo> DataProviderColumnTypes { get; }

	public GetSystemStateActionResult(
		bool isSuccess,
		List<Role> roles,
		List<DatabaseColumnTypeInfo> dataProviderColumnTypes
		)
	{
		IsSuccess = isSuccess;
		Roles = roles;
		DataProviderColumnTypes = dataProviderColumnTypes;
	}
}
