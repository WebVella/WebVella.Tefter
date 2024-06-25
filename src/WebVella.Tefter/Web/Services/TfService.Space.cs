namespace WebVella.Tefter.Web.Services;

public partial interface ITfService{
	ValueTask<List<Space>> GetSpacesAsync();

	ValueTask<List<Space>> GetSpacesForUserAsync(Guid userId);
}

public partial class TfService
{
	public async ValueTask<List<Space>> GetSpacesAsync(){ 
		return await dataBroker.GetSpacesAsync();
	}

	public async ValueTask<List<Space>> GetSpacesForUserAsync(Guid userId)
	{
		return await dataBroker.GetSpacesForUserAsync(userId);
	}
}
