namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfRepositoryFile GetRepositoryFileByUri(
	   string uriString);

	public TfRepositoryFile GetRepositoryFile(
		string filename);

	public List<TfRepositoryFile> GetRepositoryFiles(
	   string filenameStartsWith = null,
	   string filenameContains = null,
	   string filenameEndWith = null,
	   int? page = null,
	   int? pageSize = null);

	public TfRepositoryFile CreateRepositoryFile(
		string filename,
		string localPath,
		Guid? createdBy = null);

	public void UpdateRepositoryFile(
		string filename,
		string localPath,
		Guid? updatedBy = null);

	public void DeleteRepositoryFile(
		string filename);

	public byte[] GetRepositoryFileContentAsByteArray(
		string filename);

	public Stream GetRepositoryFileContentAsFileStream(
		string filename);
}

public partial class TfService : ITfService
{
	public TfRepositoryFile GetRepositoryFileByUri(
		string uriString)
	{
		if (string.IsNullOrEmpty(uriString))
		{
			throw new TfException("Invalid tefter uri. " +
				"It should start with: 'tefter://fs/repository/' ");
		}

		uriString = uriString.ToLowerInvariant();

		if (!uriString.StartsWith("tefter://fs/repository"))
		{
			throw new TfException("Invalid tefter uri. " +
				"It should start with: 'tefter://fs/repository/' ");
		}

		string filename = string.Empty;
		try
		{
			var uri = new Uri(uriString);

			if (uri.Segments.Length != 3)
				throw new TfException("Invalid tefter uri");

			filename = uri.Segments.Last();
		}
		catch (Exception ex)
		{
			throw new TfException("Invalid tefter uri", ex);
		}

		var file = _dboManager.Get<TfRepositoryFile>(
				" WHERE filename ILIKE @filename",
				new NpgsqlParameter("@filename", filename));

		return file;
	}

	public TfRepositoryFile GetRepositoryFile(
		string filename)
	{
		if (string.IsNullOrEmpty(filename))
			return null;

		var file = _dboManager.Get<TfRepositoryFile>(
				" WHERE filename ILIKE @filename",
				new NpgsqlParameter("@filename", filename));

		return file;
	}

	public List<TfRepositoryFile> GetRepositoryFiles(
		string filenameStartsWith = null,
		string filenameContains = null,
		string filenameEndWith = null,
		int? page = null,
		int? pageSize = null)
	{
		var pagingSql = GeneratePagingSql(page, pageSize);

		var orderBySql = " ORDER BY filename ASC ";
		var startsWithSql = " ( @starts_with IS NULL OR filename ILIKE @starts_with ) ";
		var containsSql = " ( @contains IS NULL OR filename ILIKE @contains ) ";
		var endsWithSql = " ( @ends_with IS NULL OR filename ILIKE @ends_with ) ";

		var sql = $"WHERE {startsWithSql} AND {containsSql} AND {endsWithSql} {orderBySql} {pagingSql}";

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

		return files;
	}

	public TfRepositoryFile CreateRepositoryFile(
		string filename,
		string localPath,
		Guid? createdBy = null)
	{
		new TfRepositoryServiceValidator(this)
			.ValidateCreate(filename, localPath)
			.ToValidationException()
			.ThrowIfContainsErrors();

		DateTime now = DateTime.Now;

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{

			var blobId = CreateBlob(localPath);


			TfRepositoryFile file = new TfRepositoryFile
			{
				Id = blobId,
				Filename = filename,
				CreatedBy = createdBy,
				CreatedOn = now,
				LastModifiedBy = createdBy,
				LastModifiedOn = now
			};

			var insertResult = _dboManager.Insert<TfRepositoryFile>(file);
			if (!insertResult)
				throw new TfDboServiceException("Insert repository file record into database failed");

			scope.Complete();

			return file;
		}
	}

	public void UpdateRepositoryFile(
		string filename,
		string localPath,
		Guid? updatedBy = null)
	{
		new TfRepositoryServiceValidator(this)
			.ValidateUpdate(filename, localPath)
			.ToValidationException()
			.ThrowIfContainsErrors();

		DateTime now = DateTime.Now;

		using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
		{
			var repFile = GetRepositoryFile(filename);

			repFile.LastModifiedBy = updatedBy;

			repFile.LastModifiedOn = DateTime.Now;

			var updateResult = _dboManager.Update<TfRepositoryFile>(repFile);

			if (!updateResult)
				throw new TfDboServiceException("Update repository file record into database failed");

			UpdateBlob(repFile.Id, localPath);

			scope.Complete();
		}
	}


	public void DeleteRepositoryFile(
		string filename)
	{

		new TfRepositoryServiceValidator(this)
			.ValidateDelete(filename)
			.ToValidationException()
			.ThrowIfContainsErrors();


		using (var scope = _dbService.CreateTransactionScope())
		{
			var existingFile = GetRepositoryFile(filename);

			var deleteResult = _dboManager.Delete<TfRepositoryFile>(existingFile.Id);
			if (!deleteResult)
				throw new TfDboServiceException("Failed to delete file from database.");

			DeleteBlob(existingFile.Id);

			scope.Complete();
		}
	}

	public byte[] GetRepositoryFileContentAsByteArray(
		string filename)
	{
		using var contentStream = GetRepositoryFileContentAsFileStream(filename);
		return new BinaryReader(contentStream)
				.ReadBytes((int)contentStream.Length);
	}

	public Stream GetRepositoryFileContentAsFileStream(
		string filename)
	{
		new TfRepositoryServiceValidator(this)
			.ValidateFileContentStream(filename)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var file = GetRepositoryFile(filename);
		return GetBlobStream(file.Id);
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
		private readonly ITfService _tfService;

		public TfRepositoryServiceValidator(
			ITfService tfService)
		{
			_tfService = tfService;
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

			var fileExists = _tfService.GetRepositoryFile(filename) != null;

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

			var fileExists = _tfService.GetRepositoryFile(filename) != null;

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

			var fileToDelete = _tfService.GetRepositoryFile(filename);

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

			var file = _tfService.GetRepositoryFile(filename);

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
