using System.Text;

namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{

    event EventHandler<Asset> AssetCreated;
    event EventHandler<Asset> AssetUpdated;
    event EventHandler<Asset> AssetDeleted;

    public Asset GetAsset(
        Guid id);

    public List<Asset> GetAssets(
        Guid? folderId = null,
        string dataIdentityValue = null,
        string search = null);

    public Asset CreateFileAsset(
        CreateFileAssetWithRowIdModel asset);

    public Asset CreateFileAsset(
        CreateFileAssetWithDataIdentityModel asset);

    public Asset CreateLinkAsset(
        CreateLinkAssetWithRowIdModel asset);

    public Asset CreateLinkAsset(
        CreateLinkAssetWithDataIdentityModel asset);

    public Asset UpdateFileAsset(
        Guid id,
        string label,
        string localPath,
        Guid userId);

    public Asset UpdateLinkAsset(
        Guid id,
        string label,
        string url,
        string iconUrl,
        Guid userId);

    public void DeleteAsset(
        Guid assetId);

    //public void UpdateFoldersSharedColumnCount();

    //public void UpdateFolderSharedColumnCount(
    //  AssetsFolder folder);

    public void ModifyAssetSharedColumnCount(
       Asset asset,
       bool isIncrement);
}

internal partial class AssetsService : IAssetsService
{

    #region << Events >>
    public event EventHandler<Asset> AssetCreated = default!;
    public event EventHandler<Asset> AssetUpdated = default!;
    public event EventHandler<Asset> AssetDeleted = default!;
    #endregion

    public Asset GetAsset(
        Guid id)
    {

        string SQL = @"SELECT id, folder_id FROM assets_asset WHERE id = @id";

        var assetIdPar = CreateParameter(
            "id",
            id,
            DbType.Guid);

        var dt = _dbService.ExecuteSqlQueryCommand(SQL, assetIdPar);
        if (dt.Rows.Count == 0)
            return null;

        Guid folderId = (Guid)dt.Rows[0]["folder_id"];
        var folder = GetFolder(folderId);

        string folderDataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;
        if (!string.IsNullOrWhiteSpace(folder.DataIdentity))
            folderDataIdentity = folder.DataIdentity;

        SQL =
$@"

WITH sk_identity_info AS (
	SELECT trs.id, JSON_AGG( dic.* ) AS json_result
	FROM assets_asset trs
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trs.identity_row_id AND dic.data_identity_2 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_1 = '{folderDataIdentity}' ) OR
			( dic.value_1 = trs.identity_row_id AND dic.data_identity_1 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_2 = '{folderDataIdentity}' ) 
	GROUP BY trs.id
)
SELECT 
	aa.id,
	aa.folder_id,
	aa.type,
	aa.content_json,
	aa.created_by,
	aa.created_on,
	aa.modified_by,
	aa.modified_on,
	sk_identity_info.json_result AS identity_connection_json
FROM assets_asset aa
	LEFT OUTER JOIN sk_identity_info ON aa.id = sk_identity_info.id
WHERE aa.id = @id
";

        assetIdPar = CreateParameter(
            "id",
            id,
            DbType.Guid);

        dt = _dbService.ExecuteSqlQueryCommand(SQL, assetIdPar);

        List<Asset> assets = ToAssetList(dt);

        if (assets.Count == 0)
        {
            return null;
        }

        return assets[0];
    }

    public List<Asset> GetAssets(
        Guid? folderId,
        string dataIdentityValue = null,
        string search = null)
    {
        string folderDataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;
        if (folderId is not null)
        {
            var folder = GetFolder(folderId.Value);
            if (folder is null)
                throw new Exception($"Failed to find folder with id '{folderId}'");
            else if (!string.IsNullOrWhiteSpace(folder.DataIdentity))
                folderDataIdentity = folder.DataIdentity;
        }

        string SQL = $@"
WITH sk_identity_info AS (
	SELECT trs.id, JSON_AGG( dic.* ) AS json_result
	FROM assets_asset trs
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trs.identity_row_id AND dic.data_identity_2 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_1 = '{folderDataIdentity}' ) OR
			( dic.value_1 = trs.identity_row_id AND dic.data_identity_1 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_2 = '{folderDataIdentity}' ) 
	GROUP BY trs.id
),
sk_identity_filter AS (
	SELECT trf.id
	FROM assets_asset trf
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trf.identity_row_id AND dic.data_identity_2 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_1 = '{folderDataIdentity}' ) OR
			( dic.value_1 = trf.identity_row_id AND dic.data_identity_1 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_2 = '{folderDataIdentity}' ) 
	WHERE ( @data_identity_value IS NULL OR ( dic.value_1 = @data_identity_value OR dic.value_2 = @data_identity_value ) )
	GROUP BY trf.id
)
SELECT 
	aa.id,
	aa.folder_id,
	aa.type,
	aa.content_json,
	aa.created_by,
	aa.created_on,
	aa.modified_by,
	aa.modified_on,
	sk.json_result AS identity_connection_json
FROM assets_asset aa
	LEFT OUTER JOIN sk_identity_info sk ON aa.id = sk.id
    LEFT OUTER JOIN sk_identity_filter sf ON aa.id = sf.id
