namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
	event EventHandler<TalkThread> ThreadCreated;
	event EventHandler<TalkThread> ThreadUpdated;
	event EventHandler<TalkThread> ThreadDeleted;
	public TalkThread GetThread(
		Guid id);

	public List<TalkThread> GetThreads(
		Guid channelId,
		string dataIdentityValue = null);

    public Guid CreateThread(
		CreateTalkThread thread);

	public Guid CreateSubThread(
		CreateTalkSubThread thread);

	public void UpdateThread(
		Guid threadid,
		string content);

	public void DeleteThread(
		Guid threadId);
}

internal partial class TalkService : ITalkService
{
	#region << Events >>
	public event EventHandler<TalkThread> ThreadCreated = default!;
	public event EventHandler<TalkThread> ThreadUpdated = default!;
	public event EventHandler<TalkThread> ThreadDeleted = default!;
	#endregion

	public TalkThread GetThread(
		Guid id)
	{
		const string SQL = @"SELECT id, channel_id FROM talk_thread WHERE id = @id";

		var threadIdPar = TalkUtility.CreateParameter(
			"id",
			id,
			DbType.Guid);

		var dt = _dbService.ExecuteSqlQueryCommand(SQL, threadIdPar);
		if (dt.Rows.Count == 0)
			return null;

		Guid channelId = (Guid)dt.Rows[0]["channel_id"];

		var threads = GetThreads(channelId, dataIdentityValue: null);

		var threadsAsFlatList = new List<TalkThread>();

		foreach (var thread in threads)
		{
			threadsAsFlatList.Add(thread);
			foreach (var subThread in thread.SubThread)
				threadsAsFlatList.Add(subThread);
		}

		return threadsAsFlatList.SingleOrDefault(x => x.Id == id);
	}

	public List<TalkThread> GetThreads(
		Guid channelId,
		string dataIdentityValue = null)
	{

		string channelDataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;
		var channel = GetChannel(channelId);
		if( channel is null)
			throw new Exception($"Failed to find channel with id '{channelId}'");
		else if (!string.IsNullOrWhiteSpace(channel.DataIdentity))
            channelDataIdentity = channel?.DataIdentity;

        string SQL_WITHOUT_DATA_IDENTITY =
$@"WITH sk_identity_info AS (
	SELECT trs.id, JSON_AGG( dic.* ) AS json_result
	FROM talk_thread trs
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trs.identity_row_id AND dic.data_identity_2 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_1 = '{channelDataIdentity}' ) OR
			( dic.value_1 = trs.identity_row_id AND dic.data_identity_1 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_2 = '{channelDataIdentity}' ) 
	GROUP BY trs.id
), 
root_threads AS (
	SELECT id 
	FROM talk_thread
	WHERE channel_id = @channel_id AND thread_id IS NULL 
)
SELECT 
	tt.id,
	tt.channel_id,
	tt.thread_id,
	tt.type,
	tt.content,
	tt.user_id,
	tt.created_on,
	tt.last_updated_on,
	tt.visible_in_channel,
	tt.deleted_on,
	sk_identity_info.json_result AS identity_connection_json
FROM talk_thread tt
	LEFT OUTER JOIN sk_identity_info  ON tt.id = sk_identity_info.id
	LEFT OUTER JOIN root_threads  rt ON rt.id = tt.id OR tt.thread_id = rt.id
WHERE rt.id IS NOT NULL
ORDER BY tt.created_on DESC
";

		string SQL_WITH_DATA_IDENTITY =
$@"WITH sk_identity_info AS (
	SELECT trs.id, JSON_AGG( dic.* ) AS json_result
	FROM talk_thread trs
			LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trs.identity_row_id AND dic.data_identity_2 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_1 = '{channelDataIdentity}' ) OR
			( dic.value_1 = trs.identity_row_id AND dic.data_identity_1 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND dic.data_identity_2 = '{channelDataIdentity}' ) 
	GROUP BY trs.id
), 
root_threads AS (
	SELECT id 
	FROM talk_thread tt
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			  ( dic.value_2 = tt.identity_row_id AND dic.data_identity_2 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND 
				dic.data_identity_1 = '{channelDataIdentity}' AND dic.value_1 = @identity_value ) 
			OR
			  ( dic.value_1 = tt.identity_row_id AND dic.data_identity_1 = '{TfConstants.TF_ROW_ID_DATA_IDENTITY}' AND 
				dic.data_identity_2 = '{channelDataIdentity}' AND dic.value_2 = @identity_value ) 
	WHERE channel_id = @channel_id AND thread_id IS NULL AND dic.data_identity_1 IS NOT NULL AND dic.data_identity_2 IS NOT NULL
)
SELECT 
	tt.id,
	tt.channel_id,
	tt.thread_id,
	tt.type,
	tt.content,
	tt.user_id,
	tt.created_on,
	tt.last_updated_on,
	tt.visible_in_channel,
	tt.deleted_on,
	sk_identity_info.json_result AS identity_connection_json
