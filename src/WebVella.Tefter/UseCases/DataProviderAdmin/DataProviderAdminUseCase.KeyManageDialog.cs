namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{

	internal TucDataProviderSharedKeyForm KeyForm { get; set; } = new();


	internal Task InitForKeyManageDialog()
	{
		return Task.CompletedTask;
	}


}
