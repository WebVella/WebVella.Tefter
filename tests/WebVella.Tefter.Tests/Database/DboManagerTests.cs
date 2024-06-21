namespace WebVella.Tefter.Tests.Database;

public partial class DboManagerTests : BaseTest
{
	[Fact]
	public async Task FullTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())
			{
				CreateTestTableInternal();

				Guid id = Guid.NewGuid();

				var success = await dboManager.InsertAsync<TestModel>(new TestModel
				{
					Id = id,
					Guid = Guid.NewGuid(),
					GuidNull = null,
					Date = DateTime.Today,
					DateNull = null,
					DateTime = DateTime.Now,
					DateTimeNull = null,
					Checkbox = true,
					CheckboxNull = null,
					Number = 10,
					NumberNull = null,
					Text = "text",
					JsonObj = "{}",
					JsonList = "[]"
				});
				success.Should().BeTrue();

				var idKey = new Dictionary<string, Guid> { { nameof(TestModel.Id), id } };
				var obj = await dboManager.GetAsync<TestModel>(idKey);
				obj.Should().NotBeNull();
			}
		}
	}

	[Fact]
	public async Task NumberTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())
			{
				CreateTestTableInternal();

				var original = new NumberTestModel
				{
					Id = Guid.NewGuid(),
					Number = 10,
					NumberNull = null,
				};

				var success = await dboManager.InsertAsync<NumberTestModel>(original);
				success.Should().BeTrue();

				var idKey = new Dictionary<string, Guid> { { nameof(NumberTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<NumberTestModel>(idKey);
				obj?.Should().NotBeNull();

				obj.Number.Should().Be(original.Number);
				obj.NumberNull.Should().Be(original.NumberNull);
				obj.NumberNull.Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task LongTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())
			{
				CreateTestTableInternal();
				var original = new LongNumberTestModel
				{
					Id = Guid.NewGuid(),
					Number = 98765432100011001,
					NumberNull = null,
				};

				var success = await dboManager.InsertAsync<LongNumberTestModel>(original);
				success.Should().BeTrue();

				var idKey = new Dictionary<string, Guid> { { nameof(NumberTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<LongNumberTestModel>(idKey);
				obj.Should().NotBeNull();

				obj.Number.Should().Be(original.Number);
				obj.NumberNull.Should().Be(original.NumberNull);
				obj.NumberNull.Should().BeNull();
			}
		}
	}

	[Fact]
	public async Task DateTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())

			{
				CreateTestTableInternal();

				var original = new DateTestModel
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now.Date,
					DateNull = null
				};

				var success = await dboManager.InsertAsync<DateTestModel>(original);
				success.Should().BeTrue();

				var obj = await dboManager.GetAsync<DateTestModel>(original.Id);
				obj.Should().NotBeNull();

				obj.Date.Should().Be(original.Date);
				obj.DateNull.Should().Be(original.DateNull);
				obj.DateNull.Should().BeNull();
				obj.Date.Kind.Should().Be(DateTimeKind.Local);
			}
		}
	}

	[Fact]
	public async Task DateTimeTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())

			{
				CreateTestTableInternal();

				var original = new DateTimeTestModel
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now,
					DateNull = null
				};

				var success = await dboManager.InsertAsync<DateTimeTestModel>(original);
				success.Should().BeTrue();

				var idKey = new Dictionary<string, Guid> { { nameof(DateTimeTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<DateTimeTestModel>(idKey);
				obj.Should().NotBeNull();

				obj.Date.Should().Be(original.Date);
				obj.DateNull.Should().Be(original.DateNull);
				obj.DateNull.Should().BeNull();
				obj.Date.Kind.Should().Be(DateTimeKind.Local);
			}
		}
	}

	[Fact]
	public async Task JsonTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())

			{
				CreateTestTableInternal();

				var original = new JsonTestModel
				{
					Id = Guid.NewGuid(),
					JsonObj = new JsonObject { Name = "json name", Date = DateTime.Now },
					JsonList = new List<JsonObject> {
					new JsonObject { Name = "json1", Date = DateTime.Now },
					new JsonObject { Name = "json2", Date = DateTime.Now }
				}
				};

				var success = await dboManager.InsertAsync<JsonTestModel>(original);
				success.Should().BeTrue();

				var idKey = new Dictionary<string, Guid> { { nameof(DateTimeTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<JsonTestModel>(idKey);
				obj.Should().NotBeNull();

				obj.JsonObj.Should().Be(original.JsonObj);
				obj.JsonList.Count.Should().Be(original.JsonList.Count);
			}
		}
	}

	[Fact]
	public async Task EnumTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())
			{
				CreateTestTableInternal();

				var original = new EnumTestModel
				{
					Id = Guid.NewGuid(),
					Sample = SampleEnumType.EnumValue3,
					SampleNull = null
				};

				var success = await dboManager.InsertAsync<EnumTestModel>(original);
				success.Should().BeTrue();

				var idKey = new Dictionary<string, Guid> { { nameof(EnumTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<EnumTestModel>(idKey);
				obj.Should().NotBeNull();

				obj.Sample.Should().Be( original.Sample);
				obj.SampleNull.Should().BeNull();
				original.SampleNull.Should().BeNull();


				original = new EnumTestModel
				{
					Id = Guid.NewGuid(),
					Sample = SampleEnumType.EnumValue3,
					SampleNull = SampleEnumType.EnumValue1
				};

				success = await dboManager.InsertAsync<EnumTestModel>(original);
				success.Should().BeTrue();

				idKey = new Dictionary<string, Guid> { { nameof(DateTimeTestModel.Id), original.Id } };
				obj = await dboManager.GetAsync<EnumTestModel>(idKey);
				obj.Should().NotBeNull();
				obj.Sample.Should().Be( original.Sample);
			}
		}
	}

	#region <=== UTILITY METHODS ===>

	protected void CreateTestTableInternal()
	{
		ExecuteSqlQueryCommand(
			$@"
DROP TABLE IF EXISTS public.test_table;

CREATE TABLE IF NOT EXISTS public.test_table
(
    id uuid NOT NULL DEFAULT uuid_generate_v1(),
    checkbox boolean NOT NULL DEFAULT true,
    date date NOT NULL DEFAULT now(),
    date_null date,
    checkbox_null boolean DEFAULT false,
    datetime_null timestamp with time zone,
    datetime timestamp with time zone NOT NULL DEFAULT now(),
    number_null numeric,
    ""number"" numeric NOT NULL DEFAULT 0,
    guid_null uuid,
    guid uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'::uuid,
    text text COLLATE pg_catalog.""default"" NOT NULL DEFAULT ''::text,
    json_obj text COLLATE pg_catalog.""default"" NOT NULL DEFAULT ''::text,
    json_list text COLLATE pg_catalog.""default"" NOT NULL DEFAULT '[]'::text,
    CONSTRAINT rec_test_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.test_table
  OWNER to dev;"
		);
	}

	protected void DropTestTableInternal()
	{
		ExecuteSqlQueryCommand("DROP TABLE IF EXISTS public.test_table;");
	}

	#endregion

}
