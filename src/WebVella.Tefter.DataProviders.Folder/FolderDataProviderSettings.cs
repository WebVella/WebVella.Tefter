namespace WebVella.Tefter.DataProviders.Folder;

public class FolderDataProviderSettings
{
    public List<FolderDataProviderShareSettings> Shares { get; set; } = new();
}

public class FolderDataProviderShareSettings
{
    public string Path { get; set; } = string.Empty;
    public string PathPrefix { get; set; } = string.Empty;
    public string? Domain { get; set; } = null;
    public string? Username { get; set; } = null;
    public string? Password { get; set; } = null;
}
