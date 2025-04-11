namespace WebVella.Tefter.Assets.Services;

public partial interface IAssetsService
{
	public Asset GetAsset(
		Guid id);

	public List<Asset> GetAssets(
		Guid? folderId = null,
		Guid? skId = null);

	public List<Asset> GetAssets(
		Guid? folderId = null,
		string skTextId = null);

	public Asset CreateFileAsset(
		CreateFileAssetModel fileAsset);

	public Asset CreateLinkAsset(
		CreateLinkAssetModel asset);

	public Asset CreateFileAsset(
		CreateFileAssetWithSharedKeyModel asset);

	public Asset CreateLinkAsset(
		CreateLinkAssetWithSharedKeyModel asset);

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
}

internal partial class AssetsService : IAssetsService
{
	public Asset GetAsset(
		Guid id)
	{
		const string SQL =
@"WITH sk_info AS (
	SELECT trs.asset_id, JSON_AGG( idd.* ) AS json_result
	FROM assets_related_sk trs
		LEFT OUTER JOIN tf_id_dict idd ON idd.id = trs.id
	GROUP BY trs.asset_id
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
	sk_info.json_result AS related_shared_key_json
FROM assets_asset aa
	LEFT OUTER JOIN sk_info  ON aa.id = sk_info.asset_id
WHERE aa.id = @id
";

		var assetIdPar = CreateParameter(
			"id",
			id,
			DbType.Guid);

		var dt = _dbService.ExecuteSqlQueryCommand(SQL, assetIdPar);

		List<Asset> assets = ToAssetList(dt);

		if (assets.Count == 0)
		{
			return null;
		}

		return assets[0];
	}

