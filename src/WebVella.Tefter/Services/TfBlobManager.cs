namespace WebVella.Tefter;

public interface ITfBlobManager
{
	internal string BlobStoragePath { get; }

	public Result<bool> ExistsBlob(
		Guid blobId);

	public Result<Guid> CreateBlob(
		byte[] byteArray);

	public Result<Guid> CreateBlob(
		Stream stream);
	public Result<Guid> CreateBlob(
		string localPath);

	public Result UpdateBlob(
		Guid blobId,
		byte[] byteArray);

	public Result UpdateBlob(
		Guid blobId,
		Stream stream);
	public Result UpdateBlob(
		Guid blobId,
		string localPath);

	public Result DeleteBlob(
		Guid blobId);

	public Result<byte[]> GetBlobByteArray(
		Guid blobId);

	public Result<Stream> GetBlobStream(
		Guid blobId);
}

internal class TfBlobManager : ITfBlobManager
{
	public string BlobStoragePath { get; set; }

	public TfBlobManager(
		ITfConfigurationService config)
	{
		InitBlobStorageFolder(config.BlobStoragePath);
	}

	public Result<Guid> CreateBlob(
		Stream inputStream)
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
				if (File.Exists(GetFileSystemPath(blobId)))
				{
					blobId = Guid.NewGuid();
					continue;
				}

				break;
			}
			while (true);

			var path = GetFileSystemPath(blobId);

			var folderPath = Path.GetDirectoryName(path);

			if(!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			using Stream fileStream = File.Open(path, FileMode.CreateNew, FileAccess.ReadWrite);

			inputStream.Seek(0, SeekOrigin.Begin);

			inputStream.CopyTo(fileStream);

			fileStream.Close();

			inputStream.Close();

			return Result.Ok(blobId);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result<Guid> CreateBlob(
		byte[] byteArray)
	{
		try
		{
			return CreateBlob(new MemoryStream(byteArray));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result<Guid> CreateBlob(
		string localPath)
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

			var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

			var result = CreateBlob(stream);

			if (result.IsSuccess)
			{
				File.Delete(localPath);
			}

			return result;
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result UpdateBlob(
		Guid blobId,
		Stream inputStream)
	{
		try
		{
			var blobExistsResult = ExistsBlob(blobId);

			if (!blobExistsResult.IsSuccess)
			{
				return Result.Fail(new Error("Failed to check if blob exists")
					.CausedBy(blobExistsResult.Errors));
			}

			if (!blobExistsResult.Value)
			{
				return Result.Fail(new Error("Blob does not exist for specified identifier"));
			}

			var path = GetFileSystemPath(blobId);

			using Stream fileStream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);

			inputStream.Seek(0, SeekOrigin.Begin);

			inputStream.CopyTo(fileStream);

			fileStream.Close();

			inputStream.Close();

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update blob").CausedBy(ex));
		}
	}

	public Result UpdateBlob(
		Guid blobId,
		byte[] byteArray)
	{
		try
		{
			return UpdateBlob(blobId, new MemoryStream(byteArray));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result UpdateBlob(
		Guid blobId,
		string localPath)
	{
		try
		{
			if (!string.IsNullOrEmpty(localPath))
			{
				throw new Exception("Local path is not provided.");
			}

			if (!File.Exists(localPath))
			{
				throw new Exception("File does not exists on provided local path.");
			}

			var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

			var result = UpdateBlob(blobId, stream);

			if (result.IsSuccess)
			{
				File.Delete(localPath);
			}

			return result;
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result DeleteBlob(
		Guid blobId)
	{
		try
		{
			var blobExistsResult = ExistsBlob(blobId);

			if (!blobExistsResult.IsSuccess)
			{
				return Result.Fail(new Error("Failed to check if blob exists")
					.CausedBy(blobExistsResult.Errors));
			}

			if (!blobExistsResult.Value)
			{
				return Result.Fail(new Error("Blob does not exist for specified identifier"));
			}

			var filepath = GetFileSystemPath(blobId);

			if (!File.Exists(filepath))
			{
				return Result.Fail(new Error("Blob file does not exist for specified identifier"));
			}

			File.Delete(filepath);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result<bool> ExistsBlob(
		Guid blobId)
	{
		try
		{
			var path = GetFileSystemPath(blobId);

			return File.Exists(path);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to check if blob exists").CausedBy(ex));
		}
	}

	public Result<byte[]> GetBlobByteArray(
		Guid blobId)
	{

		try
		{
			var path = GetFileSystemPath(blobId);

			if (!File.Exists(path))
			{
				throw new Exception("Blob content with specified identifier is not found.");
			}

			return File.ReadAllBytes(path);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to read blob content").CausedBy(ex));
		}
	}

	public Result<Stream> GetBlobStream(
		Guid blobId)
	{
		try
		{
			var path = GetFileSystemPath(blobId);

			if (!File.Exists(path))
			{
				throw new Exception("Blob content with specified identifier is not found.");
			}

			return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to read blob content").CausedBy(ex));
		}
	}

	#region <--- private methods --->

	private void InitBlobStorageFolder(string localPath)
	{
		BlobStoragePath = GetBlobStorageFolder(localPath);

		try
		{
			if (!Directory.Exists(BlobStoragePath))
				Directory.CreateDirectory(BlobStoragePath);
		}
		catch
		{
		}
	}

	private string GetBlobStorageFolder(string localPath)
	{
		string path = localPath;

		if (string.IsNullOrEmpty(path))
			path = Path.Combine(GetApplicationPath(), "BlobStorage");

		return path;
	}

	private string GetApplicationPath()
	{
		var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
		return appPathMatcher.Match(exePath).Value;
	}

	private string GetFileSystemPath(Guid blobId)
	{
		var guidIinitialPart = blobId.ToString().Split(new[] { '-' })[0];
		var depth1Folder = guidIinitialPart.Substring(0, 2);
		var depth2Folder = guidIinitialPart.Substring(2, 2);
		return Path.Combine(BlobStoragePath, depth1Folder, depth2Folder, $"{blobId}.tfblob");
	}



	#endregion


}
