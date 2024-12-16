namespace WebVella.Tefter.Templates.Services;

public partial interface ITemplatesService
{
	public Result<Template> GetTemplate(
		Guid id);

	public Result<List<Template>> GetTemplates();

	public Result<Template> CreateTemplate(
		CreateTemplateModel template);

	public Result<Template> UpdateTemplate(
		UpdateTemplateModel template);

	public Result DeleteTemplate(
		Guid templateId);
}

internal partial class TemplatesService : ITemplatesService
{
	public Result<Template> GetTemplate(Guid id)
	{
		try
		{
			const string SQL = @"SELECT * FROM templates_template WHERE id = @id";

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

	public Result<List<Template>> GetTemplates()
	{
		try
		{
			const string SQL = @"SELECT * FROM templates_template";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL);

			return Result.Ok(ToTemplateList(dt));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get assets.").CausedBy(ex));
		}
	}

	public Result<Template> CreateTemplate(
		CreateTemplateModel template)
	{
		try
		{
			Guid id = Guid.NewGuid();

			DateTime now = DateTime.Now;

			TemplateValidator validator = new TemplateValidator(this);

			var validationResult = validator.ValidateCreate(template, id);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var contentProcessor = GetTemplateProcessor(template.ContentProcessorType).Value;

			List<string> usedColumns = contentProcessor.GetUsedColumns(template.SettingsJson,this);

			string usedColumnsJson = JsonSerializer.Serialize(usedColumns??new List<string>());

			var idPar = CreateParameter("@id", id, DbType.Guid);

			var namePar = CreateParameter("@name", template.Name, DbType.String);

			var descriptionPar = CreateParameter("@description", template.Description, DbType.String);

			var iconPar = CreateParameter("@icon", template.Icon, DbType.String);

			var isEnabledPar = CreateParameter("@is_enabled", template.IsEnabled, DbType.Boolean);

			var isSelectablePar = CreateParameter("@is_selectable", template.IsSelectable, DbType.Boolean);

			var resultTypePar = CreateParameter("@result_type", (short)contentProcessor.ResultType, DbType.Int16);

			var usedColumnsJsonPar = CreateParameter("@used_columns_json", usedColumnsJson, DbType.String);

			var settingsJsonPar = CreateParameter("@settings_json", template.SettingsJson, DbType.String);

			var cptPar = CreateParameter("@content_processor_type", template.ContentProcessorType.FullName, DbType.String);

			var createdOnPar = CreateParameter("@created_on", now, DbType.DateTime2);

			var createdByPar = CreateParameter("@created_by", template.UserId, DbType.Guid);

			var modifiedOnPar = CreateParameter("@modified_on", now, DbType.DateTime2);

			var modifiedByPar = CreateParameter("@modified_by", template.UserId, DbType.Guid);

			const string SQL = @"
				INSERT INTO templates_template(
					id, name, icon, description, used_columns_json, is_enabled, 
					is_selectable, result_type, settings_json, content_processor_type,
					created_on, modified_on, created_by, modified_by)
				VALUES (
					@id, @name, @icon, @description, @used_columns_json, @is_enabled, 
					@is_selectable, @result_type, @settings_json, @content_processor_type,
					@created_on, @modified_on, @created_by, @modified_by )";

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar, namePar, descriptionPar, iconPar,
				isEnabledPar, isSelectablePar, resultTypePar,
				usedColumnsJsonPar, settingsJsonPar, cptPar,
				createdByPar, createdOnPar, modifiedByPar, modifiedOnPar);

			if (dbResult != 1)
			{
				throw new Exception("Failed to insert new row in database for template object");
			}

			return GetTemplate(id);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new template.").CausedBy(ex));
		}
	}

	public Result<Template> UpdateTemplate(
		UpdateTemplateModel template)
	{
		try
		{
			TemplateValidator validator = new TemplateValidator(this);

			var validationResult = validator.ValidateUpdate(template);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var contentProcessor = GetTemplateProcessor(template.ContentProcessorType).Value;

			List<string> usedColumns = contentProcessor.GetUsedColumns(template.SettingsJson, this);

			string usedColumnsJson = JsonSerializer.Serialize(usedColumns ?? new List<string>());

			var idPar = CreateParameter("@id", template.Id, DbType.Guid);

			var namePar = CreateParameter("@name", template.Name, DbType.String);

			var descriptionPar = CreateParameter("@description", template.Description, DbType.String);

			var iconPar = CreateParameter("@icon", template.Icon, DbType.String);

			var isEnabledPar = CreateParameter("@is_enabled", template.IsEnabled, DbType.Boolean);

			var isSelectablePar = CreateParameter("@is_selectable", template.IsSelectable, DbType.Boolean);

			var resultTypePar = CreateParameter("@result_type", (short)template.ResultType, DbType.Int16);

			var usedColumnsJsonPar = CreateParameter("@used_columns_json", usedColumnsJson, DbType.String);

			var settingsJsonPar = CreateParameter("@settings_json", template.SettingsJson, DbType.String);

			var cptPar = CreateParameter("@content_processor_type", template.ContentProcessorType.FullName, DbType.String);

			var modifiedOnPar = CreateParameter("@modified_on", DateTime.Now, DbType.DateTime2);

			var modifiedByPar = CreateParameter("@modified_by", template.UserId, DbType.Guid);

			const string SQL = @"
				UPDATE templates_template
				SET 
					name=@name,
					icon=@icon,
					description=@description,
					used_columns_json=@used_columns_json,
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
				usedColumnsJsonPar, settingsJsonPar, cptPar,
				modifiedByPar, modifiedOnPar);

			if (dbResult != 1)
			{
				throw new Exception("Failed to insert new row in database for template object");
			}

			return GetTemplate(template.Id);
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

			TemplateValidator validator = new TemplateValidator(this);

			var validationResult = validator.ValidateDelete(existingTemplate);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			const string SQL = "DELETE FROM templates_template WHERE id = @id";

			var idPar = CreateParameter("id", templateId, DbType.Guid);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

			if (dbResult != 1)
			{
				throw new Exception("Failed to delete row in database for template object");
			}

			return Result.Ok();

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete template").CausedBy(ex));
		}
	}


	#region <--- validation --->

	internal class TemplateValidator
		: AbstractValidator<Template>
	{
		private readonly ITemplatesService _service;

		public TemplateValidator(ITemplatesService service)
		{
			_service = service;
		}

		public ValidationResult ValidateCreate(
			CreateTemplateModel template,
			Guid id)
		{
			if (template == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The template object is null.") });
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
			if( contentProcessor is null )
			{
				return new ValidationResult(new[] { new ValidationFailure("ContentProcessorType",
					"Content processor type is not found.") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateUpdate(
			UpdateTemplateModel template)
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

			if (contentProcessor.ResultType != template.ResultType)
			{
				return new ValidationResult(new[] { new ValidationFailure("ResultType",
					"Template result type does not match selected content processor result type.") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateDelete(
			Template template)
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

	private List<Template> ToTemplateList(DataTable dt)
	{
		if (dt == null)
		{
			throw new Exception("DataTable is null");
		}

		List<Template> templateList = new List<Template>();

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

			Template asset = new Template
			{
				Id = dr.Field<Guid>("id"),
				Name = dr.Field<string>("name"),
				Description = dr.Field<string>("description"),
				Icon = dr.Field<string>("icon"),
				UsedColumns = JsonSerializer.Deserialize<List<string>>(dr.Field<string>("used_columns_json")),
				ContentProcessorType = Type.GetType(dr.Field<string>("content_processor_type")),
				ResultType = (TemplateResultType)dr.Field<short>("result_type"),
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
