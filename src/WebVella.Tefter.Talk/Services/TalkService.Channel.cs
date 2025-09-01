using System.Runtime.InteropServices;

namespace WebVella.Tefter.Talk.Services;

public partial interface ITalkService
{
	event EventHandler<TalkChannel> ChannelCreated;
	event EventHandler<TalkChannel> ChannelUpdated;
	event EventHandler<TalkChannel> ChannelDeleted;

	TalkChannel GetChannel(
		Guid channelId);

	List<TalkChannel> GetChannels();

	TalkChannel CreateChannel(
		TalkChannel channel);

	TalkChannel UpdateChannel(
		TalkChannel channel);

	void DeleteChannel(
		Guid channelId);
}

internal partial class TalkService : ITalkService
{
	#region << Events >>
	public event EventHandler<TalkChannel> ChannelCreated = default!;
	public event EventHandler<TalkChannel> ChannelUpdated = default!;
	public event EventHandler<TalkChannel> ChannelDeleted = default!;
	#endregion

	public TalkChannel GetChannel(
		Guid channelId)
	{
		var SQL = "SELECT * FROM talk_channel WHERE id = @id";

		var dt = _dbService.ExecuteSqlQueryCommand(SQL,
			new NpgsqlParameter("id", channelId));

		if (dt.Rows.Count == 0)
			return null;

		return (TalkChannel)ToChannel(dt.Rows[0]);
	}

	public List<TalkChannel> GetChannels()
	{
		var SQL = "SELECT * FROM talk_channel";

		var dt = _dbService.ExecuteSqlQueryCommand(SQL);

		List<TalkChannel> channels = new List<TalkChannel>();

		foreach (DataRow row in dt.Rows)
			channels.Add(ToChannel(row));

		return channels;
	}

	public TalkChannel CreateChannel(
		TalkChannel channel)
	{
		if (channel == null)
			throw new NullReferenceException("Channel object is null");

		if (channel.Id == Guid.Empty)
			channel.Id = Guid.NewGuid();

		new TalkChannelValidator(this)
			.ValidateCreate(channel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = "INSERT INTO talk_channel(id,name,data_identity,count_shared_column_name) " +
            "VALUES( @id,@name,@data_identity,@count_shared_column_name)";

		var idPar = TalkUtility.CreateParameter(
			"id",
			channel.Id,
			DbType.Guid);

		var namePar = TalkUtility.CreateParameter(
			"name",
			channel.Name,
			DbType.StringFixedLength);

		var joinKeyPar = TalkUtility.CreateParameter(
            "data_identity",
			channel.DataIdentity,
			DbType.StringFixedLength);

		var countSharedColumnNamePar = TalkUtility.CreateParameter(
			"count_shared_column_name",
			channel.CountSharedColumnName,
			DbType.StringFixedLength);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			idPar,
			namePar,
			joinKeyPar,
			countSharedColumnNamePar);

		if (dbResult != 1)
			throw new Exception("Failed to insert new row in database for channel object");

		channel = GetChannel(channel.Id);
		ChannelCreated?.Invoke(this,channel);
		return channel;
	}

	public TalkChannel UpdateChannel(
		TalkChannel channel)
	{
		if (channel == null)
			throw new NullReferenceException("Channel object is null");

		new TalkChannelValidator(this)
			.ValidateUpdate(channel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = "UPDATE talk_channel SET " +
			"name=@name, " +
			"data_identity=@data_identity, " +
			"count_shared_column_name=@count_shared_column_name " +
			"WHERE id = @id";

		var idPar = TalkUtility.CreateParameter(
			"id",
			channel.Id,
			DbType.Guid);

		var namePar = TalkUtility.CreateParameter(
			"name",
			channel.Name,
			DbType.StringFixedLength);

		var dataIdentityPar = TalkUtility.CreateParameter(
			"data_identity",
			channel.DataIdentity,
			DbType.StringFixedLength);

		var countSharedColumnNamePar = TalkUtility.CreateParameter(
			"count_shared_column_name",
			channel.CountSharedColumnName,
			DbType.StringFixedLength);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(
			SQL,
			idPar,
			namePar,
			dataIdentityPar,
			countSharedColumnNamePar);

		if (dbResult != 1)
			throw new Exception("Failed to update row in database for channel object");

		channel = GetChannel(channel.Id);
		ChannelUpdated?.Invoke(this,channel);
		return channel;
	}

	public void DeleteChannel(
		Guid channelId)
	{
		var existingChannel = GetChannel(channelId);

		new TalkChannelValidator(this)
			.ValidateDelete(existingChannel)
			.ToValidationException()
			.ThrowIfContainsErrors();

		var SQL = "DELETE FROM talk_channel WHERE id = @id";

		var idPar = TalkUtility.CreateParameter(
			"id",
			channelId,
			DbType.Guid);

		var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

		if (dbResult != 1)
			throw new Exception("Failed to delete row in database for channel object");

		ChannelDeleted?.Invoke(this,existingChannel);
	}

	private TalkChannel ToChannel(DataRow dr)
	{
		if (dr == null)
			throw new Exception("DataRow is null");

		return new TalkChannel
		{
			Id = dr.Field<Guid>("id"),
			Name = dr.Field<string>("name") ?? string.Empty,
			DataIdentity = dr.Field<string>("data_identity"),
			CountSharedColumnName = dr.Field<string>("count_shared_column_name")
		};
	}

	#region <--- validation --->

	internal class TalkChannelValidator
		: AbstractValidator<TalkChannel>
	{
		public TalkChannelValidator(ITalkService service)
		{

			RuleSet("general", () =>
			{
				RuleFor(channel => channel.Id)
					.NotEmpty()
					.WithMessage("The channel id is required.");

				RuleFor(channel => channel.Name)
					.NotEmpty()
					.WithMessage("The channel name is required.");

			});

			RuleSet("create", () =>
			{
				RuleFor(channel => channel.Id)
						.Must((channel, id) => { return service.GetChannel(id) == null; })
						.WithMessage("There is already existing channel with specified identifier.");

				RuleFor(channel => channel.Name)
						.Must((channel, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var channels = service.GetChannels();
							return !channels.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing channel with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(channnel => channnel.Id)
						.Must((channel, id) =>
						{
							return service.GetChannel(id) != null;
						})
						.WithMessage("There is not existing channel with specified identifier.");

				RuleFor(channel => channel.Name)
						.Must((channel, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var channels = service.GetChannels();
							return !channels.Any(x =>
								x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim() &&
								x.Id != channel.Id
							);
						})
						.WithMessage("There is already existing another channel with same name.");

			});

			RuleSet("delete", () =>
			{
				RuleFor(channnel => channnel.Id)
					.Must((channel, id) =>
					{
						return service.GetChannel(id) != null;
					})
					.WithMessage("There is not existing channel with specified identifier.");
			});

		}

		public ValidationResult ValidateCreate(
			TalkChannel channel)
		{
			if (channel == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The channel object is null.") });

			return this.Validate(channel, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TalkChannel channel)
		{
			if (channel == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The channel object is null.") });

			return this.Validate(channel, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TalkChannel channel)
		{
			if (channel == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"A channel with specified identifier is not found.") });

			return this.Validate(channel, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}
	}

	#endregion
}
