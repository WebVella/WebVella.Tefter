namespace WebVella.Tefter.Web.Models;

public class SystemStateObject
{
	public List<Role> Roles { get; set; } = new List<Role>();
	public List<DatabaseColumnTypeInfo> DataProviderColumnTypes { get; set; } = new List<DatabaseColumnTypeInfo>();
}
