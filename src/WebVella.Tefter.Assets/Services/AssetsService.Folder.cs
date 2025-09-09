namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
    event EventHandler<AssetsFolder> FolderCreated;
    event EventHandler<AssetsFolder> FolderUpdated;
    event EventHandler<AssetsFolder> FolderDeleted;
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
    #region << Events >>
    public event EventHandler<AssetsFolder> FolderCreated = default!;
    public event EventHandler<AssetsFolder> FolderUpdated = default!;
    public event EventHandler<AssetsFolder> FolderDeleted = default!;
    #endregion


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
        var SQL = "SELECT * FROM assets_folder ORDER BY name";

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

        new AssetFolderValidator(this, _tfService)
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
        folder = GetFolder(folder.Id);
        FolderCreated?.Invoke(this, folder);
        return folder;
    }

    public AssetsFolder UpdateFolder(
        AssetsFolder folder)
    {
        if (folder == null)
            throw new NullReferenceException("Folder object is null");

        new AssetFolderValidator(this, _tfService)
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

        folder = GetFolder(folder.Id);
        FolderUpdated?.Invoke(this, folder);
        return folder;
    }

    public void DeleteFolder(
        Guid folderId)
    {
        var existingFolder = GetFolder(folderId);

        new AssetFolderValidator(this, _tfService)
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

        FolderDeleted?.Invoke(this, existingFolder);
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
        private readonly IAssetsService _assetService;
        private readonly ITfService _tfService;
        public AssetFolderValidator(
            IAssetsService assetService,
            ITfService tfService)
        {

            _assetService = assetService;
            _tfService = tfService;

            RuleSet("general", () =>
            {
                RuleFor(folder => folder.Id)
                    .NotEmpty()
                    .WithMessage("The folder id is required.");

                RuleFor(folder => folder.Name)
                    .NotEmpty()
                    .WithMessage("The folder name is required.");

                RuleFor(folder => folder.DataIdentity)
                   .NotEmpty()
                   .WithMessage("The folder data identity is required.");

                RuleFor(folder => folder.CountSharedColumnName)
                    .NotEmpty()
                    .WithMessage("The folder shared column count is required.");

            });

            RuleSet("create", () =>
            {
                RuleFor(folder => folder.Id)
                        .Must((folder, id) => { return _assetService.GetFolder(id) == null; })
                        .WithMessage("There is already existing folder with specified identifier.");

                RuleFor(folder => folder.Name)
                        .Must((folder, name) =>
                        {
                            if (string.IsNullOrEmpty(name))
                                return true;

                            var folders = _assetService.GetFolders();
                            return !folders.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
                        })
                        .WithMessage("There is already existing folder with same name.");

                RuleFor(folder => folder.DataIdentity)
                      .Must((folder, dataIdentityName) =>
                      {
                          if (string.IsNullOrEmpty(dataIdentityName))
                              return true;

                          var dataIdentityFound = _tfService.GetDataIdentity(dataIdentityName);
                          return dataIdentityFound != null;
                      })
                      .WithMessage("There is no data identity with specified name.");

                RuleFor(folder => folder.CountSharedColumnName)
                        .Must((folder, sharedColumnName) =>
                        {
                            if (string.IsNullOrEmpty(sharedColumnName))
                                return true;

                            var sharedColumn = _tfService.GetSharedColumn(sharedColumnName);
                            return sharedColumn != null;
                        })
                        .WithMessage("There is no shared column for count with specified name.");

                RuleFor(folder => folder.CountSharedColumnName)
                       .Must((folder, sharedColumnName) =>
                       {
                           if (string.IsNullOrEmpty(sharedColumnName))
                               return true;

                           var sharedColumn = _tfService.GetSharedColumn(sharedColumnName);
                           if (sharedColumn is null)
                               return true;

                           return sharedColumn.DataIdentity == folder.DataIdentity;
                       })
                       .WithMessage("The selected folder data identity is different to shared column for count data identity.");
            });

            RuleSet("update", () =>
            {
                RuleFor(folder => folder.Id)
                        .Must((folder, id) =>
                        {
                            return _assetService.GetFolder(id) != null;
                        })
                        .WithMessage("There is not existing folder with specified identifier.");

                RuleFor(folder => folder.Name)
                        .Must((folder, name) =>
                        {
                            if (string.IsNullOrEmpty(name))
                                return true;

                            var folders = _assetService.GetFolders();
                            return !folders.Any(x =>
                                x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim() &&
                                x.Id != folder.Id
                            );
                        })
                        .WithMessage("There is already existing another folder with same name.");

                RuleFor(folder => folder.DataIdentity)
                    .Must((folder, dataIdentityName) =>
                    {
                        if (string.IsNullOrEmpty(dataIdentityName))
                            return true;

                        var dataIdentityFound = _tfService.GetDataIdentity(dataIdentityName);
                        return dataIdentityFound != null;
                    })
                    .WithMessage("There is no data identity with specified name.");


                RuleFor(folder => folder.CountSharedColumnName)
                       .Must((folder, sharedColumnName) =>
                       {
                           if (string.IsNullOrEmpty(sharedColumnName))
                               return true;

                           var sharedColumn = _tfService.GetSharedColumn(sharedColumnName);
                           return sharedColumn != null;
                       })
                       .WithMessage("There is no shared column for count with specified name.");

                RuleFor(folder => folder.CountSharedColumnName)
                       .Must((folder, sharedColumnName) =>
                       {
                           if (string.IsNullOrEmpty(sharedColumnName))
                               return true;

                           var sharedColumn = _tfService.GetSharedColumn(sharedColumnName);
                           if (sharedColumn is null)
                               return true;

                           return sharedColumn.DataIdentity == folder.DataIdentity;
                       })
                       .WithMessage("The selected folder data identity is different to shared column for count data identity.");

            });

            RuleSet("delete", () =>
            {
                RuleFor(folder => folder.Id)
                    .Must((folder, id) =>
                    {
                        return _assetService.GetFolder(id) != null;
                    })
                    .WithMessage("There is not existing folder with specified identifier.");

                RuleFor(folder => folder.Id)
                    .Must((folder, id) =>
                    {
                        return _assetService.GetAssets(id).Count == 0;
                    })
                    .WithMessage("There are existing assets in the folder.");
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
