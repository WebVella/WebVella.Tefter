namespace WebVella.Tefter.Web.Services;

public partial interface ITfService{
	Task<List<Space>> GetSpacesAsync();

	Task<List<Space>> GetSpacesForUserAsync(Guid userId);
}

public partial class TfService
{
	public async Task<List<Space>> GetSpacesAsync(){ 
		return await dataBroker.GetSpacesAsync();
	}

	public async Task<List<Space>> GetSpacesForUserAsync(Guid userId)
	{
		return await dataBroker.GetSpacesForUserAsync(userId);
	}
}
