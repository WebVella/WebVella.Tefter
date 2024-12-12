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
				var createResult = blobManager.CreateBlob(sampleBytes);
				createResult.IsSuccess.Should().BeTrue();

				var blobId = createResult.Value;

				//read bytes
				var getBytesResult = blobManager.GetBlobByteArray(blobId);
				getBytesResult.IsSuccess.Should().BeTrue();

				var json = Encoding.UTF8.GetString(getBytesResult.Value);
				json.Should().Be(sampleJsonFileContent);

				//get stream
				var getStreamResult = blobManager.GetBlobStream(blobId);
				getStreamResult.IsSuccess.Should().BeTrue();

				var bytes = ReadFully(getStreamResult.Value);
				getStreamResult.Value.Close();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//exists
				var isExistsResult = blobManager.ExistsBlob(blobId);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();

				//update
				var updateResult = blobManager.UpdateBlob(blobId, sampleBytes2);
				updateResult.IsSuccess.Should().BeTrue();

				getBytesResult = blobManager.GetBlobByteArray(blobId);
				getBytesResult.IsSuccess.Should().BeTrue();

				json = Encoding.UTF8.GetString(getBytesResult.Value);
				json.Should().Be(sampleJsonFileContent2);

				//delete
				var deleteResult = blobManager.DeleteBlob(blobId);
				deleteResult.IsSuccess.Should().BeTrue();

				//exists after delete
				isExistsResult = blobManager.ExistsBlob(blobId);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeFalse();

				//create from local path
				string tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				Stream fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				var bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				createResult = blobManager.CreateBlob(tmpFilePath);
				createResult.IsSuccess.Should().BeTrue();

				//local path file should be deleted after successful create of blob
				File.Exists(tmpFilePath).Should().BeFalse();

				blobId = createResult.Value;

				isExistsResult = blobManager.ExistsBlob(blobId);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();

				//create from stream
				tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				createResult = blobManager.CreateBlob(File.Open(tmpFilePath, FileMode.Open));
				createResult.IsSuccess.Should().BeTrue();

				//file should not deleted after successful create of blob because its from stream
				File.Exists(tmpFilePath).Should().BeTrue();

				blobId = createResult.Value;

				isExistsResult = blobManager.ExistsBlob(blobId);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();

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
				var createResult = blobManager.CreateBlob(sampleBytes, true);
				createResult.IsSuccess.Should().BeTrue();

				var blobId = createResult.Value;

				//read bytes
				var getBytesResult = blobManager.GetBlobByteArray(blobId, true);
				getBytesResult.IsSuccess.Should().BeTrue();

				var json = Encoding.UTF8.GetString(getBytesResult.Value);
				json.Should().Be(sampleJsonFileContent);

				//get stream
				var getStreamResult = blobManager.GetBlobStream(blobId, true);
				getStreamResult.IsSuccess.Should().BeTrue();

				var bytes = ReadFully(getStreamResult.Value);
				getStreamResult.Value.Close();

				json = Encoding.UTF8.GetString(bytes);
				json.Should().Be(sampleJsonFileContent);

				//exists
				var isExistsResult = blobManager.ExistsBlob(blobId, true);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();

				//update
				var updateResult = blobManager.UpdateBlob(blobId, sampleBytes2, true);
				updateResult.IsSuccess.Should().BeTrue();

				getBytesResult = blobManager.GetBlobByteArray(blobId,true);
				getBytesResult.IsSuccess.Should().BeTrue();

				json = Encoding.UTF8.GetString(getBytesResult.Value);
				json.Should().Be(sampleJsonFileContent2);

				//delete
				var deleteResult = blobManager.DeleteBlob(blobId, true);
				deleteResult.IsSuccess.Should().BeTrue();

				//exists after delete
				isExistsResult = blobManager.ExistsBlob(blobId,true);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeFalse();

				//create from local path
				string tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				Stream fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				var bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				createResult = blobManager.CreateBlob(tmpFilePath,true);
				createResult.IsSuccess.Should().BeTrue();

				//local path file should be deleted after successful create of blob
				File.Exists(tmpFilePath).Should().BeFalse();

				blobId = createResult.Value;

				isExistsResult = blobManager.ExistsBlob(blobId,true);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();

				//create from stream
				tmpFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".json";
				fileStream = File.Open(tmpFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
				bw = new BinaryWriter(fileStream);
				bw.Write(sampleJsonFileContent2);
				bw.Close();
				fileStream.Close();

				createResult = blobManager.CreateBlob(File.Open(tmpFilePath, FileMode.Open),true);
				createResult.IsSuccess.Should().BeTrue();

				//file should not deleted after successful create of blob because its from stream
				File.Exists(tmpFilePath).Should().BeTrue();

				blobId = createResult.Value;

				isExistsResult = blobManager.ExistsBlob(blobId,true);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();


				var permanentResult = blobManager.MakeTempBlobPermanent(blobId);
				isExistsResult.IsSuccess.Should().BeTrue();

				isExistsResult = blobManager.ExistsBlob(blobId, false);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeTrue();

				isExistsResult = blobManager.ExistsBlob(blobId, true);
				isExistsResult.IsSuccess.Should().BeTrue();
				isExistsResult.Value.Should().BeFalse();

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