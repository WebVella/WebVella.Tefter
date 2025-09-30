namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
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
public partial class TfUIService : ITfUIService
{
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
