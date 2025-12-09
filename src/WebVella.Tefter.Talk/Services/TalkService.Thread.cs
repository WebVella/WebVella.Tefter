using DocumentFormat.OpenXml.Office2010.Excel;
using System.Text;
using System.Threading;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
    event Func<TalkThread, Task> ThreadCreated;
    event Func<TalkThread, Task> ThreadUpdated;
    event Func<TalkThread, Task> ThreadDeleted;
    public TalkThread? GetThread(
        Guid id);

    public List<TalkThread> GetThreads(
        Guid channelId,
        string dataIdentityValue = null);

    public TalkThread? CreateThread(
        CreateTalkThreadWithRowIdModel thread);

    public TalkThread? CreateThread(
        CreateTalkThreadWithDataIdentityModel thread);

    public TalkThread? CreateSubThread(
        CreateTalkSubThread thread);

    public void UpdateThread(
        Guid threadid,
        string content);

    public void DeleteThread(
        Guid threadId);

    public List<string> GetThreadRelatedIdentityValues(
        TalkThread thread);
}

internal partial class TalkService : ITalkService
{
    #region << Events >>
    public event Func<TalkThread, Task> ThreadCreated = null!;
    public event Func<TalkThread, Task> ThreadUpdated = null!;
    public event Func<TalkThread, Task> ThreadDeleted = null!;
    #endregion

    public TalkThread? GetThread(
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
        return threads.SingleOrDefault(x => x.Id == id);

        //var threadsAsFlatList = new List<TalkThread>();

        //foreach (var thread in threads)
        //{
        //	threadsAsFlatList.Add(thread);
        //	foreach (var subThread in thread.SubThread)
        //		threadsAsFlatList.Add(subThread);
        //}

        //return threadsAsFlatList.SingleOrDefault(x => x.Id == id);
    }

