using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace WebVella.Tefter;

public interface ITfBlobManager
{
	internal string BlobStoragePath { get; set; }

	public Result<bool> ExistsBlob(
		Guid blobId,
		bool temporary = false);

	public Result<Guid> CreateBlob(
		byte[] byteArray,
		bool temporary = false);

	public Result<Guid> CreateBlob(
		Stream stream,
		bool temporary = false);
	public Result<Guid> CreateBlob(
		string localPath,
		bool temporary = false);

	public Result UpdateBlob(
		Guid blobId,
		byte[] byteArray,
		bool temporary = false);

	public Result UpdateBlob(
		Guid blobId,
		Stream stream,
		bool temporary = false);

	public Result UpdateBlob(
		Guid blobId,
		string localPath,
		bool temporary = false);

	public Result DeleteBlob(
		Guid blobId,
		bool temporary = false);

	public Result<byte[]> GetBlobByteArray(
		Guid blobId,
		bool temporary = false);

	public Result<Stream> GetBlobStream(
		Guid blobId,
		bool temporary = false);

	public Result MakeTempBlobPermanent(
		Guid blobId);
	internal Task CleanupEmptyFoldersAndExpiredTemporaryFilesAsync(
		CancellationToken stoppingToken);
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

			return Result.Ok(blobId);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	private Result CreateBlob(
		Guid id,
		Stream inputStream,
		bool temporary = false)
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

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result<Guid> CreateBlob(
		byte[] byteArray,
		bool temporary = false)
	{
		try
		{
			return CreateBlob(new MemoryStream(byteArray), temporary);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result<Guid> CreateBlob(
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

			var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

			var result = CreateBlob(stream, temporary);

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
		Stream inputStream,
		bool temporary = false)
	{
		try
		{
			var blobExistsResult = ExistsBlob(blobId, temporary);

			if (!blobExistsResult.IsSuccess)
			{
				return Result.Fail(new Error("Failed to check if blob exists")
					.CausedBy(blobExistsResult.Errors));
			}

			if (!blobExistsResult.Value)
			{
				return Result.Fail(new Error("Blob does not exist for specified identifier"));
			}

			var path = GetFileSystemPath(blobId, temporary);

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
		byte[] byteArray,
		bool temporary = false)
	{
		try
		{
			return UpdateBlob(blobId, new MemoryStream(byteArray), temporary);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create blob").CausedBy(ex));
		}
	}

	public Result UpdateBlob(
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

			var stream = File.Open(localPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

			var result = UpdateBlob(blobId, stream, temporary);

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
		Guid blobId,
		bool temporary = false)
	{
		try
		{
			var blobExistsResult = ExistsBlob(blobId, temporary);

			if (!blobExistsResult.IsSuccess)
			{
				return Result.Fail(new Error("Failed to check if blob exists")
					.CausedBy(blobExistsResult.Errors));
			}

			if (!blobExistsResult.Value)
			{
				return Result.Fail(new Error("Blob does not exist for specified identifier"));
			}

			var filepath = GetFileSystemPath(blobId, temporary);

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
			return Result.Fail(new Error("Failed to check if blob exists").CausedBy(ex));
		}
	}

	public Result MakeTempBlobPermanent(
		Guid blobId)
	{
		try
		{
			var blobExistsResult = ExistsBlob(blobId, true);

			if (!blobExistsResult.IsSuccess)
			{
				return Result.Fail(new Error("Failed to check if temp blob exists")
					.CausedBy(blobExistsResult.Errors));
			}

			if (!blobExistsResult.Value)
			{
				return Result.Fail(new Error("Temporary blob does not exist for specified identifier"));
			}

			var filepath = GetFileSystemPath(blobId, true);

			if (!File.Exists(filepath))
			{
				return Result.Fail(new Error("Temporary blob file does not exist for specified identifier"));
			}

			//create permanent blob
			var stream = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			var createResult = CreateBlob(blobId, stream, false);

			if (!createResult.IsSuccess)
			{
				return Result.Fail(new Error("Failed create permanent copy of temporary blob")
					.CausedBy(blobExistsResult.Errors));
			}

			//delete temp blob
			var deleteResult = DeleteBlob(blobId, true);

			//if delete failed
			if (!deleteResult.IsSuccess)
			{
				//delete already created permanent copy
				DeleteBlob(blobId);

				return Result.Fail(new Error("Failed delete temporary blob")
					.CausedBy(blobExistsResult.Errors));
			}

			return Result.Ok();

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to make blob permanent").CausedBy(ex));
		}
	}

	public Result<byte[]> GetBlobByteArray(
		Guid blobId,
		bool temporary = false)
	{

		try
		{
			var path = GetFileSystemPath(blobId, temporary);

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
		Guid blobId,
		bool temporary = false)
	{
		try
		{
			var path = GetFileSystemPath(blobId, temporary);

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

			var tmpDirectory = Path.Combine(BlobStoragePath, "_");
			if (!Directory.Exists(tmpDirectory))
				Directory.CreateDirectory(tmpDirectory);
		}
		catch
		{
		}
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

		var tmpDirectory = Path.Combine(BlobStoragePath, "_");

		if( !Directory.Exists(tmpDirectory))
		{
			return;
		}

		foreach (var dir in Directory.GetDirectories(tmpDirectory))
		{
			string lvl1DirPath = Path.Combine(tmpDirectory, dir);

			foreach (var childDir in Directory.GetDirectories(lvl1DirPath))
			{
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

			if (Directory.GetDirectories(lvl1DirPath).Length == 0)
			{
				Directory.Delete(lvl1DirPath);
			}
		}

		await Task.Delay(100);
	}
}
