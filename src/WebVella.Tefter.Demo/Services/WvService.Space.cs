namespace WebVella.Tefter.Demo.Services;

public partial interface IWvService
{
	List<Space> GetAllSpaces();
}

public partial class WvService : IWvService
{
	public List<Space> GetAllSpaces(){ 
		return SampleData.GetSpaces();	
	}
}
