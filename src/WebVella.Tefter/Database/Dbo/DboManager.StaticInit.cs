namespace WebVella.Tefter.Database.Dbo;

internal partial class DboManager
{
	public static Dictionary<Type, DboModelMeta> MetaDict { get; internal set; } = new Dictionary<Type, DboModelMeta>();

	static DboManager()
	{
		ScanAndRegesterTypes();
	}

	private static void ScanAndRegesterTypes()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));
		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
				ProcessType(type);
		}
	}

	private static void ProcessType(Type type)
	{
		var cacheAttributes = type.GetCustomAttributes(typeof(DboCacheModelAttribute), false);
		DboCacheModelAttribute cacheAttribute = null;
		if (cacheAttributes.Length == 1 && type.IsClass)
			cacheAttribute = (DboCacheModelAttribute)cacheAttributes[0];

		var modelAttributes = type.GetCustomAttributes(typeof(DboModelAttribute), false);
		DboModelAttribute modelAttribute = null;
		if (modelAttributes.Length == 1 && type.IsClass)
			modelAttribute = (DboModelAttribute)modelAttributes[0];

		if (modelAttribute != null)
		{
			DboModelMeta meta = new DboModelMeta();
			meta = new DboModelMeta();
			meta.IsSqlModel = false;
			meta.TableName = modelAttribute.TableName;
			meta.ModelType = type;
			meta.UseCache = cacheAttribute is not null;
			if (meta.UseCache)
			{
				meta.CacheOptions = cacheAttribute.Options;
				InitCache(type, cacheAttribute.Options);
			}

			var properties = type.GetProperties();
			foreach (var property in properties)
			{
				var propAttributes = property.GetCustomAttributes(typeof(DboModelPropertyAttribute), true);
				if (propAttributes.Length == 1)
				{
					var propAttribute = (DboModelPropertyAttribute)propAttributes[0];

					DboModelPropertyMeta propMeta = new DboModelPropertyMeta();
					propMeta.PropertyInfo = property;
					propMeta.ColumnName = propAttribute.ColumnName;

					var propTypeConverterAttributes = property.GetCustomAttributes(typeof(DboTypeConverterAttribute), true);
					if (propTypeConverterAttributes.Length == 1)
						propMeta.Converter = ((DboTypeConverterAttribute)propTypeConverterAttributes[0]).Converter;

					var autoIncrAttributes = property.GetCustomAttributes(typeof(DboAutoIncrementModelAttribute), true);
					propMeta.IsAutoIncrement = autoIncrAttributes.Length == 1;

					meta.Properties.Add(propMeta);
				}
			}

			List<string> columnNames = meta.Properties.Select(x => x.ColumnName).ToList();
			List<string> upsertColumnNames = meta.Properties.Where(x => !x.IsAutoIncrement).Select(x => x.ColumnName).ToList();

			meta.GetSql = $"SELECT {string.Join(", ", columnNames)} FROM \"{meta.TableName}\" ";
			meta.GetRecordSql = $"SELECT {string.Join(",", columnNames)} FROM \"{meta.TableName}\" WHERE $$$WHERE$$$ ";
			meta.InsertRecordSql = $"INSERT INTO \"{meta.TableName}\"({string.Join(", ", upsertColumnNames)}) VALUES ({string.Join(", ", upsertColumnNames.Select(x => $"@{x}"))}) ";
			meta.UpdateRecordSql = $"UPDATE \"{meta.TableName}\" SET {string.Join(", ", upsertColumnNames.Where(x => x != "id").Select(x => $"{x}=@{x}"))} WHERE $$$WHERE$$$ ";
			meta.DeleteRecordSql = $"DELETE FROM \"{meta.TableName}\" WHERE $$$WHERE$$$ ";
			meta.DeleteAllRecordSql = $"DELETE FROM \"{meta.TableName}\"";
			meta.AnyExistsSql = $"SELECT EXISTS ( SELECT 1 FROM \"{meta.TableName}\" WHERE $$$WHERE$$$ LIMIT 1 ) AS EXISTS";
			MetaDict.Add(type, meta);
		}

		var sqlModelAttributes = type.GetCustomAttributes(typeof(DboSqlModelAttribute), false);
		DboSqlModelAttribute sqlModelAttribute = null;
		if (sqlModelAttributes.Length == 1 && type.IsClass)
			sqlModelAttribute = (DboSqlModelAttribute)sqlModelAttributes[0];

		if (sqlModelAttribute != null)
		{
			DboModelMeta meta = new DboModelMeta();
			meta = new DboModelMeta();
			meta.IsSqlModel = true;
			meta.TableName = sqlModelAttribute.QueryName ?? type.FullName;
			meta.ModelType = type;
			meta.UseCache = false;

			var properties = type.GetProperties();
			foreach (var property in properties)
			{
				var propAttributes = property.GetCustomAttributes(typeof(DboModelPropertyAttribute), true);
				if (propAttributes.Length == 1)
				{
					var propAttribute = (DboModelPropertyAttribute)propAttributes[0];

					DboModelPropertyMeta propMeta = new DboModelPropertyMeta();
					propMeta.PropertyInfo = property;
					propMeta.ColumnName = propAttribute.ColumnName;

					var propTypeConverterAttributes = property.GetCustomAttributes(typeof(DboTypeConverterAttribute), true);
					if (propTypeConverterAttributes.Length == 1)
						propMeta.Converter = ((DboTypeConverterAttribute)propTypeConverterAttributes[0]).Converter;


					meta.Properties.Add(propMeta);
				}
			}

			List<string> columnNames = meta.Properties.Select(x => x.ColumnName).ToList();

			meta.GetSql = String.Empty;
			meta.GetRecordSql = String.Empty;
			meta.InsertRecordSql = String.Empty;
			meta.UpdateRecordSql = String.Empty;
			meta.DeleteRecordSql = String.Empty;
			MetaDict.Add(type, meta);
		}
	}
}
