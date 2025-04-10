namespace WebVella.Tefter.Tests.Services;

public partial class TfServiceTest : BaseTest
{

	[Fact]
	public async Task Blob_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			tfService.InitBlobStorageFolder();

			if (dbService.Configuration.ConnectionString.ToLowerInvariant().Contains("database=tefter;"))
				throw new Exception("Invalid database");

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				const string sampleJsonFileContent = @"{
	""Tefter"": {
		""ConnectionString"": ""Server=192.168.0.190;Port=5436;User Id=dev;Password=dev;Database=tefter_unittests;Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=120;"",
		""CryptoPassword"": ""Encryption requires a key, which is created and managed by the data protection system"",
		""FilesRootPath"": """"
	}
}";
				const string sampleJsonFileContent2 = @"{
	""Tefter"": {
		""ConnectionString"": ""Server=192.168.0.192;Port=5436;User Id=dev;Password=dev;Database=tefter_unittests;Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=120;"",
		""CryptoPassword"": ""Encryption requires a key, which is created and managed by the data protection system"",
		""FilesRootPath"": """"
	}
}";

				byte[] sampleBytes = Encoding.UTF8.GetBytes(sampleJsonFileContent);
				byte[] sampleBytes2 = Encoding.UTF8.GetBytes(sampleJsonFileContent2);

				// create from bytes
				Guid blobId = Guid.Empty;
				var task = Task.Run(() => { blobId = tfService.CreateBlob(sampleBytes); });
				var exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				blobId.Should().NotBe(Guid.Empty);

				//read bytes
				byte[] bytes = null;
				task = Task.Run(() => { bytes = tfService.GetBlobByteArray(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				bytes.Should().NotBeNull();

				var json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//get stream
				Stream stream = null;
				task = Task.Run(() =>
				{
					stream = tfService.GetBlobStream(blobId);
					bytes = ReadFully(stream);
					stream.Close();
				});
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				bytes.Should().NotBeNull();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//exists
				bool doExists = false;
				task = Task.Run(() => { doExists = tfService.ExistsBlob(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				doExists.Should().BeTrue();

				//update
				task = Task.Run(() => { tfService.UpdateBlob(blobId, sampleBytes2); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();

				task = Task.Run(() => { bytes = tfService.GetBlobByteArray(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				bytes.Should().NotBeNull();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent2);

				//delete
				task = Task.Run(() => { tfService.DeleteBlob(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();


				//exists after delete
				task = Task.Run(() => { doExists = tfService.ExistsBlob(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				doExists.Should().BeFalse();

				//create from local path
				string tmpFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				Stream fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				var bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = Guid.Empty;
				task = Task.Run(() => { blobId = tfService.CreateBlob(tmpFilePath); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				blobId.Should().NotBe(Guid.Empty);


				//local path file should be deleted after successful create of blob
				File.Exists(tmpFilePath).Should().BeFalse();

				doExists = false;
				task = Task.Run(() => { doExists = tfService.ExistsBlob(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				doExists.Should().BeTrue();

				//create from stream
				tmpFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = Guid.Empty;
				task = Task.Run(() => { blobId = tfService.CreateBlob(File.Open(tmpFilePath, FileMode.Open)); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				blobId.Should().NotBe(Guid.Empty);

				//file should not deleted after successful create of blob because its from stream
				File.Exists(tmpFilePath).Should().BeTrue();

				doExists = false;
				task = Task.Run(() => { doExists = tfService.ExistsBlob(blobId); });
				exception = Record.ExceptionAsync(async () => await task).Result;
				exception.Should().BeNull();
				doExists.Should().BeTrue();

				//cleanup by deleting storage folder
				Directory.Delete(tfService.BlobStoragePath, true);
			}
		}
	}

	[Fact]
	public async Task Blob_TemporaryBlobOperations()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfService tfService = ServiceProvider.GetService<ITfService>();
			tfService.InitBlobStorageFolder();

			if (dbService.Configuration.ConnectionString.ToLowerInvariant().Contains("database=tefter;"))
				throw new Exception("Invalid database");

			using (var scope = dbService.CreateTransactionScope(TfConstants.DB_OPERATION_LOCK_KEY))
			{
				const string sampleJsonFileContent = @"{
	""Tefter"": {
		""ConnectionString"": ""Server=192.168.0.190;Port=5436;User Id=dev;Password=dev;Database=tefter_unittests;Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=120;"",
		""CryptoPassword"": ""Encryption requires a key, which is created and managed by the data protection system"",
		""FilesRootPath"": """"
	}
}";
				const string sampleJsonFileContent2 = @"{
	""Tefter"": {
		""ConnectionString"": ""Server=192.168.0.192;Port=5436;User Id=dev;Password=dev;Database=tefter_unittests;Pooling=true;MinPoolSize=1;MaxPoolSize=100;CommandTimeout=120;"",
		""CryptoPassword"": ""Encryption requires a key, which is created and managed by the data protection system"",
		""FilesRootPath"": """"
	}
}";

				byte[] sampleBytes = Encoding.UTF8.GetBytes(sampleJsonFileContent);
				byte[] sampleBytes2 = Encoding.UTF8.GetBytes(sampleJsonFileContent2);

				// create from bytes
				var blobId = tfService.CreateBlob(sampleBytes, true);

				//read bytes
				var bytes = tfService.GetBlobByteArray(blobId, true);

				var json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//get stream
				var stream = tfService.GetBlobStream(blobId, true);

				bytes = ReadFully(stream);

				stream.Close();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//exists
				var doExists = tfService.ExistsBlob(blobId, true);
				doExists.Should().BeTrue();

				//update
				tfService.UpdateBlob(blobId, sampleBytes2, true);

				bytes = tfService.GetBlobByteArray(blobId, true);

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent2);

				//delete
				tfService.DeleteBlob(blobId, true);

				//exists after delete
				doExists = tfService.ExistsBlob(blobId, true);
				doExists.Should().BeFalse();

				//create from local path
				string tmpFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				Stream fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				var bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = tfService.CreateBlob(tmpFilePath, true);

				//local path file should be deleted after successful create of blob
				File.Exists(tmpFilePath).Should().BeFalse();

				doExists = tfService.ExistsBlob(blobId, true);
				doExists.Should().BeTrue();

				//create from stream
				tmpFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = tfService.CreateBlob(File.Open(tmpFilePath, FileMode.Open), true);

				//file should not deleted after successful create of blob because its from stream
				File.Exists(tmpFilePath).Should().BeTrue();

				doExists = tfService.ExistsBlob(blobId, true);
				doExists.Should().BeTrue();


				tfService.MakeTempBlobPermanent(blobId);

				doExists = tfService.ExistsBlob(blobId, false);
				doExists.Should().BeTrue();

				doExists = tfService.ExistsBlob(blobId, true);
				doExists.Should().BeFalse();

				//cleanup by deleting storage folder
				Directory.Delete(tfService.BlobStoragePath, true);
			}
		}
	}

	public static byte[] ReadFully(Stream input)
	{
		byte[] buffer = new byte[16 * 1024];
		using (MemoryStream ms = new MemoryStream())
		{
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				ms.Write(buffer, 0, read);
			}
			return ms.ToArray();
		}
	}
}