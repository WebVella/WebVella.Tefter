namespace WebVella.Tefter;

public interface ITfFileManager
{
	public string RootPath { get; }

	Result<byte[]> GetBytesFromLocalFileSystemPath(
		string path);

	public Result<TfFile> FindFile(
		string filePath);

	public Result<List<TfFile>> FindAllFiles(
	   string startsWithPath = null,
	   string containsPath = null,
	   bool includeTempFiles = false,
	   int? page = null,
	   int? pageSize = null);

	public Result<TfFile> CreateFile(
		string filePath,
		byte[] buffer,
		bool overwrite = false,
		Guid? createdBy = null);

	public Result<TfFile> CreateTempFile(
		string filename,
		byte[] buffer,
		Guid? createdBy = null);

	public Result<TfFile> CopyFile(
		string sourceFilepath,
		string destinationFilepath,
		bool overwrite = false);

	public Result<TfFile> MoveFile(
		string sourceFilepath,
		string destinationFilepath,
		bool overwrite = false);

	public Result DeleteFile(
		string filePath);

	public Result<byte[]> GetFileContentAsByteArray(
		TfFile file);

	public Result<FileStream> GetFileContentAsFileStream(
		TfFile file,
		FileAccess fileAccess = FileAccess.ReadWrite,
		FileShare fileShare = FileShare.ReadWrite);
}

internal class TfFileManager : ITfFileManager
{
	private const string FOLDER_SEPARATOR = "/";
	private const string TMP_FOLDER_NAME = "tmp";

	private readonly IDatabaseService _dbServise;
	private readonly IDboManager _dboManager;
	public string RootPath { get; init; }

	public TfFileManager(
		ITfConfigurationService config,
		IDatabaseService dbServise,
		IDboManager dboManager)
	{
		_dbServise = dbServise;
		_dboManager = dboManager;

		RootPath = config.FilesRootPath;
		if (string.IsNullOrEmpty(RootPath))
			RootPath = Path.Combine(GetApplicationPath(), "Files");

		try
		{
			if (!Directory.Exists(RootPath))
				Directory.CreateDirectory(RootPath);
		}
		catch { }
	}

	public Result<byte[]> GetBytesFromLocalFileSystemPath(
		string path)
	{
		try
		{
			return File.ReadAllBytes(path);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to read bytes from specified local filepath").CausedBy(ex));
		}
	}

