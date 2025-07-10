namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public TfTemplate GetTemplate(
		Guid id);

	public List<TfTemplate> GetTemplates(string? search = null);

	public TfTemplate CreateTemplate(
		TfManageTemplateModel template);

	public TfTemplate UpdateTemplate(
		TfManageTemplateModel template);

	public void DeleteTemplate(
		Guid templateId);

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds);

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds,
		ITfTemplatePreviewResult preview);

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview);

	public void ValidateTemplatePreview(
		Guid templateId,
		ITfTemplatePreviewResult preview);

	List<TfSpaceDataAsOption> GetSpaceDataOptionsForTemplate();
	TfTemplate UpdateTemplateSettings(Guid templateId,string settingsJson);
}

public partial class TfService : ITfService
{
	public TfTemplate GetTemplate(Guid id)
	{
		try
		{
			const string SQL = @"SELECT * FROM tf_template WHERE id = @id";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL, CreateParameter("@id", id, DbType.Guid));

			var list = ToTemplateList(dt);

			if (list.Count == 0)
				return null;

			return list[0];
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfTemplate> GetTemplates(string? search = null)
	{
		try
		{
			const string SQL = @"SELECT * FROM tf_template";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL);

			var allTemplates = ToTemplateList(dt);

			if (String.IsNullOrWhiteSpace(search))
				return allTemplates;

			search = search.Trim().ToLowerInvariant();
			return allTemplates.Where(x => x.Name.ToLowerInvariant().Contains(search)).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfTemplate CreateTemplate(
		TfManageTemplateModel template)
	{
		try
		{
			if (template == null)
				throw new TfException("Template object is null");

			if (template.ContentProcessorType == null)
				throw new TfException("Content processor is not selected");

			var contentProcessor = GetTemplateProcessor(template.ContentProcessorType);

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				if (template is not null && template.Id == Guid.Empty)
				{
					template.Id = Guid.NewGuid();
				}

				var errors = contentProcessor.OnCreate(template, _serviceProvider);

				if (errors is not null && errors.Count > 0)
				{
					var exception = new TfException();
					foreach (var error in errors)
					{
						exception.UpsertDataList(error.PropertyName, error.Message);
					}
					throw exception;
				}

				DateTime now = DateTime.Now;

				new TemplateValidator(this)
					.ValidateCreate(template)
					.ToValidationException()
					.ThrowIfContainsErrors();

				string spaceDataListJson = JsonSerializer.Serialize(template.SpaceDataList ?? new List<Guid>());

				var idPar = CreateParameter("@id", template.Id, DbType.Guid);

				var namePar = CreateParameter("@name", template.Name, DbType.String);

				var descriptionPar = CreateParameter("@description", template.Description, DbType.String);

				var iconPar = CreateParameter("@icon", template.FluentIconName, DbType.String);

				var isEnabledPar = CreateParameter("@is_enabled", template.IsEnabled, DbType.Boolean);

				var isSelectablePar = CreateParameter("@is_selectable", template.IsSelectable, DbType.Boolean);

				var resultTypePar = CreateParameter("@result_type", (short)contentProcessor.ResultType, DbType.Int16);

				var spaceDataJsonPar = CreateParameter("@space_data_json", spaceDataListJson, DbType.String);

				var settingsJsonPar = CreateParameter("@settings_json", template.SettingsJson, DbType.String);

				var cptPar = CreateParameter("@content_processor_type", template.ContentProcessorType.AssemblyQualifiedName, DbType.String);

				var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

				var createdByPar = CreateParameter("@created_by", template.UserId, DbType.Guid);

				var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

				var modifiedByPar = CreateParameter("@modified_by", template.UserId, DbType.Guid);

				const string SQL = @"
				INSERT INTO tf_template(
					id, name, icon, description, space_data_json, is_enabled, 
					is_selectable, result_type, settings_json, content_processor_type,
					created_on, modified_on, created_by, modified_by)
				VALUES (
					@id, @name, @icon, @description, @space_data_json, @is_enabled, 
					@is_selectable, @result_type, @settings_json, @content_processor_type,
					@created_on, @modified_on, @created_by, @modified_by )";

				var dbResult = _dbService.ExecuteSqlNonQueryCommand(
					SQL,
					idPar, namePar, descriptionPar, iconPar,
					isEnabledPar, isSelectablePar, resultTypePar,
					spaceDataJsonPar, settingsJsonPar, cptPar,
					createdByPar, createdOnPar, modifiedByPar, modifiedOnPar);

				if (dbResult != 1)
				{
					throw new TfException("Failed to insert new row in database for template object");
				}

				scope.Complete();
			}

			var resultTemplate = GetTemplate(template.Id);

			contentProcessor.OnCreated(resultTemplate, _serviceProvider);

			return resultTemplate;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfTemplate UpdateTemplate(
		TfManageTemplateModel template)
	{
		try
		{
			if (template == null)
				throw new TfException("Template object is null");

			if (template.ContentProcessorType == null)
				throw new TfException("Content processor is not selected");

			var contentProcessor = GetTemplateProcessor(template.ContentProcessorType);

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var errors = contentProcessor.OnUpdate(template, _serviceProvider);
				if (errors is not null && errors.Count > 0)
				{
					var exception = new TfException();
					foreach (var error in errors)
					{
						exception.UpsertDataList(error.PropertyName, error.Message);
					}
					throw exception;
				}

				new TemplateValidator(this)
					.ValidateUpdate(template)
					.ToValidationException()
					.ThrowIfContainsErrors();

				string spaceDataListJson = JsonSerializer.Serialize(template.SpaceDataList ?? new List<Guid>());

				var idPar = CreateParameter("@id", template.Id, DbType.Guid);

				var namePar = CreateParameter("@name", template.Name, DbType.String);

				var descriptionPar = CreateParameter("@description", template.Description, DbType.String);

				var iconPar = CreateParameter("@icon", template.FluentIconName, DbType.String);

				var isEnabledPar = CreateParameter("@is_enabled", template.IsEnabled, DbType.Boolean);

				var isSelectablePar = CreateParameter("@is_selectable", template.IsSelectable, DbType.Boolean);

				var resultTypePar = CreateParameter("@result_type", (short)contentProcessor.ResultType, DbType.Int16);

				var spaceDataJsonPar = CreateParameter("@space_data_json", spaceDataListJson, DbType.String);

				var settingsJsonPar = CreateParameter("@settings_json", template.SettingsJson, DbType.String);

				var cptPar = CreateParameter("@content_processor_type", template.ContentProcessorType.AssemblyQualifiedName, DbType.String);

				var modifiedOnPar = CreateParameter("@modified_on", DateTime.Now, DbType.DateTime2);

				var modifiedByPar = CreateParameter("@modified_by", template.UserId, DbType.Guid);

				const string SQL = @"
				UPDATE tf_template
				SET 
					name=@name,
					icon=@icon,
					description=@description,
					space_data_json=@space_data_json,
					is_enabled=@is_enabled,
					is_selectable=@is_selectable, 
					result_type=@result_type, 
					settings_json=@settings_json, 
					content_processor_type=@content_processor_type, 
					modified_on=@modified_on, 
					modified_by=@modified_by
				WHERE id = @id;";

				var dbResult = _dbService.ExecuteSqlNonQueryCommand(
					SQL,
					idPar, namePar, descriptionPar, iconPar,
					isEnabledPar, isSelectablePar, resultTypePar,
					spaceDataJsonPar, settingsJsonPar, cptPar,
					modifiedByPar, modifiedOnPar);

				if (dbResult != 1)
				{
					throw new Exception("Failed to insert new row in database for template object");
				}

				scope.Complete();
			}

			var resultTemplate = GetTemplate(template.Id);

			contentProcessor.OnUpdated(resultTemplate, _serviceProvider);

			return resultTemplate;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteTemplate(
		Guid templateId)
	{
		try
		{
			var existingTemplate = GetTemplate(templateId);
			if (existingTemplate == null)
				throw new TfException("Template is not found");

			var contentProcessor = GetTemplateProcessor(existingTemplate.ContentProcessorType);

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var errors = contentProcessor.OnDelete(existingTemplate, _serviceProvider);
				if (errors is not null && errors.Count > 0)
				{
					var exception = new TfException();
					foreach (var error in errors)
					{
						exception.UpsertDataList(error.PropertyName, error.Message);
					}
					throw exception;
				}

				new TemplateValidator(this)
					.ValidateDelete(existingTemplate)
					.ToValidationException()
					.ThrowIfContainsErrors();

				const string SQL = "DELETE FROM tf_template WHERE id = @id";

				var idPar = CreateParameter("id", templateId, DbType.Guid);

				var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

				if (dbResult != 1)
				{
					throw new Exception("Failed to delete row in database for template object");
				}

				scope.Complete();
			}

			contentProcessor.OnDeleted(existingTemplate, _serviceProvider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds)
	{
		try
		{
			var template = GetTemplate(templateId);
			var spaceData = GetSpaceData(spaceDataId);
			var processor = GetTemplateProcessor(template.ContentProcessorType);

			var dataTable = QuerySpaceData(spaceData.Id, tfRecordIds);

			return processor.GenerateTemplatePreviewResult(template, dataTable, _serviceProvider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds,
		ITfTemplatePreviewResult preview)
	{
		try
		{
			var template = GetTemplate(templateId);

			if (template is null)
				throw new Exception("Template is not found.");

			if (!template.SpaceDataList.Contains(spaceDataId))
				throw new Exception("Template does not work for selected space data.");

			var spaceData = GetSpaceData(spaceDataId);
			var processor = GetTemplateProcessor(template.ContentProcessorType);
			var dataTable = QuerySpaceData(spaceData.Id, tfRecordIds);


			return processor.ProcessTemplate(template, dataTable, preview, _serviceProvider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		TfDataTable dataTable,
		ITfTemplatePreviewResult preview)
	{
		try
		{
			var template = GetTemplate(templateId);
			if (template is null)
				throw new Exception("Template is not found.");

			var processor = GetTemplateProcessor(template.ContentProcessorType);
			return processor.ProcessTemplate(template, dataTable, preview, _serviceProvider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void ValidateTemplatePreview(
		Guid templateId,
		ITfTemplatePreviewResult preview)
	{
		try
		{
			var template = GetTemplate(templateId);
			var processor = GetTemplateProcessor(template.ContentProcessorType);
			processor.ValidatePreview(template, preview, _serviceProvider);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfSpaceDataAsOption> GetSpaceDataOptionsForTemplate()
	{
		var result = new List<TfSpaceDataAsOption>();
		var spaceData = GetAllSpaceData();
		var spaceDict = GetSpacesList().ToDictionary(x => x.Id);
		foreach (var item in spaceData)
		{
			result.Add(new TfSpaceDataAsOption
			{
				Id = item.Id,
				Name = item.Name,
				SpaceName = spaceDict[item.SpaceId].Name
			});
		}

		result = result.OrderBy(x => x.SpaceName).ThenBy(x => x.Name).ToList();
		return result;
	}

	public TfTemplate UpdateTemplateSettings(Guid templateId,string settingsJson){ 
		try
		{
			var template = GetTemplate(templateId);
			var form = new TfManageTemplateModel{ 
				ContentProcessorType = template.ContentProcessorType,
				Description = template.Description,
				FluentIconName = template.FluentIconName,
				Id = templateId,
				IsEnabled = template.IsEnabled,
				IsSelectable = template.IsSelectable,
				Name = template.Name,
				SettingsJson = settingsJson,
				SpaceDataList = template.SpaceDataList,
				UserId = template.CreatedBy?.Id
			};
			return UpdateTemplate(form);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}	
	}

	#region <--- validation --->

	internal class TemplateValidator
		: AbstractValidator<TfTemplate>
	{
		private readonly ITfService _service;

		public TemplateValidator(ITfService service)
		{
			_service = service;
		}

		public ValidationResult ValidateCreate(
			TfManageTemplateModel template)
		{
			if (template == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The template object is null.") });
			}

			if (template.Id == Guid.Empty)
			{
				return new ValidationResult(new[] { new ValidationFailure("Id",
					"Id is not specified.") });
			}

			if (string.IsNullOrWhiteSpace(template.Name))
			{
				return new ValidationResult(new[] { new ValidationFailure("Name",
					"Name is not specified.") });
			}

			if (template.ContentProcessorType == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("ContentProcessorType",
					"Content processor type is not specified.") });
			}

			var contentProcessor = _service.GetTemplateProcessor(template.ContentProcessorType);
			if (contentProcessor is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("ContentProcessorType",
					"Content processor type is not found.") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateUpdate(
			TfManageTemplateModel template)
		{
			if (template == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The template object is null.") });
			}

			if (string.IsNullOrWhiteSpace(template.Name))
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"Name is not specified.") });
			}

			if (template.ContentProcessorType == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("ContentProcessorType",
					"Content processor type is not specified.") });
			}

			var contentProcessor = _service.GetTemplateProcessor(template.ContentProcessorType);
			if (contentProcessor is null)
			{
				return new ValidationResult(new[] { new ValidationFailure("ContentProcessorType",
					"Content processor type is not found.") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateDelete(
			TfTemplate template)
		{
			if (template == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The template object is null.") });
			}

			return new ValidationResult();
		}
	}

	#endregion

	#region <--- Utility ---->

	private List<TfTemplate> ToTemplateList(DataTable dt)
	{
		if (dt == null)
		{
			throw new Exception("DataTable is null");
		}

		List<TfTemplate> templateList = new List<TfTemplate>();

		foreach (DataRow dr in dt.Rows)
		{
			TfUser createdBy = null;
			if (dr.Field<Guid?>("created_by").HasValue)
			{
				createdBy = GetUser(dr.Field<Guid>("created_by"));
			}

			TfUser modifiedBy = null;
			if (dr.Field<Guid?>("modified_by").HasValue)
			{
				modifiedBy = GetUser(dr.Field<Guid>("modified_by"));
			}

			string processorTypeName = dr.Field<string>("content_processor_type");
			var contentProcessor = GetTemplateProcessor(processorTypeName);

			TfTemplate asset = new TfTemplate
			{
				Id = dr.Field<Guid>("id"),
				Name = dr.Field<string>("name"),
				Description = dr.Field<string>("description"),
				FluentIconName = dr.Field<string>("icon"),
				SpaceDataList = JsonSerializer.Deserialize<List<Guid>>(dr.Field<string>("space_data_json") ?? "[]"),
				ContentProcessorType = contentProcessor.GetType(),
				ResultType = (TfTemplateResultType)dr.Field<short>("result_type"),
				SettingsJson = dr.Field<string>("settings_json"),
				IsSelectable = dr.Field<bool>("is_selectable"),
				IsEnabled = dr.Field<bool>("is_enabled"),
				CreatedBy = createdBy,
				CreatedOn = dr.Field<DateTime>("created_on"),
				ModifiedBy = modifiedBy,
				ModifiedOn = dr.Field<DateTime>("modified_on"),
			};

			templateList.Add(asset);
		}

		return templateList;
	}

	private static NpgsqlParameter CreateParameter(
		string name,
		object value,
		DbType type)
	{
		NpgsqlParameter par = new NpgsqlParameter(name, type);
		if (value is null)
			par.Value = DBNull.Value;
		else
			par.Value = value;

		return par;
	}

	#endregion



}
