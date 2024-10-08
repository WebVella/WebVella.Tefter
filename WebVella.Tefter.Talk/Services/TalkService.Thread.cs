

using System.Threading;

namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
	Result<List<TalkThread>> GetThreads(
		Guid channelId,
		Guid? skId);

	Result<(Guid, List<TalkThread>)> CreateThread(
		CreateTalkThread thread);

	Result UpdateThread(
		Guid threadid,
		string content);

	Result DeleteThread(
		Guid threadId);
}

internal partial class TalkService : ITalkService
{
	public Result<List<TalkThread>> GetThreads(
		Guid channelId,
		Guid? skId)
	{
		try
		{
			var SQL = @"SELECT 
	tt.id,
	tt.channel_id,
	tt.thread_id,
	tt.type,
	tt.content,
	tt.user_id,
	tt.created_on,
	tt.last_updated_on,
	tt.deleted_on,
	JSON_AGG(idd.*) AS related_shared_key_json
FROM talk_thread tt
	LEFT OUTER JOIN talk_related_sk trs ON tt.id = trs.thread_id
	LEFT OUTER JOIN id_dict idd ON idd.id = trs.id
WHERE tt.channel_id = @channel_id AND ( idd.id = @sk_id OR @sk_id IS NULL )
GROUP BY tt.id,
	tt.channel_id,
	tt.thread_id,
	tt.type,
	tt.content,
	tt.user_id,
	tt.created_on,
	tt.last_updated_on,
	tt.deleted_on
ORDER BY tt.created_on ASC";

			var channelIdPar = TalkUtility.CreateParameter(
				"channel_id",
				channelId,
				DbType.Guid);

			var skIdPar = TalkUtility.CreateParameter(
				"sk_id",
				skId,
				DbType.Guid);

			var dt = _dbService.ExecuteSqlQueryCommand(SQL, channelIdPar, skIdPar);

			return Result.Ok(ToThreadList(dt));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get threads.").CausedBy(ex));
		}
	}

