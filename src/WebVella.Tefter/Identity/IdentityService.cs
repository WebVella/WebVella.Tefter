using System.Data;

namespace WebVella.Tefter.Identity;

public interface IIdentityService
{
	Task<List<User>> GetUsersAsync(string searchQuery = null, int? page = null, int? pageSize = null);
}

public class TefterIdentityService : IIdentityService
{
	private readonly IDboManager _dboManager;

	public TefterIdentityService(IServiceProvider serviceProvider)
	{
		_dboManager = serviceProvider.GetService<IDboManager>();
	}

	public async Task<List<User>> GetUsersAsync(string searchQuery = null, int? page = null, int? pageSize = null)
	{
		var orderSettings = new OrderSettings(nameof(User.LastName), OrderDirection.ASC)
				.Add(nameof(User.FirstName), OrderDirection.ASC);

		var users = await _dboManager.GetListAsync<User>(page, pageSize, orderSettings, searchQuery);
		var roles = await _dboManager.GetListAsync<Role>();

		foreach (var user in users)
		{
			var userRelatedRoleIdsHashset = (await _dboManager.GetListAsync<UserRoleRelation>(user.Id, nameof(UserRoleRelation.UserId)))
				.Select(x => x.UserId)
				.ToHashSet<Guid>();

			user.Roles = roles.Where(x => userRelatedRoleIdsHashset.Contains(x.Id)).ToList();
		}

		return users;
	}
}
