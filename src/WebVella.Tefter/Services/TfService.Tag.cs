namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	TfTag? GetTag(Guid id);
	TfTag? GetTag(string label);
	List<TfTag> GetTags(string? search = null);
	TfTag CreateTag(TfTag tag);
	TfTag UpdateTag(TfTag tag);
	void DeleteTag(Guid tagId);

	void CheckRemoveOrphanTags(Guid id);
}

public partial class TfService
{
	public TfTag? GetTag(Guid id)
	{
		try
		{
			const string sql = @"SELECT * FROM tf_tag WHERE id = @id";

			var dt = _dbService.ExecuteSqlQueryCommand(sql, CreateParameter("@id", id, DbType.Guid));

			var list = ToTagList(dt);

			if (list.Count == 0)
				return null;
			return list[0];
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfTag? GetTag(string label)
	{
		if (String.IsNullOrWhiteSpace(label))
			return null;
		try
		{
			const string sql = @"SELECT * FROM tf_tag WHERE label = @label";

			var dt = _dbService.ExecuteSqlQueryCommand(sql, CreateParameter("@label", label.Trim(), DbType.String));

			var list = ToTagList(dt);

			if (list.Count == 0)
				return null;
			return list[0];
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public List<TfTag> GetTags(string? search = null)
	{
		try
		{
			List<NpgsqlParameter> parameters = new();
			string sql = "SELECT * FROM tf_tag ORDER BY label ASC";
			if (!String.IsNullOrWhiteSpace(search))
			{
				sql = "SELECT * FROM tf_tag WHERE label ILIKE CONCAT ('%', @search, '%')  ORDER BY label ASC";
				parameters.Add(CreateParameter("@search", search.Trim(), DbType.String));
			}

			var dt = _dbService.ExecuteSqlQueryCommand(sql, parameters);
			return ToTagList(dt);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfTag CreateTag(TfTag tag)
	{
		if (tag.Id == Guid.Empty)
			tag.Id = Guid.NewGuid();

		new TagValidator(this)
			.ValidateCreate(tag)
			.ToValidationException()
			.ThrowIfContainsErrors();

		if(tag.Label.StartsWith("#"))
			tag.Label = tag.Label.Substring(1);

		var idPar = CreateParameter("@id", tag.Id, DbType.Guid);
		var labelPar = CreateParameter("@label", tag.Label, DbType.String);
		const string sql = @"
				INSERT INTO tf_tag(id, label)
				VALUES (@id, @label);";

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			sql,
			idPar, labelPar);

		if (dbResult != 1)
		{
			throw new TfException("Failed to insert new row in database for tag object");
		}

		return GetTag(tag.Id)!;
	}

	public TfTag UpdateTag(TfTag tag)
	{
		new TagValidator(this)
			.ValidateUpdate(tag)
			.ToValidationException()
			.ThrowIfContainsErrors();

		if (tag.Label.StartsWith("#"))
			tag.Label = tag.Label.Substring(1);
		var idPar = CreateParameter("@id", tag.Id, DbType.Guid);
		var labelPar = CreateParameter("@label", tag.Label, DbType.String);
		const string sql = @"
				UPDATE tf_tag
				SET label=@label
				WHERE id = @id;";

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			sql,
			idPar, labelPar);

		if (dbResult != 1)
		{
			throw new TfException("Failed to insert new row in database for tag object");
		}

		return GetTag(tag.Id)!;
	}

	public void DeleteTag(Guid tagId)
	{
		new TagValidator(this)
			.ValidateDeleteById(tagId)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var idPar = CreateParameter("@id", tagId, DbType.Guid);
		const string sql = @"
				DELETE FROM tf_tag 
				       WHERE id = @id;";

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			sql,
			idPar);

		if (dbResult != 1)
		{
			throw new TfException("Failed to insert new row in database for tag object");
		}
	}

	public void DeleteTag(string label)
	{
		new TagValidator(this)
			.ValidateDeleteByLabel(label)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var tag = GetTag(label)!;
		var idPar = CreateParameter("@id", tag.Id, DbType.Guid);
		const string sql = @"
				DELETE FROM tf_tag 
				       WHERE id = @id;";

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			sql,
			idPar);

		if (dbResult != 1)
		{
			throw new TfException("Failed to insert new row in database for tag object");
		}
	}

	public void CheckRemoveOrphanTags(Guid id)
	{
		var bookmarkTags = _dboManager.GetList<TfBookmarkTag>(id,nameof(TfBookmarkTag.TagId));
		if(bookmarkTags.Count > 0) return;
		var pageTags = _dboManager.GetList<TfSpacePageTag>(id, nameof(TfSpacePageTag.TagId));
		if (pageTags.Count > 0) return;

		//TODO BOZ: How to check and maintain for tags created in Talk and Assets
		//probably an interface method

		//TODO BOZ: Enable once the addon tag usage is implemented
		//DeleteTag(id);


	}


	#region <--- validation --->

	internal class TagValidator
		: AbstractValidator<TfTag>
	{
		private readonly ITfService _service;

		public TagValidator(ITfService service)
		{
			_service = service;
		}

		public ValidationResult ValidateCreate(
			TfTag tag)
		{
			if (tag.Id == Guid.Empty)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Id),
						"Id is not specified.")
				});
			}

			if (string.IsNullOrWhiteSpace(tag.Label))
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Label),
						"Label is not specified.")
				});
			}

			var existing = _service.GetTag(tag.Id);
			if (existing is not null)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Id),
						"Id already exists.")
				});
			}

			existing = _service.GetTag(tag.Label);
			if (existing is not null)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Label),
						"Label already exists.")
				});
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateUpdate(
			TfTag tag)
		{
			if (tag.Id == Guid.Empty)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Id),
						"Id is not specified.")
				});
			}

			if (string.IsNullOrWhiteSpace(tag.Label))
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Label),
						"Label is not specified.")
				});
			}

			var existing = _service.GetTag(tag.Label);
			if (existing is not null && existing.Id != tag.Id)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(tag.Label),
						"Tag already exists.")
				});
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateDeleteById(Guid id)
		{
			if (id == Guid.Empty)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(TfTag.Id),
						"Id is not specified.")
				});
			}

			var existing = _service.GetTag(id);
			if (existing is null)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(TfTag.Id),
						"Tag not found.")
				});
			}

			return new ValidationResult();
		}
		public ValidationResult ValidateDeleteByLabel(string label)
		{
			if (String.IsNullOrWhiteSpace(label))
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(TfTag.Label),
						"Label is not specified.")
				});
			}

			var existing = _service.GetTag(label);
			if (existing is null)
			{
				return new ValidationResult(new[]
				{
					new ValidationFailure(nameof(TfTag.Id),
						"Tag not found.")
				});
			}

			return new ValidationResult();
		}
	}

	#endregion

	#region <--- private --->

	private List<TfTag> ToTagList(DataTable dt)
	{
		if (dt == null)
		{
			throw new Exception("DataTable is null");
		}

		List<TfTag> templateList = new();

		foreach (DataRow dr in dt.Rows)
		{
			TfTag tag = new TfTag { Id = dr.Field<Guid>("id"), Label = dr.Field<string>("label") ?? String.Empty, };

			templateList.Add(tag);
		}

		return templateList;
	}

	#endregion
}