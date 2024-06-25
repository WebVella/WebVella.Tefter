namespace WebVella.Tefter.Web.Brokers;

public partial interface IDataBroker
{
	ValueTask<UserSession> GetUserSessionAsync(Guid userId);
}

public partial class DataBroker : IDataBroker
{


	public async ValueTask<UserSession> GetUserSessionAsync(Guid userId)
	{
		return null;
	}

}