	public Result<(Guid, List<TalkThread>)> CreateThread(
		CreateTalkThread thread)
	{
		try
		{
			if (thread == null)
				throw new NullReferenceException("Thread object is null");

			Guid id = Guid.NewGuid();

			TalkChannelValidator validator = new TalkChannelValidator(this);

			//var validationResult = validator.ValidateCreate(channel);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			var SQL = @"INSERT INTO talk_thread
						(id, channel_id, thread_id, type, content, user_id,
						created_on, last_updated_on, deleted_on)
					VALUES(@id, @channel_id, @thread_id, @type, @content, @user_id,
						@created_on, @last_updated_on, @deleted_on); ";

			var idPar = TalkUtility.CreateParameter(
				"@id",
				id,
				DbType.Guid);

			var channelIdPar = TalkUtility.CreateParameter(
				"@channel_id",
				thread.ChannelId,
				DbType.Guid);

			var threadIdPar = TalkUtility.CreateParameter(
				"@thread_id",
				thread.ThreadId,
				DbType.Guid);

			var typePar = TalkUtility.CreateParameter(
				"@type",
				(short)thread.Type,
				DbType.Int16);

			var contentPar = TalkUtility.CreateParameter(
				"@content",
				thread.Content,
				DbType.String);

			var userIdPar = TalkUtility.CreateParameter(
				"@user_id",
				thread.UserId,
				DbType.Guid);

			var createdOnPar = TalkUtility.CreateParameter(
				"@created_on",
				DateTime.Now,
				DbType.DateTime);

			var lastUpdatedOnPar = TalkUtility.CreateParameter(
				"@last_updated_on",
				null,
				DbType.DateTime);

			var deletedOnPar = TalkUtility.CreateParameter(
				"@deleted_on",
				null,
				DbType.DateTime);

			using (var scope = _dbService.CreateTransactionScope())
			{
				var dbResult = _dbService.ExecuteSqlNonQueryCommand(
					SQL,
					idPar, channelIdPar, threadIdPar,
					typePar, contentPar, userIdPar,
					createdOnPar, lastUpdatedOnPar, deletedOnPar);

				if (dbResult != 1)
					throw new Exception("Failed to insert new row in database for thread object");


				if (thread.RelatedSK != null && thread.RelatedSK.Count > 0)
				{
					foreach (var skId in thread.RelatedSK)
					{
						var skDbResult = _dbService.ExecuteSqlNonQueryCommand(
							"INSERT INTO talk_related_sk (id,thread_id) VALUES (@id, @thread_id)",
								new NpgsqlParameter("@id", skId ),
								new NpgsqlParameter("@thread_id", id));

						if (skDbResult != 1)
							throw new Exception("Failed to insert new row in database for related shared key object");
					}
				}

				scope.Complete();

				var threads = GetThreads(thread.ChannelId, null);

				return Result.Ok((id, threads.Value));
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new thread.").CausedBy(ex));
		}
	}

	public Result UpdateThread(
		Guid threadId,
		string content)
	{
		try
		{
			//TalkChannelValidator validator = new TalkChannelValidator(this);

			//var validationResult = validator.ValidateUpdate(channel);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			var SQL = "UPDATE talk_thread SET " +
				"content=@content, " +
				"last_updated_on=@last_updated_on " +
				"WHERE id = @id";

			var idPar = TalkUtility.CreateParameter(
				"id",
				threadId,
				DbType.Guid);

			var contentPar = TalkUtility.CreateParameter(
				"@content",
				content,
				DbType.String);

			var lastUpdatedPar = TalkUtility.CreateParameter(
				"@last_updated_on",
				DateTime.Now,
				DbType.DateTime);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar,
				contentPar,
				lastUpdatedPar);

			if (dbResult != 1)
				throw new Exception("Failed to update row in database for thread object");

			return Result.Ok();

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update thread.").CausedBy(ex));
		}
	}

