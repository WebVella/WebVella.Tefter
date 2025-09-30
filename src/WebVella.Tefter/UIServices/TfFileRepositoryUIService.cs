namespace WebVella.Tefter.UIServices;

public partial interface ITfFileRepositoryUIService
{
	//Events
	event EventHandler<TfRepositoryFile> FileRepositoryCreated;
	event EventHandler<TfRepositoryFile> FileRepositoryUpdated;
	event EventHandler<TfRepositoryFile> FileRepositoryDeleted;

	//FileRepository
	List<TfRepositoryFile> GetRepositoryFiles(string? search = null,int? page = null, int? pageSize = null);
	TfRepositoryFile GetRepositoryFile(string filename);
	TfRepositoryFile CreateRepositoryFile(TfFileForm file);
	TfRepositoryFile UpdateRepositoryFile(TfFileForm role);
	void DeleteRepositoryFile(string fileName);
}
public partial class TfFileRepositoryUIService : ITfFileRepositoryUIService
{
	#region << Ctor >>
	private static readonly AsyncLock _asyncLock = new AsyncLock();
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfSpaceUIService> LOC;
	public TfFileRepositoryUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfSpaceUIService>>() ?? default!;
	}
	#endregion

	#region << Events >>
	public event EventHandler<TfRepositoryFile> FileRepositoryCreated = default!;
	public event EventHandler<TfRepositoryFile> FileRepositoryUpdated = default!;
	public event EventHandler<TfRepositoryFile> FileRepositoryDeleted = default!;
	#endregion

	#region << File Repository >>
	public List<TfRepositoryFile> GetRepositoryFiles(string? search = null,int? page = null, int? pageSize = null) => _tfService.GetRepositoryFiles(
				filenameContains: search,
				page: page,
				pageSize: pageSize	
	);
	public TfRepositoryFile GetRepositoryFile(string filename)
		=> _tfService.GetRepositoryFile(filename);

	public TfRepositoryFile CreateRepositoryFile(TfFileForm form)
	{
		var file = _tfService.CreateRepositoryFile(
			filename: Path.GetFileName(form.Filename),
			localPath: form.LocalFilePath,
			createdBy: form.CreatedBy);
		FileRepositoryCreated?.Invoke(this, file);
		return file;
	}
	public TfRepositoryFile UpdateRepositoryFile(TfFileForm form)
	{
		var file = _tfService.UpdateRepositoryFile(
			filename: Path.GetFileName(form.Filename),
			localPath: form.LocalFilePath,
			updatedBy: form.CreatedBy);
		FileRepositoryUpdated?.Invoke(this, file);
		return file;
	}
	public void DeleteRepositoryFile(string fileName)
	{
		var file = GetRepositoryFile(fileName);
		if(file is null)
			throw new Exception("File not found");
		_tfService.DeleteRepositoryFile(fileName);
		FileRepositoryDeleted?.Invoke(this, file);
	}	
	#endregion
}