WHERE ( @folder_id IS NULL OR aa.folder_id = @folder_id ) AND
	  ( @x_search IS NULL OR aa.x_search ILIKE CONCAT ('%', @x_search, '%')  ) AND
	  sf.id is not null AND sk.id is not null
ORDER BY aa.created_on DESC;";

        var folderIdPar = CreateParameter(
            "folder_id",
            folderId,
            DbType.Guid);

        var skIdPar = CreateParameter(
            "data_identity_value",
            dataIdentityValue,
            DbType.String);

        string x_search = null;
        if (!string.IsNullOrWhiteSpace(search))
            x_search = search.Trim();

        var skXSearch = CreateParameter(
            "x_search",
            x_search,
            DbType.String);

        var dt = _dbService.ExecuteSqlQueryCommand(SQL, folderIdPar, skIdPar, skXSearch);

        return ToAssetList(dt);
    }

    public Asset CreateFileAsset(
        CreateFileAssetWithRowIdModel asset)
    {
        Guid id = Guid.NewGuid();

        new AssetValidator(this, _tfService)
            .ValidateCreateFileAsset(asset, id)
            .ToValidationException()
            .ThrowIfContainsErrors();

        using (var scope = _dbService.CreateTransactionScope())
        {
            var dataProvider = _tfService.GetDataProvider(asset.DataProviderId);

            AssetsFolder folder = GetFolder(asset.FolderId);

            TfDataIdentity folderDataIdentity = null;
            if (!string.IsNullOrWhiteSpace(folder.DataIdentity))
            {
                folderDataIdentity = _tfService.GetDataIdentity(folder.DataIdentity);
                if (folderDataIdentity is null)
                    throw new Exception($"Failed to find data identity '{folder.DataIdentity}' for folder");
            }

            if (folderDataIdentity is null)
                folderDataIdentity = _tfService.GetDataIdentity(TfConstants.TF_ROW_ID_DATA_IDENTITY);


            string filename = asset.FileName;
            if (string.IsNullOrWhiteSpace(filename))
                filename = Path.GetFileName(asset.LocalPath);

            Guid blobId = _tfService.CreateBlob(asset.LocalPath);

            DateTime now = DateTime.Now;

            var SQL = @"INSERT INTO assets_asset
						(id, folder_id, type, content_json, created_by,
						created_on, modified_by, modified_on, x_search)
					VALUES(@id, @folder_id, @type, @content_json, @created_by,
						@created_on, @modified_by, @modified_on, @x_search); ";

            var idPar = CreateParameter("@id", id, DbType.Guid);

            var folderIdPar = CreateParameter("@folder_id", asset.FolderId, DbType.Guid);

            var typePar = CreateParameter("@type", (short)AssetType.File, DbType.Int16);

            FileAssetContent content = new FileAssetContent
            {
                BlobId = blobId,
                Filename = filename,
                Label = asset.Label,
                DownloadUrl = $"/fs/assets/{id}/{filename}"
            };

            var contentJson = JsonSerializer.Serialize(content);

            var contentJsonPar = CreateParameter("@content_json", contentJson, DbType.String);

            var createdByPar = CreateParameter("@created_by", asset.CreatedBy, DbType.Guid);

            var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

            var modifiedByPar = CreateParameter("@modified_by", asset.CreatedBy, DbType.Guid);

            var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

            var xSearchPar = CreateParameter("@x_search", $"{asset.Label} {filename}", DbType.String);

            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                SQL,
                idPar, folderIdPar,
                typePar, contentJsonPar,
                createdByPar, createdOnPar,
                modifiedByPar, modifiedOnPar,
                xSearchPar);

            if (dbResult != 1)
                throw new Exception("Failed to insert new row in database for thread object");

            if (asset.RowIds != null && asset.RowIds.Count > 0)
            {
                var assetIdentityRowId = id.ToSha1();

                var rowIdentityValuesDict = _tfService.GetDataIdentityValuesForRowIds(dataProvider, folderDataIdentity, asset.RowIds);

                HashSet<string> dataIdentityValues = new HashSet<string>();
                foreach (var rowId in asset.RowIds)
                {
                    if (rowIdentityValuesDict.ContainsKey(rowId))
                    {
                        var identityValue = rowIdentityValuesDict[rowId];
                        if (!dataIdentityValues.Contains(identityValue))
                            dataIdentityValues.Add(identityValue);
                    }
                }

                foreach (var dataIdentityValue in dataIdentityValues)
                {
                    _tfService.CreateDataIdentityConnection(new TfDataIdentityConnection
                    {
                        DataIdentity1 = folderDataIdentity.DataIdentity,
                        Value1 = dataIdentityValue,
                        DataIdentity2 = TfConstants.TF_ROW_ID_DATA_IDENTITY,
                        Value2 = assetIdentityRowId
                    });
                }
            }

            scope.Complete();

            var resultAsset = GetAsset(id);

            ModifyAssetSharedColumnCount(resultAsset, isIncrement: true);

            AssetCreated?.Invoke(this, resultAsset);

            return resultAsset;
        }
    }


    public Asset CreateFileAsset(
        CreateFileAssetWithDataIdentityModel asset)
    {
        Guid id = Guid.NewGuid();

        new AssetValidator(this, _tfService)
            .ValidateCreateFileAsset(asset, id)
            .ToValidationException()
            .ThrowIfContainsErrors();

        using (var scope = _dbService.CreateTransactionScope())
        {
            AssetsFolder folder = GetFolder(asset.FolderId);

            string filename = asset.FileName;
            if (string.IsNullOrWhiteSpace(filename))
                filename = Path.GetFileName(asset.LocalPath);

            Guid blobId = _tfService.CreateBlob(asset.LocalPath);

            DateTime now = DateTime.Now;

            var SQL = @"INSERT INTO assets_asset
						(id, folder_id, type, content_json, created_by,
						created_on, modified_by, modified_on, x_search)
					VALUES(@id, @folder_id, @type, @content_json, @created_by,
						@created_on, @modified_by, @modified_on, @x_search); ";

            var idPar = CreateParameter("@id", id, DbType.Guid);

            var folderIdPar = CreateParameter("@folder_id", asset.FolderId, DbType.Guid);

            var typePar = CreateParameter("@type", (short)AssetType.File, DbType.Int16);

            FileAssetContent content = new FileAssetContent
            {
                BlobId = blobId,
                Filename = filename,
                Label = asset.Label,
                DownloadUrl = $"/fs/assets/{id}/{filename}"
            };

            var contentJson = JsonSerializer.Serialize(content);

            var contentJsonPar = CreateParameter("@content_json", contentJson, DbType.String);

            var createdByPar = CreateParameter("@created_by", asset.CreatedBy, DbType.Guid);

            var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

            var modifiedByPar = CreateParameter("@modified_by", asset.CreatedBy, DbType.Guid);

            var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

            var xSearchPar = CreateParameter("@x_search", $"{asset.Label} {filename}", DbType.String);

            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                SQL,
                idPar, folderIdPar,
                typePar, contentJsonPar,
                createdByPar, createdOnPar,
                modifiedByPar, modifiedOnPar,
                xSearchPar);

            if (dbResult != 1)
                throw new Exception("Failed to insert new row in database for thread object");

            if (asset.DataIdentityValues != null && asset.DataIdentityValues.Count > 0)
            {
                var assetIdentityRowId = id.ToSha1();

                TfDataIdentity folderDataIdentity = null;
                if (!string.IsNullOrWhiteSpace(folder.DataIdentity))
                {
                    folderDataIdentity = _tfService.GetDataIdentity(folder.DataIdentity);
                    if (folderDataIdentity is null)
                        throw new Exception($"Failed to find data identity '{folder.DataIdentity}' for folder");
                }

                if (folderDataIdentity is null)
                    folderDataIdentity = _tfService.GetDataIdentity(TfConstants.TF_ROW_ID_DATA_IDENTITY);

                foreach (var dataIdentityValue in asset.DataIdentityValues)
                {
                    if (!dataIdentityValue.IsSha1())
                        throw new Exception($"Data identity value '{dataIdentityValue}' is not a valid SHA1 value");

                    _tfService.CreateDataIdentityConnection(new TfDataIdentityConnection
                    {
                        DataIdentity1 = folderDataIdentity.DataIdentity,
                        Value1 = dataIdentityValue,
                        DataIdentity2 = TfConstants.TF_ROW_ID_DATA_IDENTITY,
                        Value2 = assetIdentityRowId
                    });
                }
            }

            scope.Complete();

            var resultAsset = GetAsset(id);

            ModifyAssetSharedColumnCount(resultAsset, isIncrement: true);

            AssetCreated?.Invoke(this, resultAsset);
            return resultAsset;
        }
    }

    public Asset CreateLinkAsset(
        CreateLinkAssetWithDataIdentityModel asset)
    {
        Guid id = Guid.NewGuid();

        new AssetValidator(this, _tfService)
            .ValidateCreateLinkAsset(asset, id)
            .ToValidationException()
            .ThrowIfContainsErrors();

        using (var scope = _dbService.CreateTransactionScope())
        {
            DateTime now = DateTime.Now;

            AssetsFolder folder = GetFolder(asset.FolderId);

            var SQL = @"INSERT INTO assets_asset
						(id, folder_id, type, content_json, created_by,
						created_on, modified_by, modified_on, x_search)
					VALUES(@id, @folder_id, @type, @content_json, @created_by,
						@created_on, @modified_by, @modified_on, @x_search); ";

            var idPar = CreateParameter("@id", id, DbType.Guid);

            var folderIdPar = CreateParameter("@folder_id", asset.FolderId, DbType.Guid);

            var typePar = CreateParameter("@type", (short)AssetType.Link, DbType.Int16);

            LinkAssetContent content = new LinkAssetContent
            {
                Url = asset.Url,
                Label = asset.Label,
                IconUrl = asset.IconUrl
            };

            var contentJson = JsonSerializer.Serialize(content);

            var contentJsonPar = CreateParameter("@content_json", contentJson, DbType.String);

            var createdByPar = CreateParameter("@created_by", asset.CreatedBy, DbType.Guid);

            var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

            var modifiedByPar = CreateParameter("@modified_by", asset.CreatedBy, DbType.Guid);

            var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

            var xSearchPar = CreateParameter("@x_search", $"{asset.Label} {asset.Url}", DbType.String);

            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                SQL,
                idPar, folderIdPar,
                typePar, contentJsonPar,
                createdByPar, createdOnPar,
                modifiedByPar, modifiedOnPar,
                xSearchPar);

            if (dbResult != 1)
            {
                throw new Exception("Failed to insert new row in database for thread object");
            }

            if (asset.DataIdentityValues != null && asset.DataIdentityValues.Count > 0)
            {
                var assetIdentityRowId = id.ToSha1();

                TfDataIdentity folderDataIdentity = null;
                if (!string.IsNullOrWhiteSpace(folder.DataIdentity))
                {
                    folderDataIdentity = _tfService.GetDataIdentity(folder.DataIdentity);
                    if (folderDataIdentity is null)
                        throw new Exception($"Failed to find data identity '{folder.DataIdentity}' for folder");
                }

                if (folderDataIdentity is null)
                    folderDataIdentity = _tfService.GetDataIdentity(TfConstants.TF_ROW_ID_DATA_IDENTITY);

                foreach (var dataIdentityValue in asset.DataIdentityValues)
                {
                    if (!dataIdentityValue.IsSha1())
                        throw new Exception($"Data identity value '{dataIdentityValue}' is not a valid SHA1 value");

                    _tfService.CreateDataIdentityConnection(new TfDataIdentityConnection
                    {
                        DataIdentity1 = folderDataIdentity.DataIdentity,
                        Value1 = dataIdentityValue,
                        DataIdentity2 = TfConstants.TF_ROW_ID_DATA_IDENTITY,
                        Value2 = assetIdentityRowId
                    });
                }
            }

            scope.Complete();

            var resultAsset = GetAsset(id);

            ModifyAssetSharedColumnCount(resultAsset, isIncrement: true);

            AssetCreated?.Invoke(this, resultAsset);

            return resultAsset;
        }
    }

    public Asset CreateLinkAsset(
        CreateLinkAssetWithRowIdModel asset)
    {
        Guid id = Guid.NewGuid();

        new AssetValidator(this, _tfService)
            .ValidateCreateLinkAsset(asset, id)
            .ToValidationException()
            .ThrowIfContainsErrors();

        using (var scope = _dbService.CreateTransactionScope())
        {
            var dataProvider = _tfService.GetDataProvider(asset.DataProviderId);

            AssetsFolder folder = GetFolder(asset.FolderId);

            TfDataIdentity folderDataIdentity = null;
            if (!string.IsNullOrWhiteSpace(folder.DataIdentity))
            {
                folderDataIdentity = _tfService.GetDataIdentity(folder.DataIdentity);
                if (folderDataIdentity is null)
                    throw new Exception($"Failed to find data identity '{folder.DataIdentity}' for folder");
            }

            if (folderDataIdentity is null)
                folderDataIdentity = _tfService.GetDataIdentity(TfConstants.TF_ROW_ID_DATA_IDENTITY);

            DateTime now = DateTime.Now;

            var SQL = @"INSERT INTO assets_asset
						(id, folder_id, type, content_json, created_by,
						created_on, modified_by, modified_on, x_search)
					VALUES(@id, @folder_id, @type, @content_json, @created_by,
						@created_on, @modified_by, @modified_on, @x_search); ";

            var idPar = CreateParameter("@id", id, DbType.Guid);

            var folderIdPar = CreateParameter("@folder_id", asset.FolderId, DbType.Guid);

            var typePar = CreateParameter("@type", (short)AssetType.Link, DbType.Int16);

            LinkAssetContent content = new LinkAssetContent
            {
                Url = asset.Url,
                Label = asset.Label,
                IconUrl = asset.IconUrl
            };

            var contentJson = JsonSerializer.Serialize(content);

            var contentJsonPar = CreateParameter("@content_json", contentJson, DbType.String);

            var createdByPar = CreateParameter("@created_by", asset.CreatedBy, DbType.Guid);

            var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

            var modifiedByPar = CreateParameter("@modified_by", asset.CreatedBy, DbType.Guid);

            var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

            var xSearchPar = CreateParameter("@x_search", $"{asset.Label} {asset.Url}", DbType.String);

            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                SQL,
                idPar, folderIdPar,
                typePar, contentJsonPar,
                createdByPar, createdOnPar,
                modifiedByPar, modifiedOnPar,
                xSearchPar);

            if (dbResult != 1)
            {
                throw new Exception("Failed to insert new row in database for thread object");
            }

            if (asset.RowIds != null && asset.RowIds.Count > 0)
            {
                var assetIdentityRowId = id.ToSha1();

                var rowIdentityValuesDict = _tfService.GetDataIdentityValuesForRowIds(dataProvider, folderDataIdentity, asset.RowIds);

                HashSet<string> dataIdentityValues = new HashSet<string>();
                foreach (var rowId in asset.RowIds)
                {
                    if (rowIdentityValuesDict.ContainsKey(rowId))
                    {
                        var identityValue = rowIdentityValuesDict[rowId];
                        if (!dataIdentityValues.Contains(identityValue))
                            dataIdentityValues.Add(identityValue);
                    }
                }

                foreach (var dataIdentityValue in dataIdentityValues)
                {
                    _tfService.CreateDataIdentityConnection(new TfDataIdentityConnection
                    {
                        DataIdentity1 = folderDataIdentity.DataIdentity,
                        Value1 = dataIdentityValue,
                        DataIdentity2 = TfConstants.TF_ROW_ID_DATA_IDENTITY,
                        Value2 = assetIdentityRowId
                    });
                }
            }

            scope.Complete();

            var resultAsset = GetAsset(id);

            ModifyAssetSharedColumnCount(resultAsset, isIncrement: true);

            AssetCreated?.Invoke(this, resultAsset);

            return resultAsset;
        }
    }

    public Asset UpdateFileAsset(
        Guid id,
        string label,
        string localPath,
        Guid userId)
    {
        var existingAsset = GetAsset(id);

        var user = _tfService.GetUser(userId);

        new AssetValidator(this, _tfService)
            .ValidateUpdateFileAsset(
                existingAsset,
                label,
                localPath,
                user)
            .ToValidationException()
            .ThrowIfContainsErrors();


        using (var scope = _dbService.CreateTransactionScope())
        {
            var SQL = "UPDATE assets_asset SET " +
            "content_json=@content_json, " +
            "modified_on=@modified_on, " +
            "modified_by=@modified_by," +
            "x_search = @x_search " +
            "WHERE id = @id";

            var idPar = CreateParameter(
                "id",
                id,
                DbType.Guid);

            FileAssetContent fileAssetContent = new FileAssetContent
            {
                BlobId = ((FileAssetContent)existingAsset.Content).BlobId,
                Filename = ((FileAssetContent)existingAsset.Content).Filename,
                Label = label,
                DownloadUrl = ((FileAssetContent)existingAsset.Content).DownloadUrl
            };

            string contentJson = JsonSerializer.Serialize(fileAssetContent);

            var contentJsonPar = CreateParameter(
                "@content_json",
                contentJson,
                DbType.String);

            var modifiedOnPar = CreateParameter(
                "@modified_on",
                DateTime.Now,
                DbType.DateTime2);

            var modifiedByPar = CreateParameter(
                "@modified_by",
                user.Id,
                DbType.Guid);

            var xSearchPar = CreateParameter(
                "@x_search",
                $"{label} {((FileAssetContent)existingAsset.Content).Filename}",
                DbType.String);

            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                SQL,
                idPar,
                contentJsonPar,
                modifiedByPar,
                modifiedOnPar,
                xSearchPar);

            if (dbResult != 1)
                throw new Exception("Failed to update row in database for asset object");

            if (!string.IsNullOrWhiteSpace(localPath))
                _tfService.UpdateBlob(fileAssetContent.BlobId, localPath);

            scope.Complete();

            var resultAsset = GetAsset(id);
            AssetUpdated?.Invoke(this, resultAsset);
            return resultAsset;
        }
    }

    public Asset UpdateLinkAsset(
        Guid id,
        string label,
        string url,
        string iconUrl,
        Guid userId)
    {
        var existingAsset = GetAsset(id);

        var user = _tfService.GetUser(userId);

        new AssetValidator(this, _tfService)
            .ValidateUpdateLinkAsset(
                existingAsset,
                label,
                url,
                user)
            .ToValidationException()
            .ThrowIfContainsErrors();

        using (var scope = _dbService.CreateTransactionScope())
        {
            var SQL = "UPDATE assets_asset SET " +
            "content_json=@content_json, " +
            "modified_on=@modified_on, " +
            "modified_by=@modified_by, " +
            "x_search = @x_search " +
            "WHERE id = @id";

            var idPar = CreateParameter(
                "id",
                id,
                DbType.Guid);

            LinkAssetContent linkAssetContent = new LinkAssetContent
            {
                Label = label,
                Url = url,
                IconUrl = iconUrl
            };

            string contentJson = JsonSerializer.Serialize(linkAssetContent);

            var contentJsonPar = CreateParameter(
                "@content_json",
                contentJson,
                DbType.String);

            var modifiedOnPar = CreateParameter(
                "@modified_on",
                DateTime.Now,
                DbType.DateTime2);

            var modifiedByPar = CreateParameter(
                "@modified_by",
                user.Id,
                DbType.Guid);

            var xSearchPar = CreateParameter(
                "@x_search",
                $"{label} {url}",
                DbType.String);


            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                SQL,
                idPar,
                contentJsonPar,
                modifiedByPar,
                modifiedOnPar,
                xSearchPar);

            if (dbResult != 1)
            {
                throw new Exception("Failed to update row in database for asset object");
            }

            scope.Complete();

            var resultAsset = GetAsset(id);
            AssetUpdated?.Invoke(this, resultAsset);
            return resultAsset;
        }
    }

    public void DeleteAsset(
        Guid assetId)
    {
        var existingAsset = GetAsset(assetId);

        new AssetValidator(this, _tfService)
            .ValidateDelete(existingAsset)
            .ToValidationException()
            .ThrowIfContainsErrors();

        using (var scope = _dbService.CreateTransactionScope())
        {
            var SQL = "DELETE FROM assets_asset WHERE id = @id";

            var idPar = CreateParameter("id", assetId, DbType.Guid);

            var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

            if (dbResult != 1)
            {
                throw new Exception("Failed to update row in database for asset object");
            }

            //delete blob if file asset

            if (existingAsset.Type == AssetType.File)
            {
                FileAssetContent content = (FileAssetContent)existingAsset.Content;
                _tfService.DeleteBlob(content.BlobId);
            }

            //update shared column count if needed
            ModifyAssetSharedColumnCount(existingAsset, isIncrement: false);

            //delete all identity connection to this asset
            _tfService.DeleteDataIdentityConnection(
                TfConstants.TF_ROW_ID_DATA_IDENTITY,
                existingAsset.IdentityRowId);

            scope.Complete();

            AssetDeleted?.Invoke(this, existingAsset);
        }
    }

    private List<Asset> ToAssetList(DataTable dt)
    {
        if (dt == null)
        {
            throw new Exception("DataTable is null");
        }

        var folders = GetFolders();
        List<Asset> assetList = new List<Asset>();

        foreach (DataRow dr in dt.Rows)
        {
            var createdBy = _tfService.GetUser(dr.Field<Guid>("created_by"));

            var modifiedBy = _tfService.GetUser(dr.Field<Guid>("modified_by"));

            var type = (AssetType)dr.Field<short>("type");

            string contentJson = dr.Field<string>("content_json");

            AssetContentBase content = null;

            if (type == AssetType.File)
            {
                content = JsonSerializer.Deserialize<FileAssetContent>(contentJson);
            }
            else if (type == AssetType.Link)
            {
                content = JsonSerializer.Deserialize<LinkAssetContent>(contentJson);
            }
            else
            {
                throw new Exception("Not supported asset type.");
            }

            Asset asset = new Asset
            {
                Id = dr.Field<Guid>("id"),
                FolderId = dr.Field<Guid>("folder_id"),
                Type = type,
                Content = content,
                CreatedBy = createdBy,
                CreatedOn = dr.Field<DateTime>("created_on"),
                ModifiedBy = modifiedBy,
                ModifiedOn = dr.Field<DateTime>("modified_on"),
                ConnectedDataIdentityValues = new Dictionary<string, string>()
            };


            AssetsFolder folder = folders.SingleOrDefault(x => x.Id == asset.FolderId);
            string folderDataIdentity = folder.DataIdentity;

            if (string.IsNullOrWhiteSpace(folderDataIdentity))
                folderDataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;

            var dataIdentityConnectionsJson = dr.Field<string>("identity_connection_json");
            if (!String.IsNullOrWhiteSpace(dataIdentityConnectionsJson) &&
                dataIdentityConnectionsJson.StartsWith("[") &&
                dataIdentityConnectionsJson != "[null]")
            {
                var dataIdentityConnections =
                    JsonSerializer.Deserialize<List<TfDataIdentityConnection>>(dataIdentityConnectionsJson);

                foreach (var dic in dataIdentityConnections)
                {
                    if (dic.Value1 == asset.IdentityRowId)
                    {
                        if (!asset.ConnectedDataIdentityValues.Keys.Contains(dic.Value2))
                            asset.ConnectedDataIdentityValues[dic.Value2] = folderDataIdentity;
                    }
                    if (dic.Value2 == asset.IdentityRowId)
                    {
                        if (!asset.ConnectedDataIdentityValues.Keys.Contains(dic.Value1))
                            asset.ConnectedDataIdentityValues[dic.Value1] = folderDataIdentity;
                    }
                }
            }

            assetList.Add(asset);
        }

        return assetList;
    }

    #region <--- validation --->

    internal class AssetValidator
        : AbstractValidator<Asset>
    {
        private readonly IAssetsService _assetService;
        private readonly ITfService _tfService;

        public AssetValidator(
            IAssetsService service,
            ITfService tfService)
        {
            _assetService = service;
            _tfService = tfService;
        }

        public ValidationResult ValidateCreateFileAsset(
            CreateFileAssetWithDataIdentityModel asset,
            Guid id)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The asset object is null.") });
            }

            if (string.IsNullOrWhiteSpace(asset.Label))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Label is not specified.") });
            }

            if (string.IsNullOrWhiteSpace(asset.LocalPath))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "LocalPath is not specified.") });
            }

            if (!File.Exists(asset.LocalPath))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "File is not found for specified local path.") });
            }

            var folder = _assetService.GetFolder(asset.FolderId);
            if (folder == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.FolderId),
                    "The folder is not found.") });
            }


            return new ValidationResult();
        }

        public ValidationResult ValidateCreateFileAsset(
            CreateFileAssetWithRowIdModel asset,
            Guid id)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The asset object is null.") });
            }

            if (string.IsNullOrWhiteSpace(asset.Label))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Label is not specified.") });
            }

            if (string.IsNullOrWhiteSpace(asset.LocalPath))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "LocalPath is not specified.") });
            }

            if (!File.Exists(asset.LocalPath))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "File is not found for specified local path.") });
            }

            var dataProvider = _tfService.GetDataProvider(asset.DataProviderId);
            if (dataProvider == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.DataProviderId),
                    "The data provider is not found.") });
            }

            var folder = _assetService.GetFolder(asset.FolderId);
            if (folder == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.FolderId),
                    "The folder is not found.") });
            }

            if (!dataProvider.Identities.Any(x => x.DataIdentity == folder.DataIdentity))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.FolderId),
                    "The folder data identity is not associated with the data provider.") });
            }

            return new ValidationResult();
        }

        public ValidationResult ValidateCreateLinkAsset(
            CreateLinkAssetWithDataIdentityModel asset,
            Guid id)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The asset object is null.") });
            }

            if (string.IsNullOrWhiteSpace(asset.Label))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Label is not specified.") });
            }

            if (string.IsNullOrWhiteSpace(asset.Url))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithDataIdentityModel.Url),
                    "The url is empty.") });
            }

            try
            {
                Uri uri = new Uri(asset.Url);
            }
            catch
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithDataIdentityModel.Url),
                    "The url is not valid.") });
            }

            var folder = _assetService.GetFolder(asset.FolderId);
            if (folder == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.FolderId),
                    "The folder is not found.") });
            }


            return new ValidationResult();
        }

        public ValidationResult ValidateCreateLinkAsset(
           CreateLinkAssetWithRowIdModel asset,
           Guid id)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The asset object is null.") });
            }

            if (string.IsNullOrWhiteSpace(asset.Label))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Label is not specified.") });
            }

            if (string.IsNullOrWhiteSpace(asset.Url))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithDataIdentityModel.Url),
                    "The url is empty.") });
            }

            try
            {
                Uri uri = new Uri(asset.Url);
            }
            catch
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithDataIdentityModel.Url),
                    "The url is not valid.") });
            }

            var dataProvider = _tfService.GetDataProvider(asset.DataProviderId);
            if (dataProvider == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.DataProviderId),
                    "The data provider is not found.") });
            }

            var folder = _assetService.GetFolder(asset.FolderId);
            if (folder == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.FolderId),
                    "The folder is not found.") });
            }

            if (!dataProvider.Identities.Any(x => x.DataIdentity == folder.DataIdentity))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithRowIdModel.FolderId),
                    "The folder data identity is not associated with the data provider.") });
            }

            return new ValidationResult();
        }

        public ValidationResult ValidateUpdateFileAsset(
            Asset asset,
            string label,
            string localPath,
            TfUser user)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The thread object is null.") });
            }

            if (user == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "User is not found.") });
            }

            if (asset.Type != AssetType.File)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Asset it not from type File") });
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Label is not specified.") });
            }

            if (!string.IsNullOrEmpty(localPath) &&
                !File.Exists(localPath))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(Asset.Content),
                    "Local path is not correct") });
            }

            return new ValidationResult();

        }

        public ValidationResult ValidateUpdateLinkAsset(
            Asset asset,
            string label,
            string url,
            TfUser user)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The thread object is null.") });
            }

            if (user == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "User is not found.") });
            }

            if (asset.Type != AssetType.Link)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Asset it not from type Link") });
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "Label is not specified.") });
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithDataIdentityModel.Url),
                    "The url is empty.") });
            }

            try
            {
                Uri uri = new Uri(url);
            }
            catch
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateLinkAssetWithDataIdentityModel.Url),
                    "The url is not valid.") });
            }

            return new ValidationResult();

        }

        public ValidationResult ValidateDelete(
            Asset asset)
        {
            if (asset == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The asset object is null.") });
            }

            return this.Validate(asset, options =>
            {
                options.IncludeRuleSets("delete");
            });
        }
    }

    #endregion

    public void UpdateFoldersSharedColumnCount()
    {
        var folders = GetFolders();
        foreach (var folder in folders)
        {
            UpdateFolderSharedColumnCount(folder);
        }
    }

    public void UpdateFolderSharedColumnCount(
        AssetsFolder folder)
    {
        if (folder == null)
            return;

        if (string.IsNullOrWhiteSpace(folder.CountSharedColumnName))
            return;

        var sharedColumn = _tfService.GetSharedColumn(folder.CountSharedColumnName);

        if (sharedColumn is null)
            return;

        //only number type columns are supported
        if (!(  sharedColumn.DbType == TfDatabaseColumnType.Number ||
                sharedColumn.DbType == TfDatabaseColumnType.ShortInteger ||
                sharedColumn.DbType == TfDatabaseColumnType.Integer ||
                sharedColumn.DbType == TfDatabaseColumnType.LongInteger
            ))
        {
            return;
        }

        if(sharedColumn.DataIdentity != folder.DataIdentity)
            return;

        string folderDataIdentityName = folder.DataIdentity;

        if (string.IsNullOrEmpty(folderDataIdentityName))
            folderDataIdentityName = TfConstants.TF_ROW_ID_DATA_IDENTITY;

        var folderIdentity = _tfService.GetDataIdentity(folderDataIdentityName);

        if (folderIdentity is null)
            return;

        var assets = GetAssets(folder.Id);

        var assetIdentityValues = assets.Select(x => x.IdentityRowId).ToHashSet();

        var folderIdentityConnections = _tfService.GetDataIdentityConnections(
            dataIndentity1: TfConstants.TF_ROW_ID_DATA_IDENTITY,
            dataIdentity2: folderDataIdentityName);

        var connectedObjectIdentityValuesDict = new Dictionary<string, object>();

        foreach (var idCon in folderIdentityConnections)
        {
            if (idCon.DataIdentity1 == TfConstants.TF_ROW_ID_DATA_IDENTITY &&
                assetIdentityValues.Contains(idCon.Value1))
            {
                if (!connectedObjectIdentityValuesDict.ContainsKey(idCon.Value2))
                    connectedObjectIdentityValuesDict[idCon.Value2] = 0;

                connectedObjectIdentityValuesDict[idCon.Value2] = ((int)connectedObjectIdentityValuesDict[idCon.Value2]) + 1;

            }

            if (idCon.DataIdentity2 == TfConstants.TF_ROW_ID_DATA_IDENTITY &&
               assetIdentityValues.Contains(idCon.Value2))
            {
                if (!connectedObjectIdentityValuesDict.ContainsKey(idCon.Value1))
                    connectedObjectIdentityValuesDict[idCon.Value1] = 0;

                connectedObjectIdentityValuesDict[idCon.Value1] = ((int)connectedObjectIdentityValuesDict[idCon.Value1]) + 1;
            }
        }

        _tfService.SaveSharedColumnValues(sharedColumn.Id, connectedObjectIdentityValuesDict, batchSize: 1000);

    }

    public void ModifyAssetSharedColumnCount(
        Asset asset,
        bool isIncrement )
    {
        if (asset == null)
            return;

        if(asset.ConnectedDataIdentityValues == null ||asset.ConnectedDataIdentityValues.Count == 0)
            return;

        var folder = GetFolder(asset.FolderId);

        if (folder == null)
            return;

        if (string.IsNullOrWhiteSpace(folder.CountSharedColumnName))
            return;

        var sharedColumn = _tfService.GetSharedColumn(folder.CountSharedColumnName);

        if (sharedColumn is null)
            return;

        if (sharedColumn.DataIdentity != folder.DataIdentity)
            return;

        //only number type columns are supported
        if (!(  sharedColumn.DbType == TfDatabaseColumnType.Number ||
                sharedColumn.DbType == TfDatabaseColumnType.ShortInteger ||
                sharedColumn.DbType == TfDatabaseColumnType.Integer ||
                sharedColumn.DbType == TfDatabaseColumnType.LongInteger
            ))
        {
            return;
        }

        var identityValues = asset.ConnectedDataIdentityValues.Keys.ToList();  

        ModifySharedColumnValues(sharedColumn, identityValues, isIncrement?1:-1, 1000);
    }

    private void ModifySharedColumnValues(
        TfSharedColumn sharedColumn,
        List<string> identityValues,
        int valueChange,
        int batchSize )
    {
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero");

        if (identityValues is null)
            throw new ArgumentNullException(nameof(identityValues));

        if (sharedColumn is null)
            throw new Exception("Shared column not found");

        string tableName;
        switch (sharedColumn.DbType)
        {
            case TfDatabaseColumnType.ShortInteger:
                tableName = "tf_shared_column_short_integer_value";
                break;
            case TfDatabaseColumnType.Integer:
                tableName = "tf_shared_column_integer_value";
                break;
            case TfDatabaseColumnType.LongInteger:
                tableName = "tf_shared_column_long_integer_value";
                break;
            case TfDatabaseColumnType.Number:
                tableName = "tf_shared_column_number_value";
                break;
            default:
                throw new Exception("Shared column db type is not supported column type for modification.");
        }

        using (var scope = _dbService.CreateTransactionScope())
        {
            foreach (IEnumerable<string> keysBatch in identityValues.Batch(batchSize))
            {
                int paramCounter = 1;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                parameters.Add(new NpgsqlParameter($"@shared_column_id", sharedColumn.Id));

                StringBuilder sqlSb = new StringBuilder();

                foreach (var identityValue in keysBatch)
                {
                    sqlSb.AppendLine( $@"INSERT INTO {tableName} (shared_column_id,data_identity_value, value)
                            VALUES (@shared_column_id,@data_identity_value_{paramCounter},{(valueChange>=0?valueChange:0)} )
                            ON CONFLICT (data_identity_value,shared_column_id)
                            DO UPDATE SET  value = GREATEST( 0, {tableName}.value + ( {valueChange} ) );");

                    parameters.Add(new NpgsqlParameter($"@data_identity_value_{paramCounter}", identityValue));

                    paramCounter++;
                }

                _dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);
            }

            scope.Complete();
        }
    }
}
