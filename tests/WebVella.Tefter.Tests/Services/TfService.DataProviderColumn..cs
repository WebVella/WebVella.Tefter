using WebVella.Tefter.Models;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{
	[Fact]
	public async Task DataProviderColumn_DbName_Invalid()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_textOwa colona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true
				};

				//name format invalid
				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();


				//empty name
				column.DbName = string.Empty;
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//start with tf_
				column.DbName = "tf_test";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//start with wrong prefix -it should start with dp{index}, where index is data provider index
				column.DbName = "prefix_test";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//too short
				column.DbName = $"dp{provider.Index}_a";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();

				//too long
				column.DbName = "rtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrt" +
					"rtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrtrt";
				task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DbName)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_Id_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.Empty,
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true
				};

				//empty id, but internally we set new id
				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_DataProviderId_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = Guid.Empty,
					DbName = $"dp{provider.Index}_textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				//empty id
				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.DataProviderId)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_SourceName_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				//we allow this now
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_SourceType_Empty()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = null,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_textcolona",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source",
					SourceType = "",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				var task = Task.Run(() => { tfService.CreateDataProviderColumn(column); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().NotBeNull();
				exception.Should().BeOfType(typeof(TfValidationException));
				((TfValidationException)exception).Data.Keys.Count.Should().Be(1);
				((TfValidationException)exception).Data.Contains(nameof(TfDataProviderColumn.SourceType)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_Expressions()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = Guid.NewGuid().ToString(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_text",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_text_expression",
					DbType = TfDatabaseColumnType.Text,
					Expression = $" dp{provider.Index}_text ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = string.Empty,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_short_text",
					DbType = TfDatabaseColumnType.ShortText,
					SourceName = "source_column",
					SourceType = "SHORT_TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_short_text_expression",
					DbType = TfDatabaseColumnType.ShortText,
					Expression = $" dp{provider.Index}_short_text ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = short.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_short_int",
					DbType = TfDatabaseColumnType.ShortInteger,
					SourceName = "source_column",
					SourceType = "SHORT_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_short_int_expression",
					DbType = TfDatabaseColumnType.ShortInteger,
					Expression = $" dp{provider.Index}_short_int ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = int.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_int",
					DbType = TfDatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_int_expression",
					DbType = TfDatabaseColumnType.Integer,
					Expression = $" dp{provider.Index}_int ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = long.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_long_int",
					DbType = TfDatabaseColumnType.LongInteger,
					SourceName = "source_column",
					SourceType = "LONG_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_long_int_expression",
					DbType = TfDatabaseColumnType.LongInteger,
					Expression = $" dp{provider.Index}_long_int ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = decimal.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_number",
					DbType = TfDatabaseColumnType.Number,
					SourceName = "source_column",
					SourceType = "NUMBER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_number_expression",
					DbType = TfDatabaseColumnType.Number,
					Expression = $" dp{provider.Index}_number ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27",
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_date",
					DbType = TfDatabaseColumnType.DateOnly,
					SourceName = "source_column",
					SourceType = "DATE",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_date_expression",
					DbType = TfDatabaseColumnType.DateOnly,
					Expression = $" dp{provider.Index}_date ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27 12:01",
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_datetime",
					DbType = TfDatabaseColumnType.DateTime,
					SourceName = "source_column",
					SourceType = "DATETIME",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_datetime_expression",
					DbType = TfDatabaseColumnType.DateTime,
					Expression = $" dp{provider.Index}_datetime ",
					ExpressionJson = "{}"
				};

				provider = tfService.CreateDataProviderColumn(column);
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_DefaultValue()
	{
		using (await locker.LockAsync())
		{
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

			using (var scope = dbService.CreateTransactionScope())
			{
				var provider = CreateProviderInternal(tfService, tfMetaService);

				TfDataProviderColumn column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = Guid.NewGuid().ToString(),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_text",
					DbType = TfDatabaseColumnType.Text,
					SourceName = "source_column",
					SourceType = "TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = string.Empty,
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_short_text",
					DbType = TfDatabaseColumnType.ShortText,
					SourceName = "source_column",
					SourceType = "SHORT_TEXT",
					IncludeInTableSearch = true,
					IsNullable = false,
					IsSearchable = true,
					IsSortable = true,
					IsUnique = true,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = short.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_short_int",
					DbType = TfDatabaseColumnType.ShortInteger,
					SourceName = "source_column",
					SourceType = "SHORT_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = int.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_int",
					DbType = TfDatabaseColumnType.Integer,
					SourceName = "source_column",
					SourceType = "INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = long.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_long_int",
					DbType = TfDatabaseColumnType.LongInteger,
					SourceName = "source_column",
					SourceType = "LONG_INTEGER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = true,
					DefaultValue = decimal.MaxValue.ToString(CultureInfo.InvariantCulture),
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_number",
					DbType = TfDatabaseColumnType.Number,
					SourceName = "source_column",
					SourceType = "NUMBER",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27",
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_date",
					DbType = TfDatabaseColumnType.DateOnly,
					SourceName = "source_column",
					SourceType = "DATE",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);

				column = new TfDataProviderColumn
				{
					Id = Guid.NewGuid(),
					AutoDefaultValue = false,
					DefaultValue = "2024-06-27 12:01",
					DataProviderId = provider.Id,
					DbName = $"dp{provider.Index}_datetime",
					DbType = TfDatabaseColumnType.DateTime,
					SourceName = "source_column",
					SourceType = "DATETIME",
					IncludeInTableSearch = true,
					IsNullable = true,
					IsSearchable = false,
					IsSortable = false,
					IsUnique = false,
				};

				provider = tfService.CreateDataProviderColumn(column);
			}
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_TEXT()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Text, "TEXT", "test def value");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_SHORT_TEXT()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.ShortText, "SHORT_TEXT", "test def value");
		}
	}


	[Fact]
	public async Task DataProviderColumn_CreateUpdate_NUMBER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Number, "NUMBER", "123.456");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_SHORT_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.ShortInteger, "SHORT_INTEGER", "1");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Integer, "INTEGER", "1");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_LONG_INTEGER()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.LongInteger, "LONG_INTEGER", "1");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_DATE()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.DateOnly, "DATE", "1975-10-25");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_DATETIME()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.DateTime, "DATETIME", "1975-10-25 20:25:45.123");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_BOOLEAN()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Boolean, "BOOLEAN", "true");
		}
	}

	[Fact]
	public async Task DataProviderColumn_CreateUpdate_GUID()
	{
		using (await locker.LockAsync())
		{
			CreateAndUpdateColumnType(TfDatabaseColumnType.Guid, "GUID", Guid.NewGuid().ToString());
		}
	}


	private void CreateAndUpdateColumnType(
		TfDatabaseColumnType type,
		string sourceType,
		string defaultValue)
	{
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		ITfMetaService tfMetaService = ServiceProvider.GetService<ITfMetaService>();
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();

		using (var scope = dbService.CreateTransactionScope())
		{
			var provider = CreateProviderInternal(tfService, tfMetaService);

			TfDataProviderColumn column = new TfDataProviderColumn
			{
				Id = Guid.NewGuid(),
				AutoDefaultValue = false,
				DefaultValue = null,
				DataProviderId = provider.Id,
				DbName = $"dp{provider.Index}_db_column",
				DbType = type,
				SourceName = "source_column",
				SourceType = sourceType,
				IncludeInTableSearch = false,
				IsNullable = true,
				IsSearchable = true,
				IsSortable = true,
				IsUnique = true,
			};

			provider = tfService.CreateDataProviderColumn(column);

			var exColumn = provider.Columns.FirstOrDefault();

			exColumn.Should().NotBeNull();
			exColumn.AutoDefaultValue.Should().Be(column.AutoDefaultValue);
			exColumn.DataProviderId.Should().Be(column.DataProviderId);
			exColumn.DbName.Should().Be(column.DbName);
			exColumn.DbType.Should().Be(column.DbType);
			exColumn.SourceName.Should().Be(column.SourceName);
			exColumn.SourceType.Should().Be(column.SourceType);
			exColumn.IncludeInTableSearch.Should().Be(column.IncludeInTableSearch);
			exColumn.IsNullable.Should().Be(column.IsNullable);
			exColumn.IsSearchable.Should().Be(column.IsSearchable);
			exColumn.IsSortable.Should().Be(column.IsSortable);
			exColumn.IsUnique.Should().Be(column.IsUnique);
			exColumn.DefaultValue.Should().Be(column.DefaultValue);

			column.DefaultValue = defaultValue;
			column.SourceName = "source_column_updated";
			column.IncludeInTableSearch = !column.IncludeInTableSearch;
			column.AutoDefaultValue = !column.AutoDefaultValue;
			column.IsNullable = !column.IsNullable;
			column.IsSearchable = !column.IsSearchable;
			column.IsSortable = !column.IsSortable;
			column.IsUnique = !column.IsUnique;

			provider = tfService.UpdateDataProviderColumn(column);

			exColumn = provider.Columns.FirstOrDefault();
			exColumn.AutoDefaultValue.Should().Be(column.AutoDefaultValue);
			exColumn.DataProviderId.Should().Be(column.DataProviderId);
			exColumn.DbName.Should().Be(column.DbName);
			exColumn.DbType.Should().Be(column.DbType);
			exColumn.SourceName.Should().Be(column.SourceName);
			exColumn.SourceType.Should().Be(column.SourceType);
			exColumn.IncludeInTableSearch.Should().Be(column.IncludeInTableSearch);
			exColumn.IsNullable.Should().Be(column.IsNullable);
			exColumn.IsSearchable.Should().Be(column.IsSearchable);
			exColumn.IsSortable.Should().Be(column.IsSortable);
			exColumn.IsUnique.Should().Be(column.IsUnique);
			exColumn.DefaultValue.Should().Be(column.DefaultValue);
		}
	}
}
