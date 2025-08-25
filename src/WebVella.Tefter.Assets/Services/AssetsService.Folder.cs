namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
	AssetsFolder GetFolder(
		Guid folderId);

	List<AssetsFolder> GetFolders();

	AssetsFolder CreateFolder(
		AssetsFolder folder);

	AssetsFolder UpdateFolder(
		AssetsFolder folder);

	void DeleteFolder(
		Guid folderId);
}

internal partial class AssetsService : IAssetsService
{
	public AssetsFolder GetFolder(
		Guid folderId)
	{
		var SQL = "SELECT * FROM assets_folder WHERE id = @id";

		var dt = _dbService.ExecuteSqlQueryCommand(SQL,
			new NpgsqlParameter("id", folderId));

		if (dt.Rows.Count == 0)
			return null;

		return ToFolder(dt.Rows[0]);
	}

	public List<AssetsFolder> GetFolders()
	{
		var SQL = "SELECT * FROM assets_folder";

		var dt = _dbService.ExecuteSqlQueryCommand(SQL);

		List<AssetsFolder> folders = new List<AssetsFolder>();

		foreach (DataRow row in dt.Rows)
			folders.Add(ToFolder(row));

		return folders;
	}

	public AssetsFolder CreateFolder(
		AssetsFolder folder)
	{
		if (folder == null)
			throw new NullReferenceException("Folder object is null");

		if (folder.Id == Guid.Empty)
			folder.Id = Guid.NewGuid();

		new AssetFolderValidator(this)
			.ValidateCreate(folder)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = "INSERT INTO assets_folder(id,name,data_identity,count_shared_column_name) " +
			"VALUES( @id,@name,@data_identity,@count_shared_column_name)";

		var idPar = CreateParameter(
			"id",
			folder.Id,
			DbType.Guid);

		var namePar = CreateParameter(
			"name",
			folder.Name,
			DbType.StringFixedLength);

		var dataIdentityPar = CreateParameter(
			"data_identity",
			folder.DataIdentity,
			DbType.StringFixedLength);

		var countSharedColumnNamePar = CreateParameter(
			"count_shared_column_name",
			folder.CountSharedColumnName,
			DbType.StringFixedLength);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			idPar,
			namePar,
			dataIdentityPar,
			countSharedColumnNamePar);

		if (dbResult != 1)
			throw new Exception("Failed to insert new row in database for folder object");

		return GetFolder(folder.Id);
	}

	public AssetsFolder UpdateFolder(
		AssetsFolder folder)
	{
		if (folder == null)
			throw new NullReferenceException("Folder object is null");

		new AssetFolderValidator(this)
			.ValidateUpdate(folder)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = "UPDATE assets_folder SET " +
			"name=@name, " +
            "data_identity=@data_identity, " +
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

		var dataIdentityPar = CreateParameter(
			"data_identity",
			folder.DataIdentity,
			DbType.StringFixedLength);

		var countSharedColumnNamePar = CreateParameter(
			"count_shared_column_name",
			folder.CountSharedColumnName,
			DbType.StringFixedLength);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			idPar,
			namePar,
			dataIdentityPar,
			countSharedColumnNamePar);

		if (dbResult != 1)
			throw new Exception("Failed to update row in database for channel object");

		return GetFolder(folder.Id);
	}

	public void DeleteFolder(
		Guid folderId)
	{
		var existingFolder = GetFolder(folderId);

		new AssetFolderValidator(this)
			.ValidateDelete(existingFolder)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = "DELETE FROM assets_folder WHERE id = @id";

		var idPar = CreateParameter(
			"id",
			folderId,
			DbType.Guid);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

		if (dbResult != 1)
			throw new Exception("Failed to delete row in database for folder object");
	}

	private AssetsFolder ToFolder(DataRow dr)
	{
		if (dr == null)
			throw new Exception("DataRow is null");

		return new AssetsFolder
		{
			Id = dr.Field<Guid>("id"),
			Name = dr.Field<string>("name") ?? string.Empty,
			DataIdentity = dr.Field<string>("data_identity"),
			CountSharedColumnName = dr.Field<string>("count_shared_column_name")
		};
	}

	#region <--- validation --->

	internal class AssetFolderValidator
		: AbstractValidator<AssetsFolder>
	{
		public AssetFolderValidator(IAssetsService service)
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
						.Must((folder, id) => { return service.GetFolder(id) == null; })
						.WithMessage("There is already existing folder with specified identifier.");

				RuleFor(folder => folder.Name)
						.Must((folder, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var folders = service.GetFolders();
							return !folders.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing folder with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(folder => folder.Id)
						.Must((folder, id) =>
						{
							return service.GetFolder(id) != null;
						})
						.WithMessage("There is not existing folder with specified identifier.");

				RuleFor(folder => folder.Name)
						.Must((folder, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var folders = service.GetFolders();
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
						return service.GetFolder(id) != null;
					})
					.WithMessage("There is not existing folder with specified identifier.");
			});

		}

		public ValidationResult ValidateCreate(
			AssetsFolder folder)
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
			AssetsFolder folder)
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
			AssetsFolder folder)
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