FROM talk_thread tt
	LEFT OUTER JOIN sk_identity_info  ON tt.id = sk_identity_info.id
	LEFT OUTER JOIN root_threads  rt ON rt.id = tt.id OR tt.thread_id = rt.id
WHERE rt.id IS NOT NULL
ORDER BY tt.created_on DESC";

		string sql = string.Empty;

		List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

		if (dataIdentityValue is not null)
		{
			sql = SQL_WITH_DATA_IDENTITY;

			var channelIdPar = TalkUtility.CreateParameter(
				"channel_id",
				channelId,
				DbType.Guid);

			parameters.Add(channelIdPar);

			var skIdPar = TalkUtility.CreateParameter(
                "identity_value",
				dataIdentityValue,
				DbType.String);

			parameters.Add(skIdPar);
		}
		else
		{
			sql = SQL_WITHOUT_DATA_IDENTITY;

			var channelIdPar = TalkUtility.CreateParameter(
				"channel_id",
				channelId,
				DbType.Guid);

			parameters.Add(channelIdPar);
		}

		var dt = _dbService.ExecuteSqlQueryCommand(sql, parameters);

		return ToThreadList(dt);
	}

	public Guid CreateThread(
		CreateTalkThread thread)
	{
		if (thread == null)
			throw new NullReferenceException("Thread object is null");

		Guid id = Guid.NewGuid();

		new TalkThreadValidator(this)
			.ValidateCreate(thread, id)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = @"INSERT INTO talk_thread
						(id, channel_id, thread_id, type, content, user_id,
						created_on, last_updated_on, deleted_on, visible_in_channel)
					VALUES(@id, @channel_id, @thread_id, @type, @content, @user_id,
						@created_on, @last_updated_on, @deleted_on,@visible_in_channel); ";

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
			null,
			DbType.Guid);

		var typePar = TalkUtility.CreateParameter(
			"@type",
			(short)thread.Type,
			DbType.Int16);

		var contentPar = TalkUtility.CreateParameter(
			"@content",
			thread.Content,
			DbType.String);

		var visibleInChannelPar = TalkUtility.CreateParameter(
			"@visible_in_channel",
			true,
			DbType.Boolean);

		var userIdPar = TalkUtility.CreateParameter(
			"@user_id",
			thread.UserId,
			DbType.Guid);

		var createdOnPar = TalkUtility.CreateParameter(
			"@created_on",
			DateTime.Now,
			DbType.DateTime2);

		var lastUpdatedOnPar = TalkUtility.CreateParameter(
			"@last_updated_on",
			null,
			DbType.DateTime2);

		var deletedOnPar = TalkUtility.CreateParameter(
			"@deleted_on",
			null,
			DbType.DateTime2);

		using (var scope = _dbService.CreateTransactionScope())
		{
			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar, channelIdPar, threadIdPar,
				typePar, contentPar, userIdPar,
				createdOnPar, lastUpdatedOnPar,
				deletedOnPar, visibleInChannelPar);

			if (dbResult != 1)
				throw new Exception("Failed to insert new row in database for thread object");

			if (thread.DataIdentityValues != null && thread.DataIdentityValues.Count > 0)
			{
				var threadIdentityRowId = id.ToSha1();
				TalkChannel channel = null;
				channel = GetChannel(thread.ChannelId);
				if (channel is null)
					throw new Exception($"Failed to find channel with id '{thread.ChannelId}' for thread");
				

				TfDataIdentity channelDataIdentity = null;
				if (!string.IsNullOrWhiteSpace(channel.DataIdentity))
				{
					channelDataIdentity = _tfService.GetDataIdentity(channel.DataIdentity);
					if (channelDataIdentity is null)
						throw new Exception($"Failed to find data identity '{channel.DataIdentity}' for channel");
				}
				
				if(channelDataIdentity is null)
					channelDataIdentity = _tfService.GetDataIdentity(TfConstants.TF_ROW_ID_DATA_IDENTITY);

				foreach(var dataIdentityValue in thread.DataIdentityValues )
				{
					if (!dataIdentityValue.IsSha1())
						throw new Exception($"Data identity value '{dataIdentityValue}' is not a valid SHA1 value");

                    _tfService.CreateDataIdentityConnection(new TfDataIdentityConnection
					{
						DataIdentity1 = channelDataIdentity.DataIdentity,
						Value1 = dataIdentityValue,
						DataIdentity2 = TfConstants.TF_ROW_ID_DATA_IDENTITY,
						Value2 = threadIdentityRowId
                    });
				}
			}

			scope.Complete();

			return id;
		}
	}

	public Guid CreateSubThread(
		CreateTalkSubThread thread)
	{
		if (thread == null)
			throw new NullReferenceException("Thread object is null");

		Guid id = Guid.NewGuid();

		var parentThread = GetThread(thread.ThreadId);


		new TalkThreadValidator(this)
			.ValidateCreateSubThread(
				thread,
				parentThread,
				id)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = @"INSERT INTO talk_thread
						(id, channel_id, thread_id, type, content, user_id,
						created_on, last_updated_on, deleted_on, visible_in_channel)
					VALUES(@id, @channel_id, @thread_id, @type, @content, @user_id,
						@created_on, @last_updated_on, @deleted_on,@visible_in_channel); ";

		var idPar = TalkUtility.CreateParameter(
			"@id",
			id,
			DbType.Guid);

		var channelIdPar = TalkUtility.CreateParameter(
			"@channel_id",
			parentThread.ChannelId,
			DbType.Guid);

		var threadIdPar = TalkUtility.CreateParameter(
			"@thread_id",
			thread.ThreadId,
			DbType.Guid);

		var typePar = TalkUtility.CreateParameter(
			"@type",
			(short)parentThread.Type,
			DbType.Int16);

		var contentPar = TalkUtility.CreateParameter(
			"@content",
			thread.Content,
			DbType.String);

		var visibleInChannelPar = TalkUtility.CreateParameter(
			"@visible_in_channel",
			thread.VisibleInChannel,
			DbType.Boolean);

		var userIdPar = TalkUtility.CreateParameter(
			"@user_id",
			thread.UserId,
			DbType.Guid);

		var createdOnPar = TalkUtility.CreateParameter(
			"@created_on",
			DateTime.Now,
			DbType.DateTime2);

		var lastUpdatedOnPar = TalkUtility.CreateParameter(
			"@last_updated_on",
			null,
			DbType.DateTime2);

		var deletedOnPar = TalkUtility.CreateParameter(
			"@deleted_on",
			null,
			DbType.DateTime2);

		using (var scope = _dbService.CreateTransactionScope())
		{
			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar, channelIdPar, threadIdPar,
				typePar, contentPar, userIdPar,
				createdOnPar, lastUpdatedOnPar,
				deletedOnPar, visibleInChannelPar);

			if (dbResult != 1)
				throw new Exception("Failed to insert new row in database for sub thread object");

			scope.Complete();

			return id;
		}
	}

	public void UpdateThread(
		Guid threadId,
		string content)
	{
		var existingThread = GetThread(threadId);

		new TalkThreadValidator(this)
			.ValidateUpdate(existingThread, content)
			.ToValidationException()
			.ThrowIfContainsErrors();

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
			DbType.DateTime2);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			idPar,
			contentPar,
			lastUpdatedPar);

		if (dbResult != 1)
			throw new Exception("Failed to update row in database for thread object");
	}

	public void DeleteThread(
		Guid threadId)
	{
		var existingThread = GetThread(threadId);

		new TalkThreadValidator(this)
			.ValidateDelete(existingThread)
			.ToValidationException()
			.ThrowIfContainsErrors();

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
			DbType.DateTime2);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			idPar,
			deletedOnPar);

		if (dbResult != 1)
			throw new Exception("Failed to update row in database for thread object");
	}

	private List<TalkThread> ToThreadList(DataTable dt)
	{
		if (dt == null)
			throw new Exception("DataTable is null");

		var channels = GetChannels();
		List<TalkThread> threadList = new List<TalkThread>();

		foreach (DataRow dr in dt.Rows)
		{
			var user = _tfService.GetUser(dr.Field<Guid>("user_id"));

			TalkThread thread = new TalkThread
			{
				Id = dr.Field<Guid>("id"),
				ChannelId = dr.Field<Guid>("channel_id"),
				ThreadId = dr.Field<Guid?>("thread_id"),
				Content = dr.Field<string>("content"),
				Type = (TalkThreadType)dr.Field<short>("type"),
				VisibleInChannel = dr.Field<bool>("visible_in_channel"),
				User = user,
				CreatedOn = dr.Field<DateTime>("created_on"),
				LastUpdatedOn = dr.Field<DateTime?>("last_updated_on"),
				DeletedOn = dr.Field<DateTime?>("deleted_on"),
				SubThread = new List<TalkThread>(),
                ConnectedDataIdentityValues = new Dictionary<string,string>()
            };

			TalkChannel channel = channels.SingleOrDefault(x => x.Id == thread.ChannelId);
			string channelDataIdentity = channel.DataIdentity;

			if( string.IsNullOrWhiteSpace(channelDataIdentity))
                channelDataIdentity = TfConstants.TF_ROW_ID_DATA_IDENTITY;

            var dataIdentityConnectionsJson = dr.Field<string>("identity_connection_json");
            if (!String.IsNullOrWhiteSpace(dataIdentityConnectionsJson) &&
                dataIdentityConnectionsJson.StartsWith("[") &&
                dataIdentityConnectionsJson != "[null]")
            {
                var dataIdentityConnections = 
					JsonSerializer.Deserialize<List<TfDataIdentityConnection>>(dataIdentityConnectionsJson);

				foreach(var dic in dataIdentityConnections)
				{
					if(dic.Value1 == thread.IdentityRowId)
					{
						if(!thread.ConnectedDataIdentityValues.Keys.Contains(dic.Value2))
                            thread.ConnectedDataIdentityValues[dic.Value2] = channelDataIdentity;
                    }
                    if (dic.Value2 == thread.IdentityRowId)
                    {
                        if (!thread.ConnectedDataIdentityValues.Keys.Contains(dic.Value1))
                            thread.ConnectedDataIdentityValues[dic.Value1] = channelDataIdentity;
                    }
                }
            }

			threadList.Add(thread);
        }

		//fill sub thread
		foreach (var thread in threadList.OrderBy(x => x.CreatedOn))
		{
			if (thread.ThreadId is not null)
			{
				var parentThread = threadList.Single(x => x.Id == thread.ThreadId);
				thread.ParentThread = parentThread;
				parentThread.SubThread.Add(thread);
			}
		}

		return threadList.Where(x => x.VisibleInChannel).ToList();
	}

	#region <--- validation --->

	internal class TalkThreadValidator
		: AbstractValidator<TalkThread>
	{
		public TalkThreadValidator(ITalkService service)
		{
		}

		public ValidationResult ValidateCreate(
			CreateTalkThread thread,
			Guid id)
		{
			if (thread == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The channel object is null.") });
			}

			if (string.IsNullOrWhiteSpace(thread.Content))
			{
				return new ValidationResult(new[] { new ValidationFailure(
					nameof(CreateTalkThread.Content),
					"The content is empty.") });
			}

            return new ValidationResult();
		}

		//public ValidationResult ValidateCreate(
		//	CreateTalkThreadWithDataIdentity thread,
		//	Guid id)
		//{
		//	if (thread == null)
		//	{
		//		return new ValidationResult(new[] { new ValidationFailure("",
		//			"The channel object is null.") });
		//	}

		//	if (string.IsNullOrWhiteSpace(thread.Content))
		//	{
		//		return new ValidationResult(new[] { new ValidationFailure(
		//			nameof(CreateTalkThread.Content),
		//			"The content is empty.") });
		//	}

		//	return new ValidationResult();
		//}


		public ValidationResult ValidateCreateSubThread(
			CreateTalkSubThread thread,
			TalkThread parent,
			Guid id)
		{
			if (thread == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The channel object is null.") });
			}

			if (parent == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The parent thread object is not found.") });
			}

			if (parent.ThreadId is not null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The parent thread is sub thread. SubThread cannot be created with SubThread parent.") });
			}

			if (string.IsNullOrWhiteSpace(thread.Content))
			{
				return new ValidationResult(new[] { new ValidationFailure(
					nameof(CreateTalkThread.Content),
					"The content is empty.") });
			}

			return new ValidationResult();
		}

		public ValidationResult ValidateUpdate(
			TalkThread thread,
			string content)
		{
			if (thread == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The thread object is null.") });
			}

			if (string.IsNullOrWhiteSpace(content))
			{
				return new ValidationResult(new[] { new ValidationFailure(
					nameof(CreateTalkThread.Content),
					"The content is empty.") });
			}

			if (thread.DeletedOn.HasValue)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The thread is already deleted") });
			}


			return new ValidationResult();

		}

		public ValidationResult ValidateDelete(
			TalkThread thread)
		{
			if (thread == null)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The thread object is null.") });
			}

			if (thread.DeletedOn.HasValue)
			{
				return new ValidationResult(new[] { new ValidationFailure("",
					"The thread is already deleted") });
			}


			return this.Validate(thread, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
