﻿using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public List<TfBookmark> GetBookmarksListForUser(
		Guid userId, Guid spaceId);
	public List<TfBookmark> GetSavesListForUser(
		Guid userId, Guid spaceId);	

	public TfBookmark GetBookmark(
		Guid id);

	public void ToggleBookmark(
		Guid userId, Guid spaceId);	
	
	public TfBookmark CreateBookmark(
		TfBookmark bookmark);

	public TfBookmark UpdateBookmark(
		TfBookmark bookmark);

	public void DeleteBookmark(
		Guid id);
}

public partial class TfService : ITfService
{
	public List<TfBookmark> GetBookmarksListForUser(
		Guid userId, Guid spaceId)
	{
		try
		{
			var bookmarks = _dboManager.GetList<TfBookmark>(userId, nameof(TfBookmark.UserId));
			foreach (var bookmark in bookmarks)
				bookmark.Tags = GetBookmarkTags(bookmark.Id);

			return bookmarks.Where(x => String.IsNullOrWhiteSpace(x.Url)).ToList();
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}
	
	public List<TfBookmark> GetSavesListForUser(
		Guid userId, Guid spaceId)
	{
		try
		{
			var bookmarks = _dboManager.GetList<TfBookmark>(userId, nameof(TfBookmark.UserId));
			foreach (var bookmark in bookmarks)
				bookmark.Tags = GetBookmarkTags(bookmark.Id);

			return bookmarks.Where(x => !String.IsNullOrWhiteSpace(x.Url)).ToList();;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}	

	public List<TfBookmark> GetBookmarksListForSpace(
		Guid spaceId)
	{
		try
		{
			return _dboManager.GetList<TfBookmark>(spaceId, nameof(TfBookmark.SpaceId));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public TfBookmark GetBookmark(
		Guid id)
	{
		try
		{
			var bookmark = _dboManager.Get<TfBookmark>(id);
			if (bookmark is not null)
				bookmark.Tags = GetBookmarkTags(id);
			return bookmark;
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	private List<TfTag> GetBookmarkTags(
		Guid bookmarkId)
	{
		try
		{
			return _dboManager.GetListBySql<TfTag>(@"SELECT t.* FROM tf_tag t
				LEFT OUTER JOIN tf_bookmark_tag bt ON bt.tag_id = t.id AND bt.bookmark_id = @bookmark_id
			WHERE bt.tag_id IS NOT NULL AND bt.bookmark_id = @bookmark_id
			", new NpgsqlParameter("bookmark_id", bookmarkId));
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	private List<string> GetUniqueTagsFromText(
		string text)
	{
		try
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
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void ToggleBookmark(
		Guid userId, Guid spaceId)
	{
		var bookmark = GetBookmarksListForUser(userId,spaceId).FirstOrDefault();
		if (bookmark is not null)
		{
			DeleteBookmark(bookmark.Id);
		}
		else
		{
			var space = GetSpace(spaceId);
			CreateBookmark(new TfBookmark()
			{
				Id= Guid.NewGuid(),
				UserId = userId,
				SpaceId = spaceId,
				Name = space.Name ?? "uknown space",
				Description = space.Name ?? "unknown space",
			});
		}
	}

	public TfBookmark CreateBookmark(
		TfBookmark bookmark)
	{
		try
		{
			if (bookmark != null && bookmark.Id == Guid.Empty)
				bookmark.Id = Guid.NewGuid();

			new TfBookmarkValidator(this)
				.ValidateCreate(bookmark)
				.ToValidationException()
				.ThrowIfContainsErrors();

			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				bool success = false;

				success = _dboManager.Insert<TfBookmark>(bookmark);

				if (!success)
					throw new TfDboServiceException("Insert<TfBookmark> failed.");

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
								throw new TfDboServiceException("Insert<TfTag> failed.");

							bookmarkTag = new TfBookmarkTag
							{
								BookmarkId = bookmark.Id,
								TagId = newTag.Id
							};

						}

						success = _dboManager.Insert<TfBookmarkTag>(bookmarkTag);

						if (!success)
							throw new TfDboServiceException("Insert<TfBookmarkTag> failed.");

					}
				}

				scope.Complete();

				PublishEventWithScope(new TfUserUpdatedEvent(GetUser(bookmark.UserId)));
				return GetBookmark(bookmark.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}

	}

	public TfBookmark UpdateBookmark(
		TfBookmark bookmark)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var existingBookmark = GetBookmark(bookmark.Id);

				new TfBookmarkValidator(this)
					.ValidateUpdate(existingBookmark)
					.ToValidationException()
					.ThrowIfContainsErrors();

				bool success = false;

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
						throw new TfDboServiceException("Insert<TfTag> failed.");

					var bookmarkTag = new TfBookmarkTag { BookmarkId = bookmark.Id, TagId = newTag.Id };

					success = _dboManager.Insert<TfBookmarkTag>(bookmarkTag);

					if (!success)
						throw new TfDboServiceException("Insert<TfBookmarkTag> failed.");

				}

				//remove connection to missing tags
				foreach (Guid id in tagIdsToRemove)
				{
					Dictionary<string, Guid> deleteKey = new Dictionary<string, Guid>();
					deleteKey.Add(nameof(TfBookmarkTag.BookmarkId), existingBookmark.Id);
					deleteKey.Add(nameof(TfBookmarkTag.TagId), id);

					success = _dboManager.Delete<TfBookmarkTag>(deleteKey);

					if (!success)
						throw new TfDboServiceException("Delete<TfBookmarkTag> failed.");
				}


				//TODO process tags also
				success = _dboManager.Update<TfBookmark>(bookmark);

				if (!success)
					throw new TfDboServiceException("Update<TfBookmark> failed.");

				scope.Complete();
				PublishEventWithScope(new TfUserUpdatedEvent(GetUser(bookmark.UserId)));

				return GetBookmark(bookmark.Id);
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}

	public void DeleteBookmark(
		Guid id)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var existingBookmark = GetBookmark(id);

				new TfBookmarkValidator(this)
					.ValidateDelete(existingBookmark)
					.ToValidationException()
					.ThrowIfContainsErrors();


				bool success = false;

				//remove connection to tags
				foreach (Guid tagId in existingBookmark.Tags.Select(x => x.Id).ToList())
				{
					Dictionary<string, Guid> deleteKey = new Dictionary<string, Guid>();
					deleteKey.Add(nameof(TfBookmarkTag.BookmarkId), existingBookmark.Id);
					deleteKey.Add(nameof(TfBookmarkTag.TagId), tagId);

					success = _dboManager.Delete<TfBookmarkTag>(deleteKey);

					if (!success)
						throw new TfDboServiceException("Delete<TfBookmarkTag> failed.");
				}

				success = _dboManager.Delete<TfBookmark>(id);

				if (!success)
					throw new TfDboServiceException("Delete<TfBookmark> failed.");
				
				scope.Complete();
				
				PublishEventWithScope(new TfUserUpdatedEvent(GetUser(existingBookmark.UserId)));
			}
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}



	#region <--- validation --->

	internal class TfBookmarkValidator : AbstractValidator<TfBookmark>
	{
		public TfBookmarkValidator(
			ITfService tfService)
		{

			RuleSet("general", () =>
			{
				RuleFor(bookmark => bookmark.Id)
					.NotEmpty()
					.WithMessage("The bookmark id is required.");

				RuleFor(bookmark => bookmark.Name)
					.NotEmpty()
					.WithMessage("The bookmark name is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(bookmark => bookmark.Id)
						.Must((bookmark, id) => { return tfService.GetBookmark(id) == null; })
						.WithMessage("There is already existing bookmark with specified identifier.");

			});

			RuleSet("update", () =>
			{
				RuleFor(bookmark => bookmark.Id)
						.Must((bookmark, id) =>
						{
							return tfService.GetBookmark(id) != null;
						})
						.WithMessage("There is not existing bookmark with specified identifier.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfBookmark bookmark)
		{
			if (bookmark == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The bookmark is null.") });

			return this.Validate(bookmark, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfBookmark bookmark)
		{
			if (bookmark == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The bookmark is null.") });

			return this.Validate(bookmark, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfBookmark bookmark)
		{
			if (bookmark == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The bookmark with specified identifier is not found.") });

			return this.Validate(bookmark, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
