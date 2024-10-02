namespace WebVella.Tefter.Talk.Services;

internal partial interface ITalkService
{
	Result<TalkChannel?> GetChannel(
		Guid channelId);

	Result<List<TalkChannel>> GetChannels();

	Result<TalkChannel> CreateChannel(
		TalkChannel channel);

	Result UpdateChannel(
		TalkChannel channel);

	Result DeleteChannel(
		Guid channelId);
}

internal partial class TalkService : ITalkService
{
	public Result<TalkChannel?> GetChannel(
		Guid channelId)
	{
		try
		{
			var SQL = "SELECT * FROM talk_channel WHERE id = @id";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL,
				new NpgsqlParameter("id", channelId));

			if (dt.Rows.Count == 0)
				return Result.Ok((TalkChannel?)null);

			return Result.Ok((TalkChannel?)ToChannel(dt.Rows[0]));
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new channel.").CausedBy(ex));
		}
	}

	public Result<List<TalkChannel>> GetChannels()
	{
		try
		{
			var SQL = "SELECT * FROM talk_channel";

			var dt = _dbService.ExecuteSqlQueryCommand(SQL);

			List<TalkChannel> channels = new List<TalkChannel>();

			foreach (DataRow row in dt.Rows)
				channels.Add(ToChannel(row));

			return Result.Ok(channels);
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new channel.").CausedBy(ex));
		}
	}

	public Result<TalkChannel> CreateChannel(
		TalkChannel channel)
	{
		try
		{
			if (channel == null)
				throw new NullReferenceException("Channel object is null");

			if (channel.Id == Guid.Empty)
				channel.Id = Guid.NewGuid();

			TalkChannelValidator validator = new TalkChannelValidator(this);

			var validationResult = validator.ValidateCreate(channel);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var SQL = "INSERT INTO talk_channel(id,name,shared_key,count_shared_column_name) " +
				"VALUES( @id,@name,@shared_key,@count_shared_column_name)";

			var idPar = TalkUtility.CreateParameter(
				"id",
				channel.Id,
				DbType.Guid);

			var namePar = TalkUtility.CreateParameter(
				"name",
				channel.Name,
				DbType.StringFixedLength);

			var sharedKeyPar = TalkUtility.CreateParameter(
				"shared_key",
				channel.SharedKey,
				DbType.StringFixedLength);

			var countSharedColumnNamePar = TalkUtility.CreateParameter(
				"count_shared_column_name",
				channel.SharedKey,
				DbType.StringFixedLength);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar,
				namePar,
				sharedKeyPar,
				countSharedColumnNamePar);

			if (dbResult != 1)
				throw new Exception("Failed to insert new row in database for channel object");

			var insertedChannelResult = GetChannel(channel.Id);

			if (!insertedChannelResult.IsSuccess || insertedChannelResult.Value is null)
				throw new Exception("Failed to get newly create channel from database");

			return Result.Ok(insertedChannelResult.Value);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create new channel.").CausedBy(ex));
		}
	}

	public Result UpdateChannel(
		TalkChannel channel)
	{
		try
		{
			if (channel == null)
				throw new NullReferenceException("Channel object is null");

			TalkChannelValidator validator = new TalkChannelValidator(this);

			var validationResult = validator.ValidateUpdate(channel);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var SQL = "UPDATE talk_channel SET " +
				"name=@name, " +
				"shared_key=@shared_key, " +
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

			var sharedKeyPar = TalkUtility.CreateParameter(
				"shared_key",
				channel.SharedKey,
				DbType.StringFixedLength);

			var countSharedColumnNamePar = TalkUtility.CreateParameter(
				"count_shared_column_name",
				channel.SharedKey,
				DbType.StringFixedLength);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(
				SQL,
				idPar,
				namePar,
				sharedKeyPar,
				countSharedColumnNamePar);

			if (dbResult != 1)
				throw new Exception("Failed to update row in database for channel object");

			return Result.Ok();

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update channel.").CausedBy(ex));
		}
	}

	public Result DeleteChannel(
		Guid channelId)
	{
		try
		{

			var existingChannel = GetChannel(channelId).Value;

			TalkChannelValidator validator = new TalkChannelValidator(this);

			var validationResult = validator.ValidateDelete(existingChannel);

			if (!validationResult.IsValid)
				return validationResult.ToResult();

			var SQL = "DELETE FROM talk_channel WHERE id = @id";

			var idPar = TalkUtility.CreateParameter(
				"id",
				channelId,
				DbType.Guid);

			var dbResult = _dbService.ExecuteSqlNonQueryCommand(SQL, idPar);

			if (dbResult != 1)
				throw new Exception("Failed to delete row in database for channel object");

			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete channel.").CausedBy(ex));
		}
	}

	private TalkChannel ToChannel(DataRow dr)
	{
		if (dr == null)
			throw new Exception("DataRow is null");

		return new TalkChannel
		{
			Id = dr.Field<Guid>("id"),
			Name = dr.Field<string>("name") ?? string.Empty,
			SharedKey = dr.Field<string?>("shared_key"),
			CountSharedColumnName = dr.Field<string?>("count_shared_column_name")
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
						.Must((channel, id) => { return service.GetChannel(id).Value == null; })
						.WithMessage("There is already existing channel with specified identifier.");

				RuleFor(channel => channel.Name)
						.Must((channel, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var channels = service.GetChannels().Value;
							return !channels.Any(x => x.Name.ToLowerInvariant().Trim() == name.ToLowerInvariant().Trim());
						})
						.WithMessage("There is already existing channel with same name.");
			});

			RuleSet("update", () =>
			{
				RuleFor(channnel => channnel.Id)
						.Must((channel, id) =>
						{
							return service.GetChannel(id).Value != null;
						})
						.WithMessage("There is not existing channel with specified identifier.");

				RuleFor(channel => channel.Name)
						.Must((channel, name) =>
						{
							if (string.IsNullOrEmpty(name))
								return true;

							var channels = service.GetChannels().Value;
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
						return service.GetChannel(id).Value != null;
					})
					.WithMessage("There is not existing channel with specified identifier.");
			});

		}

		public ValidationResult ValidateCreate(
			TalkChannel? channel)
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
			TalkChannel? channel)
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
			TalkChannel? channel)
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
