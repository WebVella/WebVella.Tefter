using Bogus;

namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{

	[Fact]
	public async Task Data_Operations()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				var (provider, dataset) = await CreateTestStructureAndData(ServiceProvider, dbService);

				Data_InsertProviderRow(provider);

				Data_InsertDatasetRow(provider, dataset);

				Data_UpdateProviderRow(provider);

				Data_UpdateDatasetRow(provider, dataset);

				Data_InsertProviderRow_ValidateUniqueValues(provider);

				Data_InsertDatasetRow_ValidateUniqueValues(provider, dataset);
			}
		}
	}

	public void Data_InsertProviderRow(TfDataProvider provider)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var faker = new Faker("en");

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var newRow = new Dictionary<string, object?>();

			newRow[$"dp{provider.Index}_guid_column"] = Guid.NewGuid();
			newRow[$"dp{provider.Index}_short_text_column"] = faker.Lorem.Sentence();
			newRow[$"dp{provider.Index}_text_column"] = faker.Lorem.Lines();
			newRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
			newRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
			newRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
			newRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
			newRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
			newRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

			newRow["test_data_identity_1.sc_text"] = "this is shared columns text";
			newRow["test_data_identity_2.sc_int"] = 0;

			var result = tfService.InsertProviderRow(provider.Id, newRow);
		}
	}

	public void Data_InsertDatasetRow(TfDataProvider provider, TfDataset dataset)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var faker = new Faker("en");

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var newRow = new Dictionary<string, object?>();

			newRow[$"dp{provider.Index}_guid_column"] = Guid.NewGuid();
			newRow[$"dp{provider.Index}_short_text_column"] = faker.Lorem.Sentence();
			newRow[$"dp{provider.Index}_text_column"] = faker.Lorem.Lines();
			newRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
			newRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
			newRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
			newRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
			newRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
			newRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

			newRow["test_data_identity_1.sc_text"] = "this is shared columns text";
			newRow["test_data_identity_2.sc_int"] = 0;

			var result = tfService.InsertDatasetRow(dataset.Id, newRow);
		}
	}

	public void Data_UpdateProviderRow(TfDataProvider provider)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var faker = new Faker("en");

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var newRow = new Dictionary<string, object?>();

			newRow[$"dp{provider.Index}_guid_column"] = Guid.NewGuid();
			newRow[$"dp{provider.Index}_short_text_column"] = faker.Lorem.Sentence();
			newRow[$"dp{provider.Index}_text_column"] = faker.Lorem.Lines();
			newRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
			newRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
			newRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
			newRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
			newRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
			newRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

			newRow["test_data_identity_1.sc_text"] = "this is shared columns text";
			newRow["test_data_identity_2.sc_int"] = 0;

			var insertedRow = tfService.InsertProviderRow(provider.Id, newRow);

			Guid tfId = (Guid)insertedRow["tf_id"];
			var updateRow = new Dictionary<string, object?>();

			updateRow[$"dp{provider.Index}_short_text_column"] = faker.Lorem.Sentence() + " upd";
			updateRow[$"dp{provider.Index}_text_column"] = faker.Lorem.Lines() + " upd";
			updateRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
			updateRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
			updateRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
			updateRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
			updateRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
			updateRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

			updateRow["test_data_identity_1.sc_text"] = "this is shared columns text" + " upd";
			updateRow["test_data_identity_2.sc_int"] = 1;

			var updatedRow = tfService.UpdateProviderRow(tfId, provider.Id, updateRow);

			updateRow[$"dp{provider.Index}_short_text_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_short_text_column"]);
			updateRow[$"dp{provider.Index}_text_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_text_column"]);
			updateRow[$"dp{provider.Index}_date_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_date_column"]);

			//datetime comparison with tolerance of 2 milliseconds,
			//because postgres datetime precision is 1 millisecond
			((DateTime)updateRow[$"dp{provider.Index}_datetime_column"] -
				(DateTime)updatedRow[$"dp{provider.Index}_datetime_column"])
				.TotalMilliseconds.Should().BeLessThan(2);

			updateRow[$"dp{provider.Index}_short_int_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_short_int_column"]);
			updateRow[$"dp{provider.Index}_int_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_int_column"]);
			updateRow[$"dp{provider.Index}_long_int_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_long_int_column"]);
			updateRow[$"dp{provider.Index}_number_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_number_column"]);

			//TODO rumen - shared columns should be returned from get provider row
			//updateRow[$"test_data_identity_1.sc_text"].Should()
			//	.Be(updatedRow[$"test_data_identity_1.sc_text"]);
			//updateRow[$"test_data_identity_2.sc_int"].Should()
			//	.Be(updatedRow[$"test_data_identity_2.sc_int"]);

		}
	}

	public void Data_UpdateDatasetRow(TfDataProvider provider, TfDataset dataset)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var faker = new Faker("en");

		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			var newRow = new Dictionary<string, object?>();

			newRow[$"dp{provider.Index}_guid_column"] = Guid.NewGuid();
			newRow[$"dp{provider.Index}_short_text_column"] = faker.Lorem.Sentence();
			newRow[$"dp{provider.Index}_text_column"] = faker.Lorem.Lines();
			newRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
			newRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
			newRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
			newRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
			newRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
			newRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

			newRow["test_data_identity_1.sc_text"] = "this is shared columns text";
			newRow["test_data_identity_2.sc_int"] = 0;

			var insertedRow = tfService.InsertDatasetRow(dataset.Id, newRow);

			Guid tfId = (Guid)insertedRow["tf_id"];
			var updateRow = new Dictionary<string, object?>();

			updateRow[$"dp{provider.Index}_short_text_column"] = faker.Lorem.Sentence() + " upd";
			updateRow[$"dp{provider.Index}_text_column"] = faker.Lorem.Lines() + " upd";
			updateRow[$"dp{provider.Index}_date_column"] = faker.Date.PastDateOnly();
			updateRow[$"dp{provider.Index}_datetime_column"] = faker.Date.Future();
			updateRow[$"dp{provider.Index}_short_int_column"] = faker.Random.Short(0, 100);
			updateRow[$"dp{provider.Index}_int_column"] = faker.Random.Number(100, 1000);
			updateRow[$"dp{provider.Index}_long_int_column"] = faker.Random.Long(1000, 10000);
			updateRow[$"dp{provider.Index}_number_column"] = faker.Random.Decimal(100000, 1000000);

			updateRow["test_data_identity_1.sc_text"] = "this is shared columns text" + " upd";
			updateRow["test_data_identity_2.sc_int"] = 1;

			var updatedRow = tfService.UpdateDatasetRow(tfId, dataset.Id, updateRow);

			updateRow[$"dp{provider.Index}_short_text_column"].Should()
			.Be(updatedRow[$"dp{provider.Index}_short_text_column"]);
			updateRow[$"dp{provider.Index}_text_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_text_column"]);
			updateRow[$"dp{provider.Index}_date_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_date_column"]);

			//datetime comparison with tolerance of 2 milliseconds,
			//because postgres datetime precision is 1 millisecond
			((DateTime)updateRow[$"dp{provider.Index}_datetime_column"] -
				(DateTime)updatedRow[$"dp{provider.Index}_datetime_column"])
				.TotalMilliseconds.Should().BeLessThan(2);

			updateRow[$"dp{provider.Index}_short_int_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_short_int_column"]);
			updateRow[$"dp{provider.Index}_int_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_int_column"]);
			updateRow[$"dp{provider.Index}_long_int_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_long_int_column"]);
			updateRow[$"dp{provider.Index}_number_column"].Should()
				.Be(updatedRow[$"dp{provider.Index}_number_column"]);
			updateRow[$"test_data_identity_1.sc_text"].Should()
				.Be(updatedRow[$"test_data_identity_1.sc_text"]);
			updateRow[$"test_data_identity_2.sc_int"].Should()
				.Be(updatedRow[$"test_data_identity_2.sc_int"]);
		}
	}

	public void Data_InsertProviderRow_ValidateUniqueValues(TfDataProvider provider)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var faker = new Faker("en");


		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_guid_column_unique";
				try
				{
					newRow[columnName] = Guid.NewGuid();
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_short_text_column_unique";
				try
				{
					newRow[columnName] = columnName;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_text_column_unique";
				try
				{
					newRow[columnName] = columnName;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_date_column_unique";
				try
				{
					newRow[columnName] = DateOnly.FromDateTime(DateTime.Now);
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_datetime_column_unique";
				try
				{
					newRow[columnName] = DateTime.Now;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_short_int_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_int_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_long_int_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_number_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertProviderRow(provider.Id, newRow);
					tfService.InsertProviderRow(provider.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}
		}
	}

	public void Data_InsertDatasetRow_ValidateUniqueValues(TfDataProvider provider, TfDataset dataset)
	{
		ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
		ITfService tfService = ServiceProvider.GetService<ITfService>();
		var faker = new Faker("en");


		using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
		{
			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_guid_column_unique";
				try
				{
					newRow[columnName] = Guid.NewGuid();
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_short_text_column_unique";
				try
				{
					newRow[columnName] = columnName;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_text_column_unique";
				try
				{
					newRow[columnName] = columnName;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_date_column_unique";
				try
				{
					newRow[columnName] = DateOnly.FromDateTime(DateTime.Now);
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_datetime_column_unique";
				try
				{
					newRow[columnName] = DateTime.Now;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_short_int_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_int_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_long_int_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}

			{
				var newRow = new Dictionary<string, object?>();
				Exception exception = null;
				string columnName = $"dp{provider.Index}_number_column_unique";
				try
				{
					newRow[columnName] = 1;
					tfService.InsertDatasetRow(dataset.Id, newRow);
					tfService.InsertDatasetRow(dataset.Id, newRow);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				exception.Should().NotBeNull();
				exception.Should().BeOfType<TfValidationException>();
				var valEx = exception as TfValidationException;
				valEx.Data[columnName].Should().NotBeNull();
				((List<ValidationError>)valEx.Data[columnName])[0].PropertyName.Should().Be(columnName);
				((List<ValidationError>)valEx.Data[columnName])[0].Message.Should().Be("Value must be unique");
			}
		}
	}
}