    public List<TalkThread> GetThreads(
        Guid channelId,
        string dataIdentityValue = null)
    {


        var channel = GetChannel(channelId);

        if (channel is null)
            throw new Exception($"Failed to find channel with id '{channelId}'");

        if (string.IsNullOrWhiteSpace(channel.DataIdentity))
            throw new Exception($"Channel identity is not specified");

        var dataIdentity = _tfService.GetDataIdentity(channel.DataIdentity);
        if (dataIdentity is null)
            throw new Exception($"Failed to find data identity '{channel.DataIdentity}' for channel");

        var channelDataIdentity = dataIdentity.DataIdentity;

        string SQL_WITHOUT_DATA_IDENTITY =
$@"
WITH sk_identity_info AS (
	SELECT trs.id, count( dic.* ) AS count
	FROM talk_thread trs
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trs.identity_row_id AND dic.data_identity_2 IS NULL AND dic.data_identity_1 = '{channelDataIdentity}' ) OR
			( dic.value_1 = trs.identity_row_id AND dic.data_identity_1 IS NULL AND dic.data_identity_2 = '{channelDataIdentity}' ) 
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
	sk.count AS data_identity_values_count
FROM talk_thread tt
	LEFT OUTER JOIN root_threads  rt ON rt.id = tt.id OR tt.thread_id = rt.id
	LEFT OUTER JOIN sk_identity_info sk ON tt.id = sk.id
WHERE rt.id IS NOT NULL
ORDER BY tt.created_on DESC
";

        string SQL_WITH_DATA_IDENTITY =
$@"
WITH sk_identity_info AS (
	SELECT trs.id, count( dic.* ) AS count
	FROM talk_thread trs
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			( dic.value_2 = trs.identity_row_id AND dic.data_identity_2 IS NULL AND dic.data_identity_1 = '{channelDataIdentity}' ) OR
			( dic.value_1 = trs.identity_row_id AND dic.data_identity_1 IS NULL AND dic.data_identity_2 = '{channelDataIdentity}' ) 
	GROUP BY trs.id
),
root_threads AS (
	SELECT id 
	FROM talk_thread tt
		LEFT OUTER JOIN tf_data_identity_connection dic ON 
			  ( dic.value_2 = tt.identity_row_id AND dic.data_identity_2 IS NULL AND 
				dic.data_identity_1 = '{channelDataIdentity}' AND dic.value_1 = @identity_value ) 
			OR
			  ( dic.value_1 = tt.identity_row_id AND dic.data_identity_1 IS NULL AND 
				dic.data_identity_2 = '{channelDataIdentity}' AND dic.value_2 = @identity_value ) 
	WHERE channel_id = @channel_id AND thread_id IS NULL AND 
		( ( dic.data_identity_1 IS NULL AND dic.data_identity_2 = '{channelDataIdentity}' ) OR
		  ( dic.data_identity_2 IS NULL AND dic.data_identity_1 = '{channelDataIdentity}' ) )
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
	sk.count AS data_identity_values_count
FROM talk_thread tt
	LEFT OUTER JOIN root_threads  rt ON rt.id = tt.id OR tt.thread_id = rt.id
	LEFT OUTER JOIN sk_identity_info sk ON tt.id = sk.id
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

    public TalkThread? CreateThread(
        CreateTalkThreadWithRowIdModel thread)
    {
        if (thread == null)
            throw new NullReferenceException("Thread object is null");

        Guid id = Guid.NewGuid();

        new TalkThreadValidator(this, _tfService)
            .ValidateCreate(thread, id)
            .ToValidationException()
            .ThrowIfContainsErrors();

        var dataProvider = _tfService.GetDataProvider(thread.DataProviderId);

        var channel = GetChannel(thread.ChannelId);

        if (channel is null)
            throw new Exception($"Failed to find channel with id '{thread.ChannelId}' for thread");

        if (string.IsNullOrWhiteSpace(channel.DataIdentity))
            throw new Exception($"Channel data identity is not specified");

        var channelDataIdentity = _tfService.GetDataIdentity(channel.DataIdentity);

        if (channelDataIdentity is null)
            throw new Exception($"Failed to find data identity '{channel.DataIdentity}' for channel");


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

        TalkThread? createdThread = null;
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

            if (thread.RowIds != null && thread.RowIds.Count > 0)
            {
                var threadIdentityRowId = id.ToSha1();

                var rowIdentityValuesDict = _tfService.GetDataIdentityValuesForRowIds(dataProvider, channelDataIdentity, thread.RowIds);

                HashSet<string> dataIdentityValues = new HashSet<string>();
                foreach (var rowId in thread.RowIds)
                {
                    if (rowIdentityValuesDict.ContainsKey(rowId))
                    {
                        var identityValue = rowIdentityValuesDict[rowId];
                        if (!dataIdentityValues.Contains(identityValue))
                            dataIdentityValues.Add(identityValue);
                    }
                }

                List<TfDataIdentityConnection> connectionsToCreate = new List<TfDataIdentityConnection>();

                foreach (var dataIdentityValue in dataIdentityValues)
                {
                    connectionsToCreate.Add(new TfDataIdentityConnection
                    {
                        DataIdentity1 = channelDataIdentity.DataIdentity,
                        Value1 = dataIdentityValue,
                        DataIdentity2 = null,
                        Value2 = threadIdentityRowId
                    });
                }

                if (connectionsToCreate.Count > 0)
                {
                    _tfService.CreateBatchDataIdentityConnections(connectionsToCreate);
                }
            }

            createdThread = GetThread(id);

            ModifyThreadSharedColumnCount(createdThread, isIncrement: true);

            MaintainThreadTags(createdThread, _dbService);
            FixNewTagsInThreadContent(createdThread, _dbService);
            createdThread = GetThread(id);
            scope.Complete();
        }
        ThreadCreated?.Invoke(createdThread);
        return createdThread;
    }

    public TalkThread? CreateThread(
        CreateTalkThreadWithDataIdentityModel thread)
    {
        if (thread == null)
            throw new NullReferenceException("Thread object is null");

        Guid id = Guid.NewGuid();

        new TalkThreadValidator(this, _tfService)
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

        TalkThread? createdThread = null;
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

                var channel = GetChannel(thread.ChannelId);
                if (channel is null)
                    throw new Exception($"Failed to find channel with id '{thread.ChannelId}' for thread");

                if (string.IsNullOrWhiteSpace(channel.DataIdentity))
                    throw new Exception($"Data identity is not specified for channel");

                var channelDataIdentity = _tfService.GetDataIdentity(channel.DataIdentity);

                if (channelDataIdentity is null)
                    throw new Exception($"Failed to find data identity '{channel.DataIdentity}' for channel");


                List<TfDataIdentityConnection> connectionsToCreate = new List<TfDataIdentityConnection>();

                foreach (var dataIdentityValue in thread.DataIdentityValues)
                {
                    if (!dataIdentityValue.IsSha1())
                        throw new Exception($"Data identity value '{dataIdentityValue}' is not a valid SHA1 value");

                    connectionsToCreate.Add(new TfDataIdentityConnection
                    {
                        DataIdentity1 = channelDataIdentity.DataIdentity,
                        Value1 = dataIdentityValue,
                        DataIdentity2 = null,
                        Value2 = threadIdentityRowId
                    });
                }

                if (connectionsToCreate.Count > 0)
                {
                    _tfService.CreateBatchDataIdentityConnections(connectionsToCreate);
                }
            }

            createdThread = GetThread(id);

            ModifyThreadSharedColumnCount(createdThread, isIncrement: true);

            MaintainThreadTags(createdThread, _dbService);
            FixNewTagsInThreadContent(createdThread, _dbService);
            createdThread = GetThread(id);
            scope.Complete();
        }
        ThreadCreated?.Invoke(createdThread);
        return createdThread;
    }

    public TalkThread? CreateSubThread(
        CreateTalkSubThread thread)
    {
        if (thread == null)
            throw new NullReferenceException("Thread object is null");

        Guid id = Guid.NewGuid();

        var parentThread = GetThread(thread.ThreadId);


        new TalkThreadValidator(this, _tfService)
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


        TalkThread? createdThread = null;
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

            createdThread = GetThread(id);
            MaintainThreadTags(createdThread, _dbService);
            FixNewTagsInThreadContent(createdThread, _dbService);
            createdThread = GetThread(id);
            scope.Complete();
        }
        ThreadCreated?.Invoke(createdThread);
        return createdThread;
    }

    public void UpdateThread(
        Guid threadId,
        string content)
    {
        var existingThread = GetThread(threadId);

        new TalkThreadValidator(this, _tfService)
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

        using (var scope = _dbService.CreateTransactionScope())
        {
            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
            SQL,
            idPar,
            contentPar,
            lastUpdatedPar);

            if (dbResult != 1)
                throw new Exception("Failed to update row in database for thread object");

            existingThread = GetThread(threadId);
            MaintainThreadTags(existingThread, _dbService);
            FixNewTagsInThreadContent(existingThread, _dbService);
            existingThread = GetThread(threadId);
            scope.Complete();
        }
        ThreadUpdated?.Invoke(existingThread);

    }

    public void DeleteThread(
        Guid threadId)
    {
        using (var scope = _dbService.CreateTransactionScope())
        {
            var existingThread = GetThread(threadId);

            new TalkThreadValidator(this, _tfService)
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

            ModifyThreadSharedColumnCount(existingThread, isIncrement: false);

            //Remove Tag connections
            foreach (var tag in GetThreadTags(threadId))
            {
                var talkThreadIdPar = TalkUtility.CreateParameter(
                                       "@talk_thread_id",
                                       threadId,
                                       DbType.Guid);
                var tagIdPar = TalkUtility.CreateParameter(
                           "@tag_id",
                           tag.Id,
                           DbType.Guid);
                var addSql = @"DELETE FROM talk_thread_tag
                        WHERE talk_thread_id = @talk_thread_id AND tag_id = @tag_id";
                var dbResultDel = _dbService.ExecuteSqlNonQueryCommand(
                    addSql,
                    talkThreadIdPar, tagIdPar);

                if (dbResultDel != 1)
                    throw new Exception("Failed to insert new row in database for thread object");

                _tfService.CheckRemoveOrphanTags(tag.Id);
            }

            scope.Complete();

            ThreadDeleted?.Invoke(existingThread);
        }
    }

    private List<TalkThread> ToThreadList(
        DataTable dt)
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
                ConnectedDataIdentityValuesCount = dr.Field<long>("data_identity_values_count"),
                SubThread = new List<TalkThread>()
            };

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

    private void FixNewTagsInThreadContent(TalkThread thread, ITfDatabaseService _dbService)
    {

        var content = thread.Content;
        var contentTags = thread.Content.GetUniqueTagsFromText();
        var index = -1;
        foreach (var tag in contentTags)
        {
            var matchString = $" #{tag}";
            index++;
            if (!content.Contains(matchString)) continue;

            var tagModel = _tfService.GetTag(tag);
            content = content.Replace(matchString, $" <span class=\"mention\" data-index=\"{index}\" data-denotation-char=\"#\" data-id=\"{tagModel.Id}\" data-value=\"{tagModel.Label}\">﻿<span contenteditable=\"false\">#{tagModel.Label}</span>﻿</span>");
        }



        var SQL = " UPDATE talk_thread SET " +
            " content = @content " +
            " WHERE id = @id ";

        var idPar = TalkUtility.CreateParameter(
            "id",
            thread.Id,
            DbType.Guid);

        var contentPar = TalkUtility.CreateParameter(
            "@content",
            content,
            DbType.String);

        var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                   SQL,
                   idPar,
                   contentPar);

        if (dbResult != 1)
            throw new Exception("Failed to update row in database for thread object");
    }

    #region <--- validation --->

    internal class TalkThreadValidator
        : AbstractValidator<TalkThread>
    {
        public readonly ITalkService _talkService;
        public readonly ITfService _tfService;

        public TalkThreadValidator(
            ITalkService talkService,
            ITfService tfService)
        {
            _talkService = talkService;
            _tfService = tfService;
        }

        public ValidationResult ValidateCreate(
            CreateTalkThreadWithDataIdentityModel thread,
            Guid id)
        {
            if (thread == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The thread object is null.") });
            }

            if (string.IsNullOrWhiteSpace(thread.Content))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateTalkThreadWithDataIdentityModel.Content),
                    "The content is empty.") });
            }

            var channel = _talkService.GetChannel(thread.ChannelId);
            if (channel == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateTalkThreadWithRowIdModel.ChannelId),
                    "The channel is not found.") });
            }

            return new ValidationResult();
        }

        public ValidationResult ValidateCreate(
            CreateTalkThreadWithRowIdModel thread,
            Guid id)
        {
            if (thread == null)
            {
                return new ValidationResult(new[] { new ValidationFailure("",
                    "The thread object is null.") });
            }

            if (string.IsNullOrWhiteSpace(thread.Content))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateTalkThreadWithDataIdentityModel.Content),
                    "The content is empty.") });
            }

            var dataProvider = _tfService.GetDataProvider(thread.DataProviderId);
            if (dataProvider == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateTalkThreadWithRowIdModel.DataProviderId),
                    "The data provider is not found.") });
            }

            var channel = _talkService.GetChannel(thread.ChannelId);
            if (channel == null)
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateTalkThreadWithRowIdModel.ChannelId),
                    "The channel is not found.") });
            }

            if (!dataProvider.Identities.Any(x => x.DataIdentity == channel.DataIdentity))
            {
                return new ValidationResult(new[] { new ValidationFailure(
                    nameof(CreateTalkThreadWithRowIdModel.ChannelId),
                    "The channel data identity is not associated with the data provider.") });
            }

            return new ValidationResult();
        }

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
                    nameof(CreateTalkThreadWithDataIdentityModel.Content),
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
                    nameof(CreateTalkThreadWithDataIdentityModel.Content),
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

    public void ModifyThreadSharedColumnCount(
      TalkThread thread,
      bool isIncrement)
    {
        if (thread == null)
            return;

        //only root threads are counted
        if (thread.ParentThread != null)
            return;

        var channel = GetChannel(thread.ChannelId);

        if (channel == null)
            return;

        if (string.IsNullOrWhiteSpace(channel.CountSharedColumnName))
            return;

        var sharedColumn = _tfService.GetSharedColumn(channel.CountSharedColumnName);

        if (sharedColumn is null)
            return;

        if (sharedColumn.DataIdentity != channel.DataIdentity)
            return;

        //only number type columns are supported
        if (!(sharedColumn.DbType == TfDatabaseColumnType.Number ||
                sharedColumn.DbType == TfDatabaseColumnType.ShortInteger ||
                sharedColumn.DbType == TfDatabaseColumnType.Integer ||
                sharedColumn.DbType == TfDatabaseColumnType.LongInteger
            ))
        {
            return;
        }

        var identityValues = GetThreadRelatedIdentityValues(thread);

        ModifySharedColumnValues(sharedColumn, identityValues, isIncrement ? 1 : -1, 1000);
    }

    private void ModifySharedColumnValues(
        TfSharedColumn sharedColumn,
        List<string> identityValues,
        int valueChange,
        int batchSize)
    {
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero");

        if (identityValues is null)
            throw new ArgumentNullException(nameof(identityValues));

        if (sharedColumn is null)
            throw new Exception("Shared column not found");

        string tableName;
        switch (sharedColumn.DbType)
        {
            case TfDatabaseColumnType.ShortInteger:
                tableName = "tf_shared_column_short_integer_value";
                break;
            case TfDatabaseColumnType.Integer:
                tableName = "tf_shared_column_integer_value";
                break;
            case TfDatabaseColumnType.LongInteger:
                tableName = "tf_shared_column_long_integer_value";
                break;
            case TfDatabaseColumnType.Number:
                tableName = "tf_shared_column_number_value";
                break;
            default:
                throw new Exception("Shared column db type is not supported column type for modification.");
        }

        using (var scope = _dbService.CreateTransactionScope())
        {
            foreach (IEnumerable<string> keysBatch in identityValues.Batch(batchSize))
            {
                int paramCounter = 1;

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                parameters.Add(new NpgsqlParameter($"@shared_column_id", sharedColumn.Id));

                StringBuilder sqlSb = new StringBuilder();

                foreach (var identityValue in keysBatch)
                {
                    sqlSb.AppendLine($@"INSERT INTO {tableName} (shared_column_id,data_identity_value, value)
                            VALUES (@shared_column_id,@data_identity_value_{paramCounter},{(valueChange >= 0 ? valueChange : 0)} )
                            ON CONFLICT (data_identity_value,shared_column_id)
                            DO UPDATE SET  value = GREATEST( 0, {tableName}.value + ( {valueChange} ) );");

                    parameters.Add(new NpgsqlParameter($"@data_identity_value_{paramCounter}", identityValue));

                    paramCounter++;
                }

                _dbService.ExecuteSqlNonQueryCommand(sqlSb.ToString(), parameters);
            }

            scope.Complete();
        }
    }

    public List<string> GetThreadRelatedIdentityValues(
        TalkThread thread)
    {
        if (thread == null)
            throw new ArgumentNullException(nameof(thread));

        var channel = GetChannel(thread.ChannelId);

        if (channel == null)
            throw new Exception($"Failed to find channel with id '{thread.ChannelId}'");

        List<string> identityValues = new List<string>();

        const string sql = @"SELECT * FROM tf_data_identity_connection WHERE ( value_1 = @value OR value_2 = @value)";

        var valuePar = TalkUtility.CreateParameter("@value", thread.IdentityRowId, DbType.String);

        var dt = _dbService.ExecuteSqlQueryCommand(sql, valuePar);

        foreach (DataRow dr in dt.Rows)
        {
            var dataIdentity1 = dr.Field<string>("data_identity_1");
            var value1 = dr.Field<string>("value_1");
            var dataIdentity2 = dr.Field<string>("data_identity_2");
            var value2 = dr.Field<string>("value_2");

            if (value1 == thread.IdentityRowId && dataIdentity1 is null && dataIdentity2 == channel.DataIdentity)
            {
                if (!identityValues.Contains(value2))
                    identityValues.Add(value2);
            }
            else if (value2 == thread.IdentityRowId && dataIdentity2 is null && dataIdentity1 == channel.DataIdentity)
            {
                if (!identityValues.Contains(value1))
                    identityValues.Add(value1);
            }
        }

        return identityValues;
    }

    private List<TfTag> GetThreadTags(
            Guid threadId)
    {
        using (var scope = _dbService.CreateTransactionScope())
        {
            var threadIdPar = TalkUtility.CreateParameter(
                "@thread_id",
                threadId,
                DbType.Guid);
            var sql = @"SELECT t.* FROM tf_tag t
				LEFT OUTER JOIN talk_thread_tag rel ON rel.tag_id = t.id
			WHERE rel.tag_id IS NOT NULL AND rel.talk_thread_id = @thread_id";

            var dt = _dbService.ExecuteSqlQueryCommand(sql, threadIdPar);
            var result = new List<TfTag>();
            foreach (DataRow dr in dt.Rows)
            {
                var tag = new TfTag()
                {
                    Id = dr.Field<Guid>("id"),
                    Label = dr.Field<string>("label") ?? String.Empty,
                };
                result.Add(tag);
            }
            return result;
        }
    }



    private void MaintainThreadTags(TalkThread thread, ITfDatabaseService _dbService)
    {
        var existingTags = GetThreadTags(thread.Id);
        var textTags = thread.Content.GetUniqueTagsFromText();

        List<string> tagsToAdd = textTags
            .Where(t => !existingTags.Any(x => x.Label == t))
            .ToList();

        List<Guid> tagIdsToRemove = existingTags
            .Where(x => !textTags.Contains(x.Label))
            .Select(x => x.Id)
            .ToList();

        //add new tags
        foreach (var textTag in tagsToAdd)
        {
            var existingTag = _tfService.GetTag(textTag);
            if (existingTag is null)
                existingTag = _tfService.CreateTag(new TfTag { Id = Guid.NewGuid(), Label = textTag });

            var talkThreadIdPar = TalkUtility.CreateParameter(
                       "@talk_thread_id",
                       thread.Id,
                       DbType.Guid);
            var tagIdPar = TalkUtility.CreateParameter(
                       "@tag_id",
                       existingTag.Id,
                       DbType.Guid);
            var addSql = @"INSERT INTO talk_thread_tag
                        (talk_thread_id,tag_id)
                        VALUES(@talk_thread_id,@tag_id)";
            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                addSql,
                talkThreadIdPar, tagIdPar);

            if (dbResult != 1)
                throw new Exception("Failed to insert new row in database for thread object");
        }

        //remove connection to missing tags
        foreach (Guid id in tagIdsToRemove)
        {
            var talkThreadIdPar = TalkUtility.CreateParameter(
                       "@talk_thread_id",
                       thread.Id,
                       DbType.Guid);
            var tagIdPar = TalkUtility.CreateParameter(
                       "@tag_id",
                       id,
                       DbType.Guid);
            var addSql = @"DELETE FROM talk_thread_tag
                        WHERE talk_thread_id = @talk_thread_id AND tag_id = @tag_id";
            var dbResult = _dbService.ExecuteSqlNonQueryCommand(
                addSql,
                talkThreadIdPar, tagIdPar);

            if (dbResult != 1)
                throw new Exception("Failed to insert new row in database for thread object");

            _tfService.CheckRemoveOrphanTags(id);
        }
    }
}
