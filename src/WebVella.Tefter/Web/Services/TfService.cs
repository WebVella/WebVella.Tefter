using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService{
}

public partial class TfService : ITfService
{
	//private ProtectedLocalStorage browserStorage;
	private IDataBroker dataBroker;
	private IIdentityManager identityManager;
	private ITfDataProviderManager dataProviderManager;
	private readonly IJSRuntime _jsRuntime;

	public TfService(IDataBroker dataBroker,
		IIdentityManager identityManager,
		ITfDataProviderManager dataProviderManager,
		IJSRuntime jsRuntime)
	{
		//this.browserStorage = protectedLocalStorage;
		this.dataBroker = dataBroker;
		this.identityManager = identityManager;
		this.dataProviderManager = dataProviderManager;
		_jsRuntime = jsRuntime;
	}

}
