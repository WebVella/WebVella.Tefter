namespace WebVella.Tefter;

public interface ITfTemplateManager
{
	public Result<bool> LoadExcelFile(Guid blobId);

}

internal class TfTemplateManager : ITfTemplateManager
{
	public TfTemplateManager(ITfConfigurationService config)
	{
		
	}

	public Result<bool> LoadExcelFile(Guid blobId)
	{
		throw new NotImplementedException();
	}
}
