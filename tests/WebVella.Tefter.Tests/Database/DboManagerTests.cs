namespace WebVella.Tefter.Tests.Database;

public partial class DboManagerTests : BaseTest
{
	[Fact]
	public async void FullTestRecord()
	{
		using (await locker.LockAsync())
		{
			IDatabaseService dbService = ServiceProvider.GetRequiredService<IDatabaseService>();
			IDboManager dboManager = ServiceProvider.GetRequiredService<IDboManager>();

			using (DatabaseTransactionScope scope = dbService.CreateTransactionScope())
			{
				CreateTestTableInternal();

				Guid id = Guid.NewGuid();

				var succes = await dboManager.InsertAsync<TestModel>(new TestModel
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
				Assert.True(succes);

				var idKey = new Dictionary<string, Guid> { { nameof(TestModel.Id), id } };
				var obj = await dboManager.GetAsync<TestModel>(idKey);
				Assert.NotNull(obj);
			}
		}
	}

	[Fact]
	public async void NumberTestRecord()
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

				var succes = await dboManager.InsertAsync<NumberTestModel>(original);
				Assert.True(succes);

				var idKey = new Dictionary<string, Guid> { { nameof(NumberTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<NumberTestModel>(idKey);
				Assert.NotNull(obj);

				Assert.Equal(obj.Number, original.Number);
				Assert.Equal(obj.NumberNull, original.NumberNull);
			}
		}
	}

	[Fact]
	public async void LongTestRecord()
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

				var succes = await dboManager.InsertAsync<LongNumberTestModel>(original);
				Assert.True(succes);

				var idKey = new Dictionary<string, Guid> { { nameof(NumberTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<LongNumberTestModel>(idKey);
				Assert.NotNull(obj);

				Assert.Equal(obj.Number, original.Number);
				Assert.Equal(obj.NumberNull, original.NumberNull);
			}
		}
	}

	[Fact]
	public async void DateTimeTestRecord()
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

				var succes = await dboManager.InsertAsync<DateTimeTestModel>(original);
				Assert.True(succes);

				var idKey = new Dictionary<string, Guid> { { nameof(DateTimeTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<DateTimeTestModel>(idKey);
				Assert.NotNull(obj);

				Assert.Equal(obj.Date, original.Date.Date);
				Assert.Equal(obj.DateNull, original.DateNull);
				Assert.True(obj.Date.Kind == DateTimeKind.Local);
			}
		}
	}

	[Fact]
	public async void JsonTestRecord()
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

				var succes = await dboManager.InsertAsync<JsonTestModel>(original);
				Assert.True(succes);

				var idKey = new Dictionary<string, Guid> { { nameof(DateTimeTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<JsonTestModel>(idKey);
				Assert.NotNull(obj);

				Assert.Equal(obj.JsonObj, original.JsonObj);
				Assert.Equal(obj.JsonList.Count, original.JsonList.Count);
			}
		}
	}

	[Fact]
	public async void EnumTestRecord()
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
				Assert.True(success);

				var idKey = new Dictionary<string, Guid> { { nameof(EnumTestModel.Id), original.Id } };
				var obj = await dboManager.GetAsync<EnumTestModel>(idKey);
				Assert.NotNull(obj);

				Assert.Equal(obj.Sample, original.Sample);
				Assert.Null(obj.SampleNull);
				Assert.Equal(obj.SampleNull, original.SampleNull);


				original = new EnumTestModel
				{
					Id = Guid.NewGuid(),
					Sample = SampleEnumType.EnumValue3,
					SampleNull = SampleEnumType.EnumValue1
				};

				success = await dboManager.InsertAsync<EnumTestModel>(original);
				Assert.True(success);

				idKey = new Dictionary<string, Guid> { { nameof(DateTimeTestModel.Id), original.Id } };
				obj = await dboManager.GetAsync<EnumTestModel>(idKey);
				Assert.NotNull(obj);

				Assert.Equal(obj.Sample, original.Sample);
				Assert.NotNull(obj.SampleNull);
				Assert.Equal(obj.SampleNull, original.SampleNull);

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
