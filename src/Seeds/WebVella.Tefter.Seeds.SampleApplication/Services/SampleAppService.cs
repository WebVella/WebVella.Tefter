using WebVella.Tefter.Database;
using WebVella.Tefter.Services;

namespace WebVella.Tefter.Seeds.SampleApplication.Services;

public partial interface ISampleAppService
{
}

internal partial class SampleAppService : ISampleAppService
{
	public readonly ITfDatabaseService _dbService;
	public readonly ITfService _tfService;

	public SampleAppService(
		ITfDatabaseService dbService,
		ITfService tfService)
	{
		_dbService = dbService;
		_tfService = tfService;
	}
}
