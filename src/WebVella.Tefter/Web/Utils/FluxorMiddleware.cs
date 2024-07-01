namespace WebVella.Tefter.Web.Utils;
public class LoggingMiddleware : Middleware
{
	private IStore Store;
	private IDispatcher Dispatcher;
	public override Task InitializeAsync(IDispatcher dispatcher, IStore store)
	{
		Store = store;
		Dispatcher = dispatcher;
		ConsoleExt.WriteLine("FLUXOR: " + nameof(InitializeAsync));
		return Task.CompletedTask;
	}

	public override void AfterDispatch(object action)
	{
		ConsoleExt.WriteLine(nameof(AfterDispatch) + ObjectInfo(action));
		ConsoleExt.WriteLine("");
	}
	private string ObjectInfo(object obj)
	  => ": " + obj.GetType().Name + " " + JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
}