	public List<Asset> GetAssets(
		Guid? folderId = null,
		Guid? skId = null)
	{
		const string SQL = @"
WITH sk_info AS (
	SELECT trs.asset_id, JSON_AGG( idd.* ) AS json_result
	FROM assets_related_sk trs
		LEFT OUTER JOIN tf_id_dict idd ON idd.id = trs.id
	WHERE ( @sk_id IS NULL OR trs.id = @sk_id )
	GROUP BY trs.asset_id
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
	sk.json_result AS related_shared_key_json
FROM assets_asset aa
	LEFT OUTER JOIN sk_info sk ON aa.id = sk.asset_id
WHERE ( @folder_id IS NULL OR aa.folder_id = @folder_id ) AND ( @sk_id IS NULL OR sk.asset_id is not null )
ORDER BY aa.created_on DESC;";

		var folderIdPar = CreateParameter(
			"folder_id",
			folderId,
			DbType.Guid);

		var skIdPar = CreateParameter(
			"sk_id",
			skId,
			DbType.Guid);

		var dt = _dbService.ExecuteSqlQueryCommand(SQL, folderIdPar, skIdPar);

		return ToAssetList(dt);
	}

	public List<Asset> GetAssets(
		Guid? folderId = null,
		string skTextId = null)
	{
		Guid skId = _tfService.GetId(skTextId);
		return GetAssets(folderId, skId);
	}

	public Asset CreateFileAsset(
		CreateFileAssetModel fileAsset)
	{
		if (fileAsset == null)
			throw new NullReferenceException("Asset object is null");

		Guid id = Guid.NewGuid();

		new AssetValidator(this)
			.ValidateCreateFileAsset(fileAsset, id)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope())
		{
			string filename = fileAsset.FileName;

			Guid blobId = _tfService.CreateBlob(fileAsset.LocalPath);

			DateTime now = DateTime.Now;

			var SQL = @"INSERT INTO assets_asset
						(id, folder_id, type, content_json, created_by,
						created_on, modified_by, modified_on, x_search)
					VALUES(@id, @folder_id, @type, @content_json, @created_by,
						@created_on, @modified_by, @modified_on, @x_search); ";

			var idPar = CreateParameter("@id", id, DbType.Guid);

			var folderIdPar = CreateParameter("@folder_id", fileAsset.FolderId, DbType.Guid);

			var typePar = CreateParameter("@type", (short)AssetType.File, DbType.Int16);

			FileAssetContent content = new FileAssetContent
			{
				BlobId = blobId,
				Filename = filename,
				Label = fileAsset.Label,
				DownloadUrl = $"/fs/assets/{id}/{filename}"
			};

			var contentJson = JsonSerializer.Serialize(content);

			var contentJsonPar = CreateParameter("@content_json", contentJson, DbType.String);

			var createdByPar = CreateParameter("@created_by", fileAsset.CreatedBy, DbType.Guid);

			var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

			var modifiedByPar = CreateParameter("@modified_by", fileAsset.CreatedBy, DbType.Guid);

			var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);


			string xSearch = string.Empty;
			xSearch = $"{fileAsset.Label} {filename}";

			var xSearchPar = CreateParameter("@x_search", xSearch, DbType.String);

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


			if (fileAsset.RowIds != null && fileAsset.RowIds.Count > 0)
			{
				var folder = GetFolder(fileAsset.FolderId);

				var provider = _tfService.GetDataProvider(fileAsset.DataProviderId);

				if (provider is null)
				{
					throw new Exception($"Failed to find data provider with id='{fileAsset.DataProviderId}'");
				}

				var dataTable = _tfService.QueryDataProvider(provider, fileAsset.RowIds);

				List<Guid> relatedSK = new List<Guid>();

				foreach (TfDataRow row in dataTable.Rows)
				{
					var skIdValue = row.GetSharedKeyValue(folder.SharedKey);

					if (skIdValue is not null && !relatedSK.Contains(skIdValue.Value))
					{
						relatedSK.Add(skIdValue.Value);
					}
				}

				foreach (var skId in relatedSK)
				{
					var skDbResult = _dbService.ExecuteSqlNonQueryCommand(
						"INSERT INTO assets_related_sk (id,asset_id) VALUES (@id, @asset_id)",
							new NpgsqlParameter("@id", skId),
							new NpgsqlParameter("@asset_id", id));

					if (skDbResult != 1)
					{
						throw new Exception("Failed to insert new row in database for related shared key object");
					}
				}
			}

			scope.Complete();

			var resultAsset = GetAsset(id);

			return resultAsset;
		}
	}

	public Asset CreateLinkAsset(
		CreateLinkAssetModel asset)
	{
		if (asset == null)
			throw new NullReferenceException("Asset object is null");

		Guid id = Guid.NewGuid();

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
			Label = asset.Label,
			Url = asset.Url,
			IconUrl = asset.IconUrl,
		};

		var contentJson = JsonSerializer.Serialize(content);

		var contentJsonPar = CreateParameter("@content_json", contentJson, DbType.String);

		var createdByPar = CreateParameter("@created_by", asset.CreatedBy, DbType.Guid);

		var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

		var modifiedByPar = CreateParameter("@modified_by", asset.CreatedBy, DbType.Guid);

		var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

		string xSearch = $"{asset.Label} {asset.Url}";

		var xSearchPar = CreateParameter("@x_search", xSearch, DbType.String);

		using (var scope = _dbService.CreateTransactionScope())
		{
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
				var folder = GetFolder(asset.FolderId);

				var provider = _tfService.GetDataProvider(asset.DataProviderId);

				if (provider is null)
				{
					throw new Exception($"Failed to find data provider with id='{asset.DataProviderId}'");
				}

				var dataTable = _tfService.QueryDataProvider(provider, asset.RowIds);

				List<Guid> relatedSK = new List<Guid>();

				foreach (TfDataRow row in dataTable.Rows)
				{
					var skIdValue = row.GetSharedKeyValue(folder.SharedKey);

					if (skIdValue is not null && !relatedSK.Contains(skIdValue.Value))
					{
						relatedSK.Add(skIdValue.Value);
					}
				}

				foreach (var skId in relatedSK)
				{
					var skDbResult = _dbService.ExecuteSqlNonQueryCommand(
						"INSERT INTO assets_related_sk (id,asset_id) VALUES (@id, @asset_id)",
							new NpgsqlParameter("@id", skId),
							new NpgsqlParameter("@asset_id", id));

					if (skDbResult != 1)
					{
						throw new Exception("Failed to insert new row in database for related shared key object");
					}
				}
			}

			scope.Complete();

			var resultAsset = GetAsset(id);

			return resultAsset;
		}
	}

	public Asset CreateFileAsset(
		CreateFileAssetWithSharedKeyModel asset)
	{

		if (asset == null)
			throw new NullReferenceException("Asset object is null");

		Guid id = Guid.NewGuid();

		new AssetValidator(this)
			.ValidateCreateFileAsset(asset, id)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope())
		{
			string filename = asset.FileName;

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
			{
				throw new Exception("Failed to insert new row in database for thread object");
			}

			if (asset.SKValueIds != null && asset.SKValueIds.Count > 0)
			{
				foreach (var skValueId in asset.SKValueIds)
				{
					Guid skId = _tfService.GetId(skValueId);
					var skDbResult = _dbService.ExecuteSqlNonQueryCommand(
						"INSERT INTO assets_related_sk (id, asset_id) VALUES (@id, @asset_id)",
							new NpgsqlParameter("@id", skId),
							new NpgsqlParameter("@asset_id", id));

					if (skDbResult != 1)
					{
						throw new Exception("Failed to insert new row in database for related shared key object");
					}
				}
			}

			scope.Complete();

			var resultAsset = GetAsset(id);

			return resultAsset;
		}
	}

	public Asset CreateLinkAsset(
		CreateLinkAssetWithSharedKeyModel asset)
	{

		if (asset == null)
		{
			throw new NullReferenceException("Asset object is null");
		}

		Guid id = Guid.NewGuid();

		new AssetValidator(this)
			.ValidateCreateLinkAsset(asset, id)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope())
		{
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

			if (asset.SKValueIds != null && asset.SKValueIds.Count > 0)
			{
				foreach (var skId in asset.SKValueIds)
				{
					var skDbResult = _dbService.ExecuteSqlNonQueryCommand(
						"INSERT INTO assets_related_sk (id, asset_id) VALUES (@id, @asset_id)",
							new NpgsqlParameter("@id", skId),
							new NpgsqlParameter("@asset_id", id));

					if (skDbResult != 1)
					{
						throw new Exception("Failed to insert new row in database for related shared key object");
					}
				}
			}

			scope.Complete();

			var resultAsset = GetAsset(id);

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

		new AssetValidator(this)
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
			{
				throw new Exception("Failed to update row in database for asset object");
			}

			if (!string.IsNullOrWhiteSpace(localPath))
			{
				_tfService.UpdateBlob(fileAssetContent.BlobId, localPath);
			}

			scope.Complete();

			var resultAsset = GetAsset(id);

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

		new AssetValidator(this)
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

			return resultAsset;
		}
	}

	public void DeleteAsset(
		Guid assetId)
	{
		var existingAsset = GetAsset(assetId);

		new AssetValidator(this)
			.ValidateDelete(existingAsset)
			.ToValidationException()
			.ThrowIfContainsErrors();

		using (var scope = _dbService.CreateTransactionScope())
		{
			//delete link to related shared keys

			var SQL = "DELETE FROM assets_related_sk WHERE asset_id = @asset_id";

			var assetIdPar = CreateParameter("asset_id", assetId, DbType.Guid);

			_dbService.ExecuteSqlNonQueryCommand(SQL, assetIdPar);

			//delete asset record

			SQL = "DELETE FROM assets_asset WHERE id = @id";

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

			scope.Complete();
		}
	}

	private List<Asset> ToAssetList(DataTable dt)
	{
		if (dt == null)
		{
			throw new Exception("DataTable is null");
		}

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
				RelatedSK = new Dictionary<Guid, string>()
			};


			var relatedSharedKeysJson = dr.Field<string>("related_shared_key_json");

			if (!String.IsNullOrWhiteSpace(relatedSharedKeysJson) &&
				relatedSharedKeysJson.StartsWith("[") &&
				relatedSharedKeysJson != "[null]")
			{
				var items = JsonSerializer.Deserialize<List<IdDictModel>>(relatedSharedKeysJson);

				foreach (var item in items)
				{
					asset.RelatedSK[item.Id] = item.TextId;
				}
			}

			assetList.Add(asset);
		}

		return assetList;
	}

	private class IdDictModel
	{
		[JsonPropertyName("id")]
		public Guid Id { get; set; }

		[JsonPropertyName("text_id")]
		public string TextId { get; set; }
	}

	#region <--- validation --->

	internal class AssetValidator
		: AbstractValidator<Asset>
	{
		private readonly IAssetsService _service;

		public AssetValidator(IAssetsService service)
		{
			_service = service;
		}

		public ValidationResult ValidateCreateFileAsset(
			CreateFileAssetModel asset,
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

			return new ValidationResult();
		}

		public ValidationResult ValidateCreateLinkAsset(
			CreateLinkAssetModel asset,
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
					nameof(CreateLinkAssetModel.Url),
					"The url is empty.") });
			}

			try
			{
				Uri uri = new Uri(asset.Url);
			}
			catch
			{
				return new ValidationResult(new[] { new ValidationFailure(
					nameof(CreateLinkAssetModel.Url),
					"The url is not valid.") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateCreateFileAsset(
			CreateFileAssetWithSharedKeyModel asset,
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

			return new ValidationResult();
		}

		public ValidationResult ValidateCreateLinkAsset(
			CreateLinkAssetWithSharedKeyModel asset,
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
					nameof(CreateLinkAssetModel.Url),
					"The url is empty.") });
			}

			try
			{
				Uri uri = new Uri(asset.Url);
			}
			catch
			{
				return new ValidationResult(new[] { new ValidationFailure(
					nameof(CreateLinkAssetModel.Url),
					"The url is not valid.") });
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
					nameof(CreateLinkAssetModel.Url),
					"The url is empty.") });
			}

			try
			{
				Uri uri = new Uri(url);
			}
			catch
			{
				return new ValidationResult(new[] { new ValidationFailure(
					nameof(CreateLinkAssetModel.Url),
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
}
