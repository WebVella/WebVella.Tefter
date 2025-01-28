using WebVella.Tefter.Identity;

namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
	public Result<List<TfBookmark>> GetBookmarksListForUser(
		Guid userId);
	public Result<List<TfBookmark>> GetBookmarksListForSpaceView(
		Guid spaceViewId);

	public Result<TfBookmark> GetBookmark(
		Guid id);

	public Result<TfBookmark> CreateBookmark(
		TfBookmark bookmark);

	public Result<TfBookmark> UpdateBookmark(
		TfBookmark bookmark);

	public Result DeleteBookmark(
		Guid id);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public Result<List<TfBookmark>> GetBookmarksListForUser(
		Guid userId)
	{
		try
		{
			var bookmarks = _dboManager.GetList<TfBookmark>(userId, nameof(TfBookmark.UserId));
			foreach (var bookmark in bookmarks)
				bookmark.Tags = GetBookmarkTags(bookmark.Id);

			return Result.Ok(bookmarks);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of user bookmarks").CausedBy(ex));
		}
	}

	public Result<List<TfBookmark>> GetBookmarksListForSpaceView(
		Guid spaceViewId)
	{
		try
		{
			var bookmarks = _dboManager.GetList<TfBookmark>(spaceViewId, nameof(TfBookmark.SpaceViewId));
			foreach (var bookmark in bookmarks)
				bookmark.Tags = GetBookmarkTags(bookmark.Id);

			return Result.Ok(bookmarks);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of user bookmarks").CausedBy(ex));
		}
	}

	public Result<TfBookmark> GetBookmark(
		Guid id)
	{
		try
		{
			var bookmark = _dboManager.Get<TfBookmark>(id);
			if (bookmark is not null)
				bookmark.Tags = GetBookmarkTags(id);
			return Result.Ok(bookmark);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of user bookmarks").CausedBy(ex));
		}
	}

	private List<TfTag> GetBookmarkTags(
		Guid bookmarkId)
	{
		return _dboManager.GetListBySql<TfTag>(@"SELECT t.* FROM tag t
				LEFT OUTER JOIN bookmark_tags bt ON bt.tag_id = t.id AND bt.bookmark_id = @bookmark_id
			WHERE bt.tag_id IS NOT NULL AND bt.bookmark_id = @bookmark_id
			", new NpgsqlParameter("bookmark_id", bookmarkId));

	}

	private List<string> GetUniqueTagsFromText(
		string text)
	{
		var result = new List<string>();

		if (string.IsNullOrWhiteSpace(text))
			return result;

		var regex = new Regex(@"#\w+");
		var matches = regex.Matches(text);
		foreach (var match in matches)
		{
			var tag = match.ToString().ToLowerInvariant().Trim().Substring(1);
			if (!string.IsNullOrWhiteSpace(tag) && !result.Contains(tag))
				result.Add(tag);
		}

		return result;
	}

	public Result<TfBookmark> CreateBookmark(
		TfBookmark bookmark)
	{
		try
		{
			if (bookmark != null && bookmark.Id == Guid.Empty)
				bookmark.Id = Guid.NewGuid();

			//TfSpaceValidator validator =
			//	new TfSpaceValidator(_dboManager, this);

			//var validationResult = validator.ValidateCreate(space);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				bool success = false;

				success = _dboManager.Insert<TfBookmark>(bookmark);

				if (!success)
					return Result.Fail(new DboManagerError("Insert", bookmark));

				var textTags = GetUniqueTagsFromText(bookmark.Description);
				if (textTags.Count > 0)
				{
					var allTags = _dboManager.GetList<TfTag>();

					foreach (var textTag in textTags)
					{
						var tag = allTags.SingleOrDefault(x => x.Label == textTag);

						TfBookmarkTag bookmarkTag = null;

						if (tag is not null)
						{
							bookmarkTag = new TfBookmarkTag
							{
								BookmarkId = bookmark.Id,
								TagId = tag.Id
							};
						}
						else
						{
							var newTag = new TfTag
							{
								Id = Guid.NewGuid(),
								Label = textTag
							};

							success = _dboManager.Insert<TfTag>(newTag);
							if (!success)
								return Result.Fail(new DboManagerError("Insert", newTag));

							bookmarkTag = new TfBookmarkTag
							{
								BookmarkId = bookmark.Id,
								TagId = newTag.Id
							};

						}

						success = _dboManager.Insert<TfBookmarkTag>(bookmarkTag);

						if (!success)
							return Result.Fail(new DboManagerError("Insert", bookmarkTag));

					}
				}

				scope.Complete();

				return GetBookmark(bookmark.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create user bookmark").CausedBy(ex));
		}
	}

	public Result<TfBookmark> UpdateBookmark(
		TfBookmark bookmark)
	{
		try
		{
			//TfSpaceValidator validator =
			//	new TfSpaceValidator(_dboManager, this);

			//var validationResult = validator.ValidateUpdate(bookmark);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				bool success = false;

				var existingBookmark = GetBookmark(bookmark.Id).Value;

				var textTags = GetUniqueTagsFromText(bookmark.Description);

				List<string> tagsToAdd = textTags
					.Where(t => !existingBookmark.Tags.Any(x => x.Label == t))
					.ToList();

				List<Guid> tagIdsToRemove = existingBookmark.Tags
					.Where(x => !textTags.Contains(x.Label))
					.Select(x => x.Id)
					.ToList();


				//add new tags
				foreach (var textTag in tagsToAdd)
				{
					var newTag = new TfTag { Id = Guid.NewGuid(), Label = textTag };

					success = _dboManager.Insert<TfTag>(newTag);

					if (!success)
						return Result.Fail(new DboManagerError("Insert", newTag));

					var bookmarkTag = new TfBookmarkTag { BookmarkId = bookmark.Id, TagId = newTag.Id };

					success = _dboManager.Insert<TfBookmarkTag>(bookmarkTag);

					if (!success)
						return Result.Fail(new DboManagerError("Insert", bookmarkTag));

				}

				//remove connection to missing tags
				foreach (Guid id in tagIdsToRemove)
				{
					Dictionary<string, Guid> deleteKey = new Dictionary<string, Guid>();
					deleteKey.Add(nameof(TfBookmarkTag.BookmarkId), existingBookmark.Id);
					deleteKey.Add(nameof(TfBookmarkTag.TagId), id);

					success = _dboManager.Delete<TfBookmarkTag>(deleteKey);

					if (!success)
						return Result.Fail(new DboManagerError("Delete", deleteKey));
				}


				//TODO process tags also
				success = _dboManager.Update<TfBookmark>(bookmark);

				if (!success)
					return Result.Fail(new DboManagerError("Update", bookmark));

				scope.Complete();

				return GetBookmark(bookmark.Id);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update user bookmark").CausedBy(ex));
		}

	}

	public Result DeleteBookmark(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				//TfSpaceValidator validator =
				//	new TfSpaceValidator(_dboManager, this);

				//var space = GetSpace(id).Value;

				//var validationResult = validator.ValidateDelete(space);

				//if (!validationResult.IsValid)
				//	return validationResult.ToResult();

				bool success = false;

				var existingBookmark = GetBookmark(id).Value;

				//remove connection to tags
				foreach (Guid tagId in existingBookmark.Tags.Select(x => x.Id).ToList())
				{
					Dictionary<string, Guid> deleteKey = new Dictionary<string, Guid>();
					deleteKey.Add(nameof(TfBookmarkTag.BookmarkId), existingBookmark.Id);
					deleteKey.Add(nameof(TfBookmarkTag.TagId), tagId);

					success = _dboManager.Delete<TfBookmarkTag>(deleteKey);

					if (!success)
						return Result.Fail(new DboManagerError("Delete", deleteKey));
				}

				success = _dboManager.Delete<TfBookmark>(id);

				if (!success)
					return Result.Fail(new DboManagerError("Delete", id));

				scope.Complete();

				return Result.Ok();
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete user bookmark.").CausedBy(ex));
		}
	}



	#region <--- validation --->

	internal class TfBookmarkValidator
	: AbstractValidator<TfBookmark>
	{
		public TfBookmarkValidator(
			ITfDboManager dboManager,
			ITfSpaceManager spaceManager)
		{

			RuleSet("general", () =>
			{
				RuleFor(space => space.Id)
					.NotEmpty()
					.WithMessage("The space id is required.");

				RuleFor(space => space.Name)
					.NotEmpty()
					.WithMessage("The space name is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) => { return spaceManager.GetSpace(id).Value == null; })
						.WithMessage("There is already existing space with specified identifier.");

				RuleFor(space => space.Name)
						.Must((space, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var spaces = spaceManager.GetSpacesList().Value;
							return !spaces.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing space with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(space => space.Id)
						.Must((space, id) =>
						{
							return spaceManager.GetSpace(id).Value != null;
						})
						.WithMessage("There is not existing space with specified identifier.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		//public ValidationResult ValidateCreate(
		//	TfSpace space)
		//{
		//	if (space == null)
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"The space is null.") });

		//	return this.Validate(space, options =>
		//	{
		//		options.IncludeRuleSets("general", "create");
		//	});
		//}

		//public ValidationResult ValidateUpdate(
		//	TfSpace space)
		//{
		//	if (space == null)
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"The space is null.") });

		//	return this.Validate(space, options =>
		//	{
		//		options.IncludeRuleSets("general", "update");
		//	});
		//}

		//public ValidationResult ValidateDelete(
		//	TfSpace space)
		//{
		//	if (space == null)
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"The space with specified identifier is not found.") });

		//	return this.Validate(space, options =>
		//	{
		//		options.IncludeRuleSets("delete");
		//	});
		//}
	}

	#endregion

}
