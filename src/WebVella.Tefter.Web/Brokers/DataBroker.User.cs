namespace WebVella.Tefter.Web.Brokers;

public partial interface IDataBroker
{
	ValueTask<User> GetUserByIdAsync(Guid id);
	ValueTask<User> GetUserByEmailAndPasswordAsync(string email, string password);
}

public partial class DataBroker : IDataBroker
{
	public async ValueTask<User> GetUserByIdAsync(Guid id){ 
		return User.GetFaker().Generate();
	}

	public async ValueTask<User> GetUserByEmailAndPasswordAsync(string email, string password)
	{
		return User.GetFaker().Generate();
	}
}
