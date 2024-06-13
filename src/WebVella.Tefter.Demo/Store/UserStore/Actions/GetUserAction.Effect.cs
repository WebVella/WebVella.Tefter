namespace WebVella.Tefter.Demo.Store;

public partial class UserStoreEffects
{
	private readonly HttpClient HttpClient;

	public UserStoreEffects(HttpClient httpClient)
	{
		HttpClient = httpClient;
	}

	[EffectMethod(typeof(GetUserAction))]
	public async Task HandleGetUserAction(IDispatcher dispatcher)
	{
		await Task.Delay(2000);

		dispatcher.Dispatch(new GetUserResultAction(false));
	}

	[EffectMethod(typeof(GetUserAction))]
	public async Task HandleGetUserAction2(IDispatcher dispatcher)
	{
		await Task.Delay(2000);
		dispatcher.Dispatch(new GetUserResultAction(false));
	}
}