	public Result<TfFile> FindFile(
		string filePath)
	{
		try
		{
			filePath = ProcessFilePath(filePath);

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return Result.Fail(new Error("Filepath is not provided"));
			}

			var files = _dboManager.GetList<TfFile>(
				"WHERE filePath = @filePath",
				order: null,
				new NpgsqlParameter("@filePath", filePath));

			if (files.Count > 0)
				return Result.Ok(files[0]);

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to fild file").CausedBy(ex));
		}
	}

	public Result<List<TfFile>> FindAllFiles(
		string startsWithPath = null,
		string containsPath = null,
		bool includeTempFiles = false,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			//all file paths are lowercase and all starts with folder separator
			if (!string.IsNullOrWhiteSpace(startsWithPath))
			{
				startsWithPath = startsWithPath.ToLowerInvariant();

				if (!startsWithPath.StartsWith(FOLDER_SEPARATOR))
					startsWithPath = FOLDER_SEPARATOR + startsWithPath;
			}

			var pagingSql = GeneratePagingSql(page, pageSize);

			//var startsWithPathSql = " ( @starts_with_path IS NULL OR filePath ILIKE @starts_with_path ) ";
			//var containsPathSql = " ( @contains_path IS NULL OR filePath ILIKE @contains_path ) ";
			//var excludeTempFilesSql = " ( @tmp_path IS NULL OR filePath NOT ILIKE @tmp_path ) ";

			//var sql = $"WHERE filePath NOT ILIKE @tmp_path AND filePath ILIKE @startswith {pagingSql}";

			List<TfFile> files = new List<TfFile>();

			if (!includeTempFiles && !string.IsNullOrWhiteSpace(startsWithPath))
			{
				files = _dboManager.GetList<TfFile>(
					$"WHERE filePath NOT ILIKE @tmp_path AND filePath ILIKE @startswith {pagingSql}",
					order: new OrderSettings(nameof(TfFile.FilePath), OrderDirection.ASC),
					new NpgsqlParameter("@tmp_path", "%" + FOLDER_SEPARATOR + TMP_FOLDER_NAME),
					new NpgsqlParameter("@startswith", "%" + startsWithPath));
			}
			else if (!string.IsNullOrWhiteSpace(startsWithPath))
			{
				files = _dboManager.GetList<TfFile>(
					$"WHERE filePath ILIKE @startswith {pagingSql}",
					order: new OrderSettings(nameof(TfFile.FilePath), OrderDirection.ASC),
					new NpgsqlParameter("@startswith", "%" + startsWithPath));
			}
			else if (!includeTempFiles)
			{
				files = _dboManager.GetList<TfFile>(
					$"WHERE filePath NOT ILIKE @tmp_path {pagingSql}",
					order: new OrderSettings(nameof(TfFile.FilePath), OrderDirection.ASC),
					new NpgsqlParameter("@tmp_path", "%" + FOLDER_SEPARATOR + TMP_FOLDER_NAME));
			}
			else
			{
				files = _dboManager.GetList<TfFile>($"WHERE TRUE = TRUE {pagingSql}");
			}

			return Result.Ok(files);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to find files").CausedBy(ex));
		}
	}

	public Result<TfFile> CreateFile(
		string filePath,
		byte[] buffer,
		bool overwrite = false,
		Guid? createdBy = null)
	{
		try
		{
			filePath = ProcessFilePath(filePath);

			TfFileManagerValidator validator = new TfFileManagerValidator(this);

			var validationResult = validator.ValidateCreate(filePath, overwrite);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			DateTime now = DateTime.Now;

			using (var scope = _dbServise.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var existingFile = FindFile(filePath).Value;

				if (existingFile is not null)
				{
					existingFile.LastModifiedBy = createdBy;

					existingFile.LastModifiedOn = now;

					var updateResult = _dboManager.Update<TfFile>(
						existingFile,
						nameof(TfFile.LastModifiedOn),
						nameof(TfFile.LastModifiedBy));

					if (!updateResult)
					{
						return Result.Fail(new Error("Update database failed"));
					}

					var path = GetFileSystemPath(existingFile);

					//delete existing file on FS
					File.Delete(path);

					//create new file on FS
					using (Stream stream = File.Open(path, FileMode.CreateNew, FileAccess.ReadWrite))
					{
						stream.Write(buffer, 0, buffer.Length);
						stream.Close();
					}
				}
				else
				{
					TfFile file = new TfFile
					{
						Id = Guid.NewGuid(),
						FilePath = filePath,
						CreatedBy = createdBy,
						CreatedOn = now,
						LastModifiedBy = createdBy,
						LastModifiedOn = now
					};

					var insertResult = _dboManager.Insert<TfFile>(file);

					if (!insertResult)
					{
						return Result.Fail(new Error("Insert into database failed"));
					}

					var path = GetFileSystemPath(file);

					var folderPath = Path.GetDirectoryName(path);

					if (!Directory.Exists(folderPath))
					{
						Directory.CreateDirectory(folderPath);
					}

					using (Stream stream = File.Open(path, FileMode.CreateNew, FileAccess.ReadWrite))
					{
						stream.Write(buffer, 0, buffer.Length);
						stream.Close();
					}
				}

				scope.Complete();
			}

			return FindFile(filePath);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new file").CausedBy(ex));
		}
	}

	public Result<TfFile> CreateTempFile(
		string filename,
		byte[] buffer,
		Guid? createdBy = null)
	{
		try
		{
			string section = Guid.NewGuid()
				.ToString()
				.Replace("-", "")
				.ToLowerInvariant();

			if (string.IsNullOrWhiteSpace(filename))
			{
				filename = Guid.NewGuid()
					.ToString()
					.Replace("-", "")
					.ToLowerInvariant() +
					".tmp";
			}

			var tmpFilePath = FOLDER_SEPARATOR +
				TMP_FOLDER_NAME +
				FOLDER_SEPARATOR +
				section +
				FOLDER_SEPARATOR +
				filename;

			var createResult = CreateFile(tmpFilePath, buffer, true, createdBy);

			if (!createResult.IsSuccess)
			{
				return Result
					.Fail(new Error("Failed to create temp file")
					.CausedBy(createResult.Errors));
			}

			var newFile = createResult.Value;

			return Result.Ok(newFile);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create temp file").CausedBy(ex));
		}
	}

	public Result<TfFile> CopyFile(
		string sourceFilepath,
		string destinationFilepath,
		bool overwrite = false)
	{
		try
		{
			sourceFilepath = ProcessFilePath(sourceFilepath);

			destinationFilepath = ProcessFilePath(destinationFilepath);

			TfFileManagerValidator validator = new TfFileManagerValidator(this);

			var validationResult = validator.ValidateCopy(
				sourceFilepath,
				destinationFilepath,
				overwrite);

			if (!validationResult.IsValid)
			{
				return validationResult.ToResult();
			}

			DateTime now = DateTime.Now;

			using (var scope = _dbServise.CreateTransactionScope())
			{
				var destFile = FindFile(destinationFilepath).Value;

				if (destFile != null && overwrite)
				{
					var deleteResult = DeleteFile(destFile.FilePath);

					if (!deleteResult.IsSuccess)
					{
						return Result.Fail(new Error("Failed to overwrite file"));
					}
				}

				var srcFile = FindFile(sourceFilepath).Value;

				var bytes = GetFileContentAsByteArray(srcFile).Value;

				var createResult = CreateFile(
					destinationFilepath,
					bytes,
					overwrite,
					srcFile.CreatedBy);

				if (!createResult.IsSuccess)
				{
					return Result.Fail(new Error("Failed to create new file during copy process"));
				}

				TfFile newFile = createResult.Value;

				scope.Complete();

				return Result.Ok(newFile);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to copy file").CausedBy(ex));
		}
	}

	public Result<TfFile> MoveFile(
		string sourceFilepath,
		string destinationFilepath,
		bool overwrite = false)
	{
		try
		{
			sourceFilepath = ProcessFilePath(sourceFilepath);

			destinationFilepath = ProcessFilePath(destinationFilepath);

			TfFileManagerValidator validator = new TfFileManagerValidator(this);

			var validationResult = validator.ValidateMove(
				sourceFilepath,
				destinationFilepath,
				overwrite);

			if (!validationResult.IsValid)
			{
				return validationResult.ToResult();
			}

			DateTime now = DateTime.Now;

			using (var scope = _dbServise.CreateTransactionScope())
			{
				var destFile = FindFile(destinationFilepath).Value;

				if (destFile != null && overwrite)
				{
					var deleteResult = DeleteFile(destFile.FilePath);

					if (!deleteResult.IsSuccess)
					{
						return Result.Fail(new Error("Failed to overwrite file"));
					}
				}

				var srcFile = FindFile(sourceFilepath).Value;

				srcFile.FilePath = destFile.FilePath;

				var updateResult = _dboManager.Update<TfFile>(srcFile, nameof(TfFile.FilePath));

				if (!updateResult)
				{
					return Result.Fail(new Error("Failed to create new file during copy process"));
				}

				TfFile newFile = FindFile(srcFile.FilePath).Value;

				scope.Complete();

				return Result.Ok(newFile);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to move file").CausedBy(ex));
		}
	}

	public Result DeleteFile(
		string filePath)
	{
		try
		{
			filePath = ProcessFilePath(filePath);

			TfFileManagerValidator validator = new TfFileManagerValidator(this);

			var validationResult = validator.ValidateDelete(filePath);

			if (!validationResult.IsValid)
			{
				return validationResult.ToResult();
			}

			using (var scope = _dbServise.CreateTransactionScope())
			{
				var existingFile = FindFile(filePath).Value;

				var deleteResult = _dboManager.Delete<TfFile>(existingFile.Id);

				if (!deleteResult)
				{
					return Result.Fail(new Error("Failed to delete file from database."));
				}

				var path = GetFileSystemPath(existingFile);

				if (File.Exists(path))
				{
					File.Delete(path);
				}

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete file").CausedBy(ex));
		}
	}

	public Result<byte[]> GetFileContentAsByteArray(
		TfFile file)
	{
		try
		{
			using var contentStream = GetFileContentAsFileStream(
					file,
					FileAccess.Read,
					FileShare.Read).Value;

			return new BinaryReader(contentStream)
				.ReadBytes((int)contentStream.Length);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get file content as byte array").CausedBy(ex));
		}
	}

	public Result<FileStream> GetFileContentAsFileStream(
		TfFile file,
		FileAccess fileAccess = FileAccess.ReadWrite,
		FileShare fileShare = FileShare.ReadWrite)
	{
		try
		{
			TfFileManagerValidator validator = new TfFileManagerValidator(this);

			var validationResult = validator.ValidateFileContentStream(file);

			if (!validationResult.IsValid)
			{
				return validationResult.ToResult();
			}

			var path = GetFileSystemPath(file);

			var stream = File.Open(path, FileMode.Open, fileAccess, fileShare);

			return Result.Ok(stream);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get file content as byte array").CausedBy(ex));
		}
	}



	#region <--- private methods --->

	private string GeneratePagingSql(int? page, int? pageSize)
	{
		if (!page.HasValue || !pageSize.HasValue)
			return string.Empty;

		int offset = (page.Value - 1) * pageSize.Value;
		int limit = pageSize.Value;
		return $"OFFSET {offset} LIMIT {limit}";
	}

	private string GetApplicationPath()
	{
		var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
		return appPathMatcher.Match(exePath).Value;
	}

	private string GetFileSystemPath(TfFile file)
	{
		var guidIinitialPart = file.Id.ToString().Split(new[] { '-' })[0];
		var fileName = file.FilePath.Split(new[] { '/' }).Last();
		var depth1Folder = guidIinitialPart.Substring(0, 2);
		var depth2Folder = guidIinitialPart.Substring(2, 2);
		// BUG: https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getextension?view=net-5.0
		// Path.GetExtension includes the "." which means further on we are adding double "."
		// Would probably ruin too many databases to just fix here though
		string filenameExt = Path.GetExtension(fileName);

		if (!string.IsNullOrWhiteSpace(filenameExt))
			return Path.Combine(RootPath, depth1Folder, depth2Folder, file.Id + "." + filenameExt);

		else
			return Path.Combine(RootPath, depth1Folder, depth2Folder, file.Id.ToString());
	}

	private string ProcessFilePath(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
			return string.Empty;

		
		filePath = filePath.ToLowerInvariant();

		if (filePath.StartsWith("tefter://fs"))
			filePath = filePath.Substring("tefter://fs".Length);

		if (!filePath.StartsWith(FOLDER_SEPARATOR))
			filePath = FOLDER_SEPARATOR + filePath;

		return filePath;
	}

	#endregion

	#region <--- Validation --->

	internal class TfFileManagerValidator : AbstractValidator<TfFile>
	{
		private readonly ITfFileManager _fileManager;

		public TfFileManagerValidator(
			ITfFileManager fileManager)
		{
			_fileManager = fileManager;
		}

		public ValidationResult ValidateCreate(
			string filePath,
			bool overwrite)
		{

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Filepath is not provided") });
			}

			if( ! ValidateFilePath(filePath) )
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Filepath is not valid") });
			}

			var fileExists = _fileManager.FindFile(filePath).Value != null;

			if (fileExists && !overwrite)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File already exists and overwrite is set to false") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateCopy(
			string sourceFilepath,
			string destinationFilepath,
			bool overwrite)
		{
			if (!ValidateFilePath(sourceFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Source filepath is not valid") });
			}

			if (string.IsNullOrWhiteSpace(sourceFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Source filePath is not provided") });
			}

			if (string.IsNullOrWhiteSpace(destinationFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Destination filePath is not provided") });
			}

			if (!ValidateFilePath(destinationFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Destination filepath is not valid") });
			}

			var srcFile = _fileManager.FindFile(sourceFilepath).Value;

			if (srcFile == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Source file does not exist") });
			}

			var destFile = _fileManager.FindFile(destinationFilepath).Value;

			if (destFile != null && overwrite == false)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Destination file already exists and overwrite is set to false") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateMove(
			string sourceFilepath,
			string destinationFilepath,
			bool overwrite)
		{

			if (!ValidateFilePath(sourceFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Source filepath is not valid") });
			}

			if (string.IsNullOrWhiteSpace(sourceFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Source filePath is not provided") });
			}

			if (string.IsNullOrWhiteSpace(destinationFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Destination filePath is not provided") });
			}

			if (!ValidateFilePath(destinationFilepath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Destination filepath is not valid") });
			}


			var srcFile = _fileManager.FindFile(sourceFilepath).Value;

			if (srcFile == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Source file does not exist") });
			}

			var destFile = _fileManager.FindFile(destinationFilepath).Value;

			if (destFile != null && overwrite == false)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Destination file already exists and overwrite is set to false") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateDelete(
			string filePath)
		{

			if (string.IsNullOrWhiteSpace(filePath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Filepath is not provided") });
			}

			if (!ValidateFilePath(filePath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Filepath is not valid") });
			}


			var fileToDelete = _fileManager.FindFile(filePath).Value;

			if (fileToDelete is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File does not exist") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateFileContentStream(
			TfFile file)
		{
			if (file is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File object is null") });
			}

			if (string.IsNullOrWhiteSpace(file.FilePath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Filepath is not provided") });
			}

			var existingFile = _fileManager.FindFile(file.FilePath).Value;

			if (existingFile is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File does not exist") });
			}

			return new ValidationResult();
		}

		private bool ValidateFilePath(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				return false;

			var filePathTmp = filePath.ToLowerInvariant();
			if (!filePathTmp.StartsWith(FOLDER_SEPARATOR))
				filePathTmp = FOLDER_SEPARATOR + filePathTmp;

			try
			{
				if (!filePathTmp.StartsWith("tefter://fs"))
					filePathTmp = $"tefter://fs{filePathTmp}";

				Uri uri = new Uri(filePathTmp);

				return true;
			}
			catch
			{
			}

			return false;
		}
	}


	#endregion

}
