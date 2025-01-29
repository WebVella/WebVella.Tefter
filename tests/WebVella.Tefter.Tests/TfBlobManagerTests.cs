namespace WebVella.Tefter.Tests;

public partial class TfBlobManagerTests : BaseTest
{

	[Fact]
	public async Task BlobManager_CRUD()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfBlobManager blobManager = ServiceProvider.GetRequiredService<ITfBlobManager>();

			if (dbService.Configuration.ConnectionString.ToLowerInvariant().Contains("database=tefter;"))
				throw new Exception("Invalid database");

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
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
				var blobId = blobManager.CreateBlob(sampleBytes);

				//read bytes
				var bytes = blobManager.GetBlobByteArray(blobId);

				var json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//get stream
				var stream = blobManager.GetBlobStream(blobId);
				bytes = ReadFully(stream);
				stream.Close();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//exists
				var doExist = blobManager.ExistsBlob(blobId);
				doExist.Should().BeTrue();

				//update
				blobManager.UpdateBlob(blobId, sampleBytes2);

				bytes = blobManager.GetBlobByteArray(blobId);

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent2);

				//delete
				blobManager.DeleteBlob(blobId);
				
				//exists after delete
				var doExists = blobManager.ExistsBlob(blobId);
				doExists.Should().BeFalse();

				//create from local path
				string tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				Stream fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				var bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = blobManager.CreateBlob(tmpFilePath);
				
				//local path file should be deleted after successful create of blob
				File.Exists(tmpFilePath).Should().BeFalse();

				doExists = blobManager.ExistsBlob(blobId);
				doExists.Should().BeTrue();

				//create from stream
				tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = blobManager.CreateBlob(File.Open(tmpFilePath, FileMode.Open));

				//file should not deleted after successful create of blob because its from stream
				File.Exists(tmpFilePath).Should().BeTrue();

				doExists = blobManager.ExistsBlob(blobId);
				doExists.Should().BeTrue();

				//cleanup by deleting storage folder
				Directory.Delete(blobManager.BlobStoragePath, true);
			}
		}
	}

	[Fact]
	public async Task BlobManager_TemporaryBlobOperations()
	{
		using (await locker.LockAsync())
		{
			ITfDatabaseService dbService = ServiceProvider.GetRequiredService<ITfDatabaseService>();
			ITfBlobManager blobManager = ServiceProvider.GetRequiredService<ITfBlobManager>();

			if (dbService.Configuration.ConnectionString.ToLowerInvariant().Contains("database=tefter;"))
				throw new Exception("Invalid database");

			using (var scope = dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
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
				var blobId = blobManager.CreateBlob(sampleBytes, true);

				//read bytes
				var bytes = blobManager.GetBlobByteArray(blobId, true);

				var json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//get stream
				var stream = blobManager.GetBlobStream(blobId, true);

				bytes = ReadFully(stream);
				
				stream.Close();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//exists
				var doExists = blobManager.ExistsBlob(blobId, true);
				doExists.Should().BeTrue();

				//update
				blobManager.UpdateBlob(blobId, sampleBytes2, true);
				
				bytes = blobManager.GetBlobByteArray(blobId,true);

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent2);

				//delete
				blobManager.DeleteBlob(blobId, true);
				
				//exists after delete
				doExists = blobManager.ExistsBlob(blobId,true);
				doExists.Should().BeFalse();

				//create from local path
				string tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				Stream fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				var bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = blobManager.CreateBlob(tmpFilePath,true);

				//local path file should be deleted after successful create of blob
				File.Exists(tmpFilePath).Should().BeFalse();

				doExists = blobManager.ExistsBlob(blobId,true);
				doExists.Should().BeTrue();

				//create from stream
				tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				blobId = blobManager.CreateBlob(File.Open(tmpFilePath, FileMode.Open),true);

				//file should not deleted after successful create of blob because its from stream
				File.Exists(tmpFilePath).Should().BeTrue();

				doExists = blobManager.ExistsBlob(blobId,true);
				doExists.Should().BeTrue();


				blobManager.MakeTempBlobPermanent(blobId);

				doExists = blobManager.ExistsBlob(blobId, false);
				doExists.Should().BeTrue();

				doExists = blobManager.ExistsBlob(blobId, true);
				doExists.Should().BeFalse();

				//cleanup by deleting storage folder
				Directory.Delete(blobManager.BlobStoragePath, true);
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