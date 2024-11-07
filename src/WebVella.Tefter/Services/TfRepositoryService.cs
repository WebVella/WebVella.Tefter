namespace WebVella.Tefter;

internal interface ITfRepositoryService
{
	public Result<TfRepositoryFile> GetFile(
		string filename);

	public Result<List<TfRepositoryFile>> GetFiles(
	   string filenameStartsWith = null,
	   string filenameContains = null,
	   string filenameEndWith = null,
	   int? page = null,
	   int? pageSize = null);

	public Result<TfRepositoryFile> CreateFile(
		string filename,
		string localPath,
		Guid? createdBy = null);

	public Result UpdateFile(
		string filename,
		string localPath,
		Guid? updatedBy = null);

	public Result DeleteFile(
		string filename);

	public Result<byte[]> GetFileContentAsByteArray(
		string filename);

	public Result<Stream> GetFileContentAsFileStream(
		string filename);
}

internal class TfRepositoryService : ITfRepositoryService
{
	private readonly ITfDatabaseService _dbServise;
	private readonly ITfDboManager _dboManager;
	private readonly ITfBlobManager _blobManager;

	public string RootPath { get; init; }

	public TfRepositoryService(
		ITfBlobManager blobManager,
		ITfDatabaseService dbServise,
		ITfDboManager dboManager)
	{
		_dbServise = dbServise;
		_dboManager = dboManager;
		_blobManager = blobManager;
	}

	public Result<TfRepositoryFile> GetFile(
		string filename)
	{
		try
		{
			if (string.IsNullOrEmpty(filename))
				return null;

			var file = _dboManager.Get<TfRepositoryFile>(
					" WHERE filename ILIKE @filename",
					new NpgsqlParameter("@filename", filename));

			return Result.Ok(file);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to find repository file").CausedBy(ex));
		}
	}

	public Result<List<TfRepositoryFile>> GetFiles(
		string filenameStartsWith = null,
		string filenameContains = null,
		string filenameEndWith = null,
		int? page = null,
		int? pageSize = null)
	{
		try
		{
			var pagingSql = GeneratePagingSql(page, pageSize);

			var orderBySql = " ORDER BY filename ASC ";
			var startsWithSql = " ( @starts_with IS NULL OR filename ILIKE @starts_with ) ";
			var containsSql = " ( @contains IS NULL OR filename ILIKE @contains ) ";
			var endsWithSql = " ( @ends_with IS NULL OR filename ILIKE @ends_with ) ";

			var sql = $"WHERE {startsWithSql} AND {containsSql} AND {endsWithSql} AND {orderBySql} {pagingSql}";

			var startsWithParameter = new NpgsqlParameter("@starts_with", DbType.String);
			if (!string.IsNullOrWhiteSpace(filenameStartsWith))
			{
				startsWithParameter.Value = filenameStartsWith + "%";
			}
			else
			{
				startsWithParameter.Value = DBNull.Value;
			}

			var containsPathParameter = new NpgsqlParameter("@contains", DbType.String);
			if (!string.IsNullOrWhiteSpace(filenameContains))
			{
				containsPathParameter.Value = "%" + filenameContains + "%";
			}
			else
			{
				containsPathParameter.Value = DBNull.Value;
			}

			var endsWithParameter = new NpgsqlParameter("@ends_with", DbType.String);
			if (!string.IsNullOrWhiteSpace(filenameEndWith))
			{
				endsWithParameter.Value = "%" + filenameEndWith;
			}
			else
			{
				endsWithParameter.Value = DBNull.Value;
			}

			var files = _dboManager.GetList<TfRepositoryFile>(
					sql,
					order: null,
					startsWithParameter,
					containsPathParameter,
					endsWithParameter);

			return Result.Ok(files);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to find repository files").CausedBy(ex));
		}
	}

