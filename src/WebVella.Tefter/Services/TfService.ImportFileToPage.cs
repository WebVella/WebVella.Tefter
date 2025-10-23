namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	Task ImportFilesAsSpacePages(Guid spaceId, List<FluentInputFileEventArgs> files);
	void DeleteFiles(List<FluentInputFileEventArgs> files);
}

public partial class TfService
{
	public async Task ImportFilesAsSpacePages(Guid spaceId, List<FluentInputFileEventArgs> files)
	{
		#region << init >>

		// ReSharper disable once NotResolvedInText
		if (spaceId == Guid.Empty) throw new ArgumentException(nameof(spaceId), "spaceId is required");
		// ReSharper disable once NotResolvedInText
		if(files is null || files.Count == 0) throw new ArgumentException(nameof(files), "nothing to import");

		foreach (var file in files)
		{
			if(file.LocalFile is null) throw new Exception($"Empty local file found");
			if (String.IsNullOrWhiteSpace(file.LocalFile.FullName)) throw new Exception($"Empty path found");
			//if (!File.Exists(path)) throw new Exception($"File does not exist on provided local path: {path}");			
		}

			
			


		#endregion

		// foreach (var localPath in localPaths)
		// {
		// 	await using Stream stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		//
		// 	string? fileName = System.IO.Path.GetFileName(localPath);
		// 	byte[]? fileContent = stream.ReadFully();
		// }
		//
		//

		await Task.Delay(1);
		DeleteFiles(files);
	}

	public void DeleteFiles(List<FluentInputFileEventArgs> files)
	{
		try
		{
			foreach (var file in files)
			{
				if (!File.Exists(file.LocalFile!.FullName)) continue;		
				File.Delete(file.LocalFile!.FullName);
			}
		}
		// ReSharper disable once EmptyGeneralCatchClause
		catch { }		
	}
}