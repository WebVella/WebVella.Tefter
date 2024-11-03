namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetService
{
	Result<Folder> GetFolder(
		Guid folderId);

	Result<List<Folder>> GetFolders();

	Result<Folder> CreateFolder(
		Folder folder);

	Result<Folder> UpdateFolder(
		Folder folder);

	Result DeleteFolder(
		Guid folderId);
}

internal partial class AssetService : IAssetService
{
	public Result<Folder> GetFolder(
		Guid folderId)
	{
		try
		{
			var SQL = "SELECT * FROM assets_folder WHERE id = @id";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL,
				new NpgsqlParameter("id", folderId));

			if (dt.Rows.Count == 0)
				return Result.Ok((Folder)null);

			return Result.Ok((Folder)ToFolder(dt.Rows[0]));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get folder.").CausedBy(ex));
		}
	}

	public Result<List<Folder>> GetFolders()
	{
		try
		{
			var SQL = "SELECT * FROM assets_folder";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL);

			List<Folder> folders = new List<Folder>();

			foreach (DataRow row in dt.Rows)
				folders.Add(ToFolder(row));

			return Result.Ok(folders);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get folders.").CausedBy(ex));
		}
	}

	public Result<Folder> CreateFolder(
		Folder folder)
	{
		try
		{
			if (folder == null)
				throw new NullReferenceException("Folder object is null");

			if (folder.Id == Guid.Empty)
				folder.Id = Guid.NewGuid();

			AssetFolderValidator validator = new AssetFolderValidator(this);

			var validationResult = validator.ValidateCreate(folder);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var SQL = "INSERT INTO assets_folder(id,name,shared_key,count_shared_column_name) " +
				"VALUES( @id,@name,@shared_key,@count_shared_column_name)";

			var idPar = CreateParameter(
				"id",
				folder.Id,
				DbType.Guid);

			var namePar = CreateParameter(
				"name",
				folder.Name,
				DbType.StringFixedLength);

			var sharedKeyPar = CreateParameter(
				"shared_key",
				folder.SharedKey,
				DbType.StringFixedLength);

			var countSharedColumnNamePar = CreateParameter(
				"count_shared_column_name",
				folder.CountSharedColumnName,
				DbType.StringFixedLength);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar,
				namePar,
				sharedKeyPar,
				countSharedColumnNamePar);

			if (dbResult != 1)
				throw new Exception("Failed to insert new row in database for folder object");

			var insertedFolderResult = GetFolder(folder.Id);

			if (!insertedFolderResult.IsSuccess || insertedFolderResult.Value is null)
				throw new Exception("Failed to get newly create folder in database");

			return Result.Ok(insertedFolderResult.Value);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new channel.").CausedBy(ex));
		}
	}

	public Result<Folder> UpdateFolder(
		Folder folder)
	{
		try
		{
			if (folder == null)
				throw new NullReferenceException("Folder object is null");

			AssetFolderValidator validator = new AssetFolderValidator(this);

			var validationResult = validator.ValidateUpdate(folder);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var SQL = "UPDATE assets_folder SET " +
				"name=@name, " +
				"shared_key=@shared_key, " +
				"count_shared_column_name=@count_shared_column_name " +
				"WHERE id = @id";

			var idPar = CreateParameter(
				"id",
				folder.Id,
				DbType.Guid);

			var namePar = CreateParameter(
				"name",
				folder.Name,
				DbType.StringFixedLength);

			var sharedKeyPar = CreateParameter(
				"shared_key",
				folder.SharedKey,
				DbType.StringFixedLength);

			var countSharedColumnNamePar = CreateParameter(
				"count_shared_column_name",
				folder.CountSharedColumnName,
				DbType.StringFixedLength);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar,
				namePar,
				sharedKeyPar,
				countSharedColumnNamePar);

			if (dbResult != 1)
				throw new Exception("Failed to update row in database for channel object");

			return Result.Ok(GetFolder(folder.Id).Value);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update folder.").CausedBy(ex));
		}
	}

	public Result DeleteFolder(
		Guid folderId)
	{
		try
		{

			var existingFolder = GetFolder(folderId).Value;

			AssetFolderValidator validator = new AssetFolderValidator(this);

			var validationResult = validator.ValidateDelete(existingFolder);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var SQL = "DELETE FROM assets_folder WHERE id = @id";

			var idPar = CreateParameter(
				"id",
				folderId,
				DbType.Guid);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

			if (dbResult != 1)
				throw new Exception("Failed to delete row in database for folder object");

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete folder.").CausedBy(ex));
		}
	}

	private Folder ToFolder(DataRow dr)
	{
		if (dr == null)
			throw new Exception("DataRow is null");

		return new Folder
		{
			Id = dr.Field<Guid>("id"),
			Name = dr.Field<string>("name") ?? string.Empty,
			SharedKey = dr.Field<string>("shared_key"),
			CountSharedColumnName = dr.Field<string>("count_shared_column_name")
		};
	}

	#region <--- validation --->

	internal class AssetFolderValidator
		: AbstractValidator<Folder>
	{
		public AssetFolderValidator(IAssetService service)
		{

			RuleSet("general", () =>
			{
				RuleFor(folder => folder.Id)
					.NotEmpty()
					.WithMessage("The folder id is required.");

				RuleFor(folder => folder.Name)
					.NotEmpty()
					.WithMessage("The folder name is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(folder => folder.Id)
						.Must((folder, id) => { return service.GetFolder(id).Value == null; })
						.WithMessage("There is already existing folder with specified identifier.");

				RuleFor(folder => folder.Name)
						.Must((folder, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var folders = service.GetFolders().Value;
							return !folders.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing folder with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(folder => folder.Id)
						.Must((folder, id) =>
						{
							return service.GetFolder(id).Value != null;
						})
						.WithMessage("There is not existing folder with specified identifier.");

				RuleFor(folder => folder.Name)
						.Must((folder, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var folders = service.GetFolders().Value;
							return !folders.Any(x =>
								x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim() &&
								x.Id != folder.Id
							);
						})
						.WithMessage("There is already existing another folder with same name.");

			});

			RuleSet("delete", () =>
			{
				RuleFor(folder => folder.Id)
					.Must((folder, id) =>
					{
						return service.GetFolder(id).Value != null;
					})
					.WithMessage("There is not existing folder with specified identifier.");
			});

		}

		public ValidationResult ValidateCreate(
			Folder folder)
		{
			if (folder == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The folder object is null.") });

			return this.Validate(folder, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			Folder folder)
		{
			if (folder == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The folder object is null.") });

			return this.Validate(folder, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			Folder folder)
		{
			if (folder == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"A folder with specified identifier is not found.") });

			return this.Validate(folder, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