	public Result<TfRepositoryFile> CreateFile(
		string filename,
		string localPath,
		Guid? createdBy = null)
	{
		try
		{
			TfRepositoryServiceValidator validator = new TfRepositoryServiceValidator(this);

			var validationResult = validator.ValidateCreate(filename, localPath);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			DateTime now = DateTime.Now;

			using (var scope = _dbServise.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var blobIdResult = _blobManager.CreateBlob(localPath);

				if (!blobIdResult.IsSuccess)
				{
					return Result.Fail(new Error("Failed to save file to file system")
						.CausedBy(blobIdResult.Errors));
				}

				TfRepositoryFile file = new TfRepositoryFile
				{
					Id = blobIdResult.Value,
					Filename = filename,
					CreatedBy = createdBy,
					CreatedOn = now,
					LastModifiedBy = createdBy,
					LastModifiedOn = now
				};

				var insertResult = _dboManager.Insert<TfRepositoryFile>(file);

				if (!insertResult)
				{
					return Result.Fail(new Error("Insert repository file record into database failed"));
				}

				scope.Complete();

				return file;
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new repository file").CausedBy(ex));
		}
	}

	public Result UpdateFile(
		string filename,
		string localPath,
		Guid? updatedBy = null)
	{
		try
		{
			TfRepositoryServiceValidator validator = new TfRepositoryServiceValidator(this);

			var validationResult = validator.ValidateUpdate(filename, localPath);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			DateTime now = DateTime.Now;

			using (var scope = _dbServise.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var repFile = GetFile(filename).Value;

				repFile.LastModifiedBy = updatedBy;

				repFile.LastModifiedOn = DateTime.Now;

				var updateResult = _dboManager.Update<TfRepositoryFile>(repFile);

				if (!updateResult)
				{
					return Result.Fail(new Error("Update repository file record into database failed"));
				}

				var blobResult = _blobManager.UpdateBlob(repFile.Id, localPath);

				if (!blobResult.IsSuccess)
				{
					return Result.Fail(new Error("Failed to save content to file system")
						.CausedBy(blobResult.Errors));
				}

				scope.Complete();
			}

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update new repository file").CausedBy(ex));
		}
	}


	public Result DeleteFile(
		string filename)
	{
		try
		{
			TfRepositoryServiceValidator validator = new TfRepositoryServiceValidator(this);

			var validationResult = validator.ValidateDelete(filename);

			if (!validationResult.IsValid)
			{
				return validationResult.ToResult();
			}

			using (var scope = _dbServise.CreateTransactionScope())
			{
				var existingFile = GetFile(filename).Value;

				var deleteResult = _dboManager.Delete<TfRepositoryFile>(existingFile.Id);

				if (!deleteResult)
				{
					return Result.Fail(new Error("Failed to delete file from database."));
				}

				_blobManager.DeleteBlob(existingFile.Id);

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete repository file").CausedBy(ex));
		}
	}

	public Result<byte[]> GetFileContentAsByteArray(
		string filename)
	{
		try
		{
			using var contentStream = GetFileContentAsFileStream(filename).Value;

			return new BinaryReader(contentStream)
				.ReadBytes((int)contentStream.Length);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get file content as byte array").CausedBy(ex));
		}
	}

	public Result<Stream> GetFileContentAsFileStream(
		string filename)
	{
		try
		{
			TfRepositoryServiceValidator validator = new TfRepositoryServiceValidator(this);

			var validationResult = validator.ValidateFileContentStream(filename);

			if (!validationResult.IsValid)
			{
				return validationResult.ToResult();
			}

			var file = GetFile(filename).Value;

			var stream = _blobManager.GetBlobStream(file.Id).Value;

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

	#endregion

	#region <--- Validation --->

	internal class TfRepositoryServiceValidator : AbstractValidator<TfRepositoryFile>
	{
		private readonly ITfRepositoryService _repoService;

		public TfRepositoryServiceValidator(
			ITfRepositoryService repoService)
		{
			_repoService = repoService;
		}

		public ValidationResult ValidateCreate(
			string filename,
			string localPath)
		{

			if (string.IsNullOrWhiteSpace(filename))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File name is not provided") });
			}

			if (!File.Exists(localPath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File is not found for specified local path") });
			}

			var fileExists = _repoService.GetFile(filename).Value != null;

			if (fileExists)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File with same name already exists") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateUpdate(
			string filename,
			string localPath)
		{

			if (string.IsNullOrWhiteSpace(filename))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File name is not provided") });
			}

			if (!File.Exists(localPath))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File is not found for specified local path") });
			}

			var fileExists = _repoService.GetFile(filename).Value != null;

			if (!fileExists)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File with specified filename does not exist") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateDelete(
			string filename)
		{

			if (string.IsNullOrWhiteSpace(filename))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File name is not provided") });
			}

			var fileToDelete = _repoService.GetFile(filename).Value;

			if (fileToDelete is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File does not exist") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateFileContentStream(
			string filename)
		{

			if (string.IsNullOrWhiteSpace(filename))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File name is not provided") });
			}

			var file = _repoService.GetFile(filename).Value;

			if (file is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"File is not found for specified filename") });
			}

			return new ValidationResult();
		}
	}


	#endregion

}
