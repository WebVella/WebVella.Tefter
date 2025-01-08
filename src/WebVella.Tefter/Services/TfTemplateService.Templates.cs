using WebVella.Tefter.Models;

namespace WebVella.Tefter.Services;

public partial interface ITfTemplateService
{
	public Result<TfTemplate> GetTemplate(
		Guid id);

	public Result<List<TfTemplate>> GetTemplates();

	public Result<TfTemplate> CreateTemplate(
		TfManageTemplateModel template);

	public Result<TfTemplate> UpdateTemplate(
		TfManageTemplateModel template);

	public Result DeleteTemplate(
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
}

internal partial class TfTemplateService : ITfTemplateService
{
	public Result<TfTemplate> GetTemplate(Guid id)
	{
		try
		{
			const string SQL = @"SELECT * FROM template WHERE id = @id";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL, CreateParameter("@id", id, DbType.Guid));

			var list = ToTemplateList(dt);

			if (list.Count == 0)
			{
				return null;
			}

			return Result.Ok(list[0]);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get template.").CausedBy(ex));
		}
	}

	public Result<List<TfTemplate>> GetTemplates()
	{
		try
		{
			const string SQL = @"SELECT * FROM template";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL);

			return Result.Ok(ToTemplateList(dt));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get assets.").CausedBy(ex));
		}
	}

	public Result<TfTemplate> CreateTemplate(
		TfManageTemplateModel template)
	{
		try
		{
			if (template == null)
			{
				return Result.Fail(new Error("Template object is null"));
			}

			if (template.ContentProcessorType == null)
			{
				return Result.Fail(new Error("Content processor is not selected"));
			}

			var contentProcessor = GetTemplateProcessor(template.ContentProcessorType).Value;
			
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				if (template is not null && template.Id == Guid.Empty)
				{
					template.Id = Guid.NewGuid();
				}

				var errors = contentProcessor.OnCreate(template, _serviceProvider);
				
				if (errors is not null && errors.Count > 0)
				{
					return Result.Fail(errors);
				}

				DateTime now = DateTime.Now;

				TemplateValidator validator = new TemplateValidator(this);

				var validationResult = validator.ValidateCreate(template);

				if (!validationResult.IsValid)
				{
					return validationResult.ToResult();
				}

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
				INSERT INTO template(
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
					throw new Exception("Failed to insert new row in database for template object");
				}

				scope.Complete();
			}

			var resultTemplate = GetTemplate(template.Id).Value;
			
			contentProcessor.OnCreated(resultTemplate, _serviceProvider);
			
			return resultTemplate;
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new template.").CausedBy(ex));
		}
	}

	public Result<TfTemplate> UpdateTemplate(
		TfManageTemplateModel template)
	{
		try
		{
			if (template == null)
			{
				return Result.Fail(new Error("Template object is null"));
			}

			if (template.ContentProcessorType == null)
			{
				return Result.Fail(new Error("Content processor is not selected"));
			}

			var contentProcessor = GetTemplateProcessor(template.ContentProcessorType).Value;

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var errors = contentProcessor.OnUpdate(template, _serviceProvider);
				
				if (errors is not null && errors.Count > 0)
				{
					return Result.Fail(errors);
				}

				TemplateValidator validator = new TemplateValidator(this);

				var validationResult = validator.ValidateUpdate(template);

				if (!validationResult.IsValid)
				{
					return validationResult.ToResult();
				}

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
				UPDATE template
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

			var resultTemplate = GetTemplate(template.Id).Value;

			contentProcessor.OnUpdated(resultTemplate, _serviceProvider);

			return resultTemplate;
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update template.").CausedBy(ex));
		}
	}

	public Result DeleteTemplate(
		Guid templateId)
	{
		try
		{
			var existingTemplate = GetTemplate(templateId).Value;

			if (existingTemplate == null)
			{
				return Result.Fail(new Error("Template is not found"));
			}

			var contentProcessor = GetTemplateProcessor(existingTemplate.ContentProcessorType).Value;

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var errors = contentProcessor.OnDelete(existingTemplate, _serviceProvider);

				if (errors is not null && errors.Count > 0)
				{
					return Result.Fail(errors);
				}

				TemplateValidator validator = new TemplateValidator(this);

				var validationResult = validator.ValidateDelete(existingTemplate);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				const string SQL = "DELETE FROM template WHERE id = @id";

				var idPar = CreateParameter("id", templateId, DbType.Guid);

				var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

				if (dbResult != 1)
				{
					throw new Exception("Failed to delete row in database for template object");
				}

				scope.Complete();
			}

			contentProcessor.OnDeleted(existingTemplate, _serviceProvider);

			return Result.Ok();

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete template").CausedBy(ex));
		}
	}


	#region <--- validation --->

	internal class TemplateValidator
		: AbstractValidator<TfTemplate>
	{
		private readonly ITfTemplateService _service;

		public TemplateValidator(ITfTemplateService service)
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

			var contentProcessor = _service.GetTemplateProcessor(template.ContentProcessorType).Value;
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

			var contentProcessor = _service.GetTemplateProcessor(template.ContentProcessorType).Value;
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
			User createdBy = null;
			if (dr.Field<Guid?>("created_by").HasValue)
			{
				createdBy = _identityManager.GetUser(dr.Field<Guid>("created_by")).Value;
			}

			User modifiedBy = null;
			if (dr.Field<Guid?>("modified_by").HasValue)
			{
				modifiedBy = _identityManager.GetUser(dr.Field<Guid>("modified_by")).Value;
			}

			string processorTypeName = dr.Field<string>("content_processor_type");
			var contentProcessor = GetTemplateProcessor(processorTypeName).Value;

			TfTemplate asset = new TfTemplate
			{
				Id = dr.Field<Guid>("id"),
				Name = dr.Field<string>("name"),
				Description = dr.Field<string>("description"),
				FluentIconName = dr.Field<string>("icon"),
				SpaceDataList = JsonSerializer.Deserialize<List<Guid>>(dr.Field<string>("space_data_json")??"[]"),
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

	public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds)
	{
		var template = GetTemplate(templateId).Value;
		var spaceData = _spaceManager.GetSpaceData(spaceDataId).Value; 
		var processor = GetTemplateProcessor(template.ContentProcessorType).Value;
		return processor.GenerateTemplatePreviewResult(template, spaceData, tfRecordIds, _serviceProvider);
	}

	public ITfTemplateResult ProcessTemplate(
		Guid templateId,
		Guid spaceDataId,
		List<Guid> tfRecordIds,
		ITfTemplatePreviewResult preview)
	{
		var template = GetTemplate(templateId).Value;
		var spaceData = _spaceManager.GetSpaceData(spaceDataId).Value;
		var processor = GetTemplateProcessor(template.ContentProcessorType).Value;
		return processor.ProcessTemplate(template, spaceData, tfRecordIds, preview, _serviceProvider);
	}

}
