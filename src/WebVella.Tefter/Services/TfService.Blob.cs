namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	internal string BlobStoragePath { get; set; }

	public bool ExistsBlob(
		Guid blobId,
		bool temporary = false);

	public Guid CreateBlob(
		byte[] byteArray,
		bool temporary = false);

	public Guid CreateBlob(
		Stream stream,
		bool temporary = false);
	public Guid CreateBlob(
		string localPath,
		bool temporary = false);
	public void CreateBlob(
		Guid blobId,
		string localPath,
		bool temporary = false);
	public void UpdateBlob(
		Guid blobId,
		byte[] byteArray,
		bool temporary = false);

	public void UpdateBlob(
		Guid blobId,
		Stream stream,
		bool temporary = false);

	public void UpdateBlob(
		Guid blobId,
		string localPath,
		bool temporary = false);

	public void DeleteBlob(
		Guid blobId,
		bool temporary = false);

	public byte[] GetBlobByteArray(
		Guid blobId,
		bool temporary = false);

	public Stream GetBlobStream(
		Guid blobId,
		bool temporary = false);

	public void MakeTempBlobPermanent(
		Guid blobId);
	internal Task CleanupEmptyFoldersAndExpiredTemporaryFilesAsync(
		CancellationToken stoppingToken);

	internal void InitBlobStorageFolder();
}

public partial class TfService : ITfService
{
	public string BlobStoragePath { get; set; }