	public Result DeleteThread(
		Guid threadId)
	{
		try
		{
			//var existingChannel = GetChannel(channelId).Value;

			//TalkChannelValidator validator = new TalkChannelValidator(this);

			//var validationResult = validator.ValidateDelete(existingChannel);

			//if (!validationResult.IsValid)
			//	return validationResult.ToResult();

			var SQL = "UPDATE talk_thread SET " +
				"deleted_on = @deleted_on " +
				"WHERE id = @id";

			var idPar = TalkUtility.CreateParameter(
				"id",
				threadId,
				DbType.Guid);

			var deletedOnPar = TalkUtility.CreateParameter(
				"@deleted_on",
				DateTime.Now,
				DbType.DateTime);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar,
				deletedOnPar);

			if (dbResult != 1)
				throw new Exception("Failed to update row in database for thread object");

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete thread.").CausedBy(ex));
		}
	}

	private List<TalkThread> ToThreadList(DataTable dt)
	{
		if (dt == null)
			throw new Exception("DataTable is null");


		List<TalkThread> threadList = new List<TalkThread>();

		foreach (DataRow dr in dt.Rows)
		{
			var user = _identityManager.GetUser(dr.Field<Guid>("user_id")).Value;

			TalkThread thread = new TalkThread
			{
				Id = dr.Field<Guid>("id"),
				ChannelId = dr.Field<Guid>("channel_id"),
				ThreadId = dr.Field<Guid?>("thread_id"),
				Content = dr.Field<string>("content"),
				Type = (TalkThreadType)dr.Field<short>("type"),
				User = user,
				CreatedOn = dr.Field<DateTime>("created_on"),
				LastUpdatedOn = dr.Field<DateTime?>("last_updated_on"),
				DeletedOn = dr.Field<DateTime?>("deleted_on"),
				SubThread = new List<TalkThread>(),
				RelatedSK = new Dictionary<Guid, string>()
			};

			var relatedSharedKeysJson = dr.Field<string>("related_shared_key_json");
			if (!String.IsNullOrWhiteSpace(relatedSharedKeysJson) && relatedSharedKeysJson.StartsWith("["))
			{
				var items = JsonSerializer.Deserialize<List<IdDictModel>>(relatedSharedKeysJson);
				foreach (var item in items)
					thread.RelatedSK[item.Id] = item.TextId;
			}

			thread.SubThread.Add(thread);
			threadList.Add(thread);
		}

		//fill sub thread
		foreach (var thread in threadList)
		{
			if (thread.ThreadId is not null)
			{
				var parentThread = threadList.Single(x => x.Id == thread.ThreadId);
				parentThread.SubThread.Add(thread);
			}
		}

		return threadList;
	}

	private class IdDictModel
	{
		[JsonPropertyName("id")]
		public Guid Id { get; set; }

		[JsonPropertyName("text_id")]
		public string TextId { get; set; }
	}

	#region <--- validation --->

	internal class TalkThreadValidator
		: AbstractValidator<TalkThread>
	{
		public TalkThreadValidator(ITalkService service)
		{

			//RuleSet("general", () =>
			//{
			//	RuleFor(channel => channel.Id)
			//		.NotEmpty()
			//		.WithMessage("The channel id is required.");

			//	RuleFor(channel => channel.Name)
			//		.NotEmpty()
			//		.WithMessage("The channel name is required.");

			//});

			//RuleSet("create", () =>
			//{
			//	RuleFor(channel => channel.Id)
			//			.Must((channel, id) => { return service.GetChannel(id).Value == null; })
			//			.WithMessage("There is already existing channel with specified identifier.");

			//	RuleFor(channel => channel.Name)
			//			.Must((channel, name) =>
			//			{
			//				if (string.IsNullOrEmpty(name))
			//					return true;

			//				var channels = service.GetChannels().Value;
			//				return !channels.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
			//			})
			//			.WithMessage("There is already existing channel with same name.");
			//});

			//RuleSet("update", () =>
			//{
			//	RuleFor(channnel => channnel.Id)
			//			.Must((channel, id) =>
			//			{
			//				return service.GetChannel(id).Value != null;
			//			})
			//			.WithMessage("There is not existing channel with specified identifier.");

			//	RuleFor(channel => channel.Name)
			//			.Must((channel, name) =>
			//			{
			//				if (string.IsNullOrEmpty(name))
			//					return true;

			//				var channels = service.GetChannels().Value;
			//				return !channels.Any(x =>
			//					x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim() &&
			//					x.Id != channel.Id
			//				);
			//			})
			//			.WithMessage("There is already existing another channel with same name.");

			//});

			//RuleSet("delete", () =>
			//{
			//	RuleFor(channnel => channnel.Id)
			//		.Must((channel, id) =>
			//		{
			//			return service.GetChannel(id).Value != null;
			//		})
			//		.WithMessage("There is not existing channel with specified identifier.");
			//});

		}

		//public ValidationResult ValidateCreate(
		//	TalkChannel? channel)
		//{
		//	if (channel == null)
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"The channel object is null.") });

		//	return this.Validate(channel, options =>
		//	{
		//		options.IncludeRuleSets("general", "create");
		//	});
		//}

		//public ValidationResult ValidateUpdate(
		//	TalkChannel? channel)
		//{
		//	if (channel == null)
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"The channel object is null.") });

		//	return this.Validate(channel, options =>
		//	{
		//		options.IncludeRuleSets("general", "update");
		//	});
		//}

		//public ValidationResult ValidateDelete(
		//	TalkChannel? channel)
		//{
		//	if (channel == null)
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"A channel with specified identifier is not found.") });

		//	return this.Validate(channel, options =>
		//	{
		//		options.IncludeRuleSets("delete");
		//	});
		//}
	}

	#endregion
}