	public Guid CreateBlob(
		Stream inputStream,
		bool temporary = false)
	{
		try
		{
			if (!Directory.Exists(BlobStoragePath))
			{
				throw new Exception($"Blob storage folder ({BlobStoragePath}) cannot be created on file system.");
			}

			//getting GUID with is not used
			Guid blobId = Guid.NewGuid();
			do
			{
				if (File.Exists(GetFileSystemPath(blobId, temporary)))
				{
					blobId = Guid.NewGuid();
					continue;
				}

				break;
			}
			while (true);

			var path = GetFileSystemPath(blobId, temporary);

			var folderPath = Path.GetDirectoryName(path);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			using Stream fileStream = File.Open(path, FileMode.CreateNew, FileAccess.ReadWrite);

			inputStream.Seek(0, SeekOrigin.Begin);

			inputStream.CopyTo(fileStream);

			fileStream.Close();

			inputStream.Close();

			return blobId;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private void CreateBlob(
		Guid id,
		Stream inputStream,
		bool temporary = false,
		bool replaceIdIfExists = true)
	{
		try
		{
			if (!Directory.Exists(BlobStoragePath))
			{
				throw new Exception($"Blob storage folder ({BlobStoragePath}) cannot be created on file system.");
			}

			Guid blobId = id;
			do
			{
				if (File.Exists(GetFileSystemPath(blobId, temporary)))
				{
					if(!replaceIdIfExists)
						throw new Exception($"Blob Id ({id}) already exists.");
					blobId = Guid.NewGuid();
					continue;
				}

				break;
			}
			while (true);

			var path = GetFileSystemPath(blobId, temporary);

			var folderPath = Path.GetDirectoryName(path);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			using Stream fileStream = File.Open(path, FileMode.CreateNew, FileAccess.ReadWrite);

			inputStream.Seek(0, SeekOrigin.Begin);

			inputStream.CopyTo(fileStream);

			fileStream.Close();

			inputStream.Close();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Guid CreateBlob(
		byte[] byteArray,
		bool temporary = false)
	{
		try
		{
			return CreateBlob(new MemoryStream(byteArray), temporary);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Guid CreateBlob(
		string localPath,
		bool temporary = false)
	{
		try
		{
			if (string.IsNullOrEmpty(localPath))
			{
				throw new Exception("Local path is not provided.");
			}

			if (!File.Exists(localPath))
			{
				throw new Exception("File does not exists on provided local path.");
			}

			try
			{
				using var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
				return CreateBlob(stream, temporary);
			}
			finally
			{
				if (File.Exists(localPath))
				{
					File.Delete(localPath);
				}
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void CreateBlob(
		Guid blobId,
		string localPath,
		bool temporary = false)
	{
		try
		{
			if (blobId == Guid.Empty)
			{
				throw new Exception("blobId is not provided.");
			}
			if (string.IsNullOrEmpty(localPath))
			{
				throw new Exception("Local path is not provided.");
			}

			if (!File.Exists(localPath))
			{
				throw new Exception("File does not exists on provided local path.");
			}

			try
			{
				using var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
				CreateBlob(blobId, stream, temporary, replaceIdIfExists:false);
			}
			finally
			{
				if (File.Exists(localPath))
				{
					File.Delete(localPath);
				}
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void UpdateBlob(
		Guid blobId,
		Stream inputStream,
		bool temporary = false)
	{
		try
		{
			if (!ExistsBlob(blobId, temporary))
			{
				throw new TfException("Blob does not exist for specified identifier");
			}

			var path = GetFileSystemPath(blobId, temporary);

			using Stream fileStream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);

			inputStream.Seek(0, SeekOrigin.Begin);

			inputStream.CopyTo(fileStream);

			fileStream.Close();

			inputStream.Close();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void UpdateBlob(
		Guid blobId,
		byte[] byteArray,
		bool temporary = false)
	{
		try
		{
			UpdateBlob(blobId, new MemoryStream(byteArray), temporary);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void UpdateBlob(
		Guid blobId,
		string localPath,
		bool temporary = false)
	{
		try
		{
			if (string.IsNullOrEmpty(localPath))
			{
				throw new Exception("Local path is not provided.");
			}

			if (!File.Exists(localPath))
			{
				throw new Exception("File does not exists on provided local path.");
			}

			try
			{
				using var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
				UpdateBlob(blobId, stream, temporary);
			}
			catch
			{
				if (File.Exists(localPath))
				{
					File.Delete(localPath);
				}
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteBlob(
		Guid blobId,
		bool temporary = false)
	{
		try
		{
			if (!ExistsBlob(blobId, temporary))
			{
				//ignore deleting missing blob
				return;
				//throw new TfException("Blob does not exist for specified identifier");
			}

			var filepath = GetFileSystemPath(blobId, temporary);

			if (!File.Exists(filepath))
			{
				throw new TfException("Blob file does not exist for specified identifier");
			}

			File.Delete(filepath);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public bool ExistsBlob(
		Guid blobId,
		bool temporary = false)
	{
		try
		{
			var path = GetFileSystemPath(blobId, temporary);
			return File.Exists(path);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void MakeTempBlobPermanent(
		Guid blobId)
	{
		try
		{
			if (!ExistsBlob(blobId, true))
			{
				throw new TfException("Temporary blob does not exist for specified identifier");
			}

			var filepath = GetFileSystemPath(blobId, true);

			if (!File.Exists(filepath))
			{
				throw new TfException("Temporary blob file does not exist for specified identifier");
			}


			var stream = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

			CreateBlob(blobId, stream, false);

			DeleteBlob(blobId, true);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public byte[] GetBlobByteArray(
		Guid blobId,
		bool temporary = false)
	{
		try
		{
			var path = GetFileSystemPath(blobId, temporary);

			if (!File.Exists(path))
			{
				throw new TfException("Blob content with specified identifier is not found.");
			}

			return File.ReadAllBytes(path);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public Stream GetBlobStream(
		Guid blobId,
		bool temporary = false)
	{
		try
		{
			var path = GetFileSystemPath(blobId, temporary);

			if (!File.Exists(path))
			{
				throw new TfException("Blob content with specified identifier is not found.");
			}

			return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void InitBlobStorageFolder()
	{
		InitBlobStorageFolder(_config.BlobStoragePath);
	}

	#region <--- private methods --->

	private void InitBlobStorageFolder(string localPath)
	{
		BlobStoragePath = GetBlobStorageFolder(localPath);

		try
		{
			if (!Directory.Exists(BlobStoragePath))
				Directory.CreateDirectory(BlobStoragePath);

			var tmpDirectory = Path.Combine(BlobStoragePath, "_");
			if (!Directory.Exists(tmpDirectory))
				Directory.CreateDirectory(tmpDirectory);
		}
		catch { }
	}

	private string GetBlobStorageFolder(string localPath)
	{
		string path = localPath;

		if (string.IsNullOrEmpty(path))
		{
			path = Path.Combine(GetApplicationPath(), "BlobStorage");
		}
		else if (path == "WINDOWS-TEMP")
		{
			path = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
		}

		return path;
	}

	private string GetApplicationPath()
	{
		var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
		return appPathMatcher.Match(exePath).Value;
	}

	private string GetFileSystemPath(Guid blobId, bool temporary = false)
	{
		var guidIinitialPart = blobId.ToString().Split(new[] { '-' })[0];
		var depth1Folder = guidIinitialPart.Substring(0, 2);
		var depth2Folder = guidIinitialPart.Substring(2, 2);

		if (temporary)
		{
			return Path.Combine(BlobStoragePath, "_", depth1Folder, depth2Folder, $"{blobId}.tfblob");
		}
		else
		{
			return Path.Combine(BlobStoragePath, depth1Folder, depth2Folder, $"{blobId}.tfblob");
		}
	}

	#endregion

	public async Task CleanupEmptyFoldersAndExpiredTemporaryFilesAsync(
		CancellationToken stoppingToken)
	{
		try
		{
			var tmpDirectory = Path.Combine(BlobStoragePath, "_");

			if (!Directory.Exists(tmpDirectory))
			{
				return;
			}

			foreach (var dir in Directory.GetDirectories(tmpDirectory))
			{
				if (stoppingToken.IsCancellationRequested)
					return;

				string lvl1DirPath = Path.Combine(tmpDirectory, dir);

				foreach (var childDir in Directory.GetDirectories(lvl1DirPath))
				{
					if (stoppingToken.IsCancellationRequested)
						return;

					if (!Directory.Exists(lvl1DirPath))
						continue;

					var lvl2DirPath = Path.Combine(dir, childDir);
					var files = Directory.GetFiles(lvl2DirPath); ;

					foreach (var file in files)
					{
						string fullFilePath = Path.Combine(lvl2DirPath, file);
						var lastModified = System.IO.File.GetLastWriteTime(fullFilePath);
						if (lastModified < DateTime.Now.AddDays(-7))
						{
							File.Delete(fullFilePath);
						}
					}

					files = Directory.GetFiles(lvl2DirPath);
					if (files.Length == 0)
					{
						Directory.Delete(lvl2DirPath);
					}
				}

				if (Directory.Exists(lvl1DirPath) && Directory.GetDirectories(lvl1DirPath).Length == 0)
				{
					Directory.Delete(lvl1DirPath);
				}
			}

			await Task.Delay(10);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}
}
