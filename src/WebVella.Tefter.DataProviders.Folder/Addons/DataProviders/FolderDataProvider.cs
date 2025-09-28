using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using WebVella.Tefter.Utility;


namespace WebVella.Tefter.DataProviders.Folder.Addons;

public class FolderDataProvider : ITfDataProviderAddon
{

    public const string ID = "e68ccfd5-46c2-4bcf-bb51-84d0bf66df81";
    public const string NAME = "Folder Data Provider";
    public const string DESCRIPTION = "Provide data from file system or network shares";
    public const string FLUENT_ICON_NAME = "DocumentTable";

    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;

    private const string COLUMN_NAME_ID = "id";
    private const string COLUMN_NAME_FULLNAME = "fullname";
    private const string COLUMN_NAME_NAME = "filename";
    private const string COLUMN_NAME_FOLDER_NAME = "folder";
    private const string COLUMN_NAME_LENGTH = "length";
    private const string COLUMN_NAME_CREATED_ON = "created_on";
    private const string COLUMN_NAME_MODIFIED_ON = "modified_on";

    /// <summary>
    /// Return what types of data types it can process from the data source
    /// </summary>
    public ReadOnlyCollection<string> GetSupportedSourceDataTypes()
    {
        //sample only
        return new List<string> {
            "TEXT", //Keep text on first place as the first place is used as default type
			"SHORT_TEXT",
            "BOOLEAN",
            "DATE",
            "DATETIME",
            "NUMBER",
            "SHORT_INTEGER",
            "INTEGER",
            "LONG_INTEGER",
            "GUID"
        }.AsReadOnly();
    }
    /// <summary>
    /// Returns mapping between source data types and Tefter data types
    /// </summary>
    public ReadOnlyCollection<TfDatabaseColumnType> GetDatabaseColumnTypesForSourceDataType(
        string dataType)
    {
        switch (dataType)
        {
            case "TEXT":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Text }.AsReadOnly();
            case "SHORT_TEXT":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortText }.AsReadOnly();
            case "BOOLEAN":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }.AsReadOnly();
            case "NUMBER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Number }.AsReadOnly();
            case "DATE":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateOnly, TfDatabaseColumnType.DateTime }.AsReadOnly();
            case "DATETIME":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.DateTime, TfDatabaseColumnType.DateOnly }.AsReadOnly();
            case "SHORT_INTEGER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.ShortInteger }.AsReadOnly();
            case "INTEGER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Integer }.AsReadOnly();
            case "LONG_INTEGER":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.LongInteger }.AsReadOnly();
            case "GUID":
                return new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }.AsReadOnly();

        }
        return new List<TfDatabaseColumnType>().AsReadOnly();
    }

    /// <summary>
    /// Gets data from the data source
    /// </summary>
    public ReadOnlyCollection<TfDataProviderDataRow> GetRows(
        TfDataProvider provider,
        ITfDataProviderSychronizationLog synchLog)
    {

        var result = new List<TfDataProviderDataRow>();

        FolderDataProviderSettings settings;

        try
        {
            synchLog.Log("start loading provider settings");
#if DEBUG
            settings = new FolderDataProviderSettings();
            settings.Shares = new List<FolderDataProviderShareSettings>();
            settings.Shares.Add(new FolderDataProviderShareSettings()
            {
                Path = @"\\192.168.0.190\Shared\folder_provider_root",
                PathPrefix = string.Empty,
                Domain = "",
                Username = "tefter",
                Password = "tefter"
            });
#else
            settings = JsonSerializer.Deserialize<FolderDataProviderSettings>(provider.SettingsJson) ?? new();
#endif
            synchLog.Log("complete loading provider settings");
        }
        catch (Exception ex)
        {
            synchLog.Log("failed loading provider settings", ex);
            throw;
        }

        if (settings is null)
        {
            synchLog.Log("failed loading provider settings");
            throw new Exception("provider settings are null");
        }

        if (settings.Shares is null || settings.Shares.Count == 0)
            return result.AsReadOnly();

        foreach (var share in settings.Shares)
        {
            if (string.IsNullOrEmpty(share.Path))
                continue;

            NetworkAccessHelper.RunAs(share.Domain, share.Username, share.Password, share.Path, () =>
            {
                Queue<string> queue = new();
                queue.Enqueue(share.Path);

                while (queue.Count > 0)
                {
                    ProcessFolder(
                        provider, queue.Dequeue(),
                        share.PathPrefix, result, queue);
                }
            });

        }

        //order by fullname if such column exists
        var column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_FULLNAME);
        if (column is not null)
        {
            return result
                .OrderBy(x => x[column.DbName!])
                .ToList()
                .AsReadOnly();
        }

        return result.AsReadOnly();

    }

    private static void ProcessFolder(
        TfDataProvider provider,
        string folderPath,
        string pathPrefix,
        List<TfDataProviderDataRow> rows,
        Queue<string> queue)
    {

        Directory.GetDirectories(folderPath)
            .ToList().ForEach(x => queue.Enqueue(Path.Combine(folderPath, x)));

        string[] files = Directory.GetFiles(folderPath);

        foreach (var file in files)
        {
            string filePath = Path.Combine(folderPath, file);
            rows.Add(GetDataProviderDataRow(provider, filePath, pathPrefix));
        }
    }

    private static TfDataProviderDataRow GetDataProviderDataRow(
         TfDataProvider provider,
        string filePath,
        string pathPrefix)
    {
        var info = new FileInfo(filePath);

        if (!info.Exists)
            throw new FileNotFoundException("File not found", filePath);

        TfDataProviderDataRow row = new TfDataProviderDataRow();
        var column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_ID);
        if (column is not null)
            row[column.DbName!] = $"{pathPrefix}-{info.FullName}".ToSha1();

        column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_FULLNAME);
        if (column is not null)
            row[column.DbName!] = info.FullName;

        column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_NAME);
        if (column is not null)
            row[column.DbName!] = info.Name;

        column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_FOLDER_NAME);
        if (column is not null)
            row[column.DbName!] = info.DirectoryName ?? string.Empty;

        column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_LENGTH);
        if (column is not null)
            row[column.DbName!] = info.Length;

        column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_CREATED_ON);
        if (column is not null)
            row[column.DbName!] = info.CreationTimeUtc.ToLocalTime();

        column = provider.Columns.SingleOrDefault(x => x.SourceName == COLUMN_NAME_MODIFIED_ON);
        if (column is not null)
            row[column.DbName!] = info.LastWriteTimeUtc.ToLocalTime();

        return row;
    }


    /// <summary>
    /// Gets the data source schema
    /// </summary>
    public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
    {
        TfDataProviderSourceSchemaInfo schemaInfo = new TfDataProviderSourceSchemaInfo();

        schemaInfo.SynchPrimaryKeyColumns = new() { COLUMN_NAME_ID };

        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_ID] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_ID] = "SHORT_TEXT";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_ID] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_ID] = new() { TfDatabaseColumnType.ShortText, TfDatabaseColumnType.Text };

        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_FULLNAME] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_FULLNAME] = "SHORT_TEXT";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_FULLNAME] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_FULLNAME] = new() { TfDatabaseColumnType.ShortText, TfDatabaseColumnType.Text };

        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_NAME] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_NAME] = "SHORT_TEXT";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_NAME] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_NAME] = new() { TfDatabaseColumnType.ShortText, TfDatabaseColumnType.Text };

        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_FOLDER_NAME] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_FOLDER_NAME] = "SHORT_TEXT";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_FOLDER_NAME] = TfDatabaseColumnType.ShortText;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_FOLDER_NAME] = new() { TfDatabaseColumnType.ShortText, TfDatabaseColumnType.Text };


        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_LENGTH] = TfDatabaseColumnType.LongInteger;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_LENGTH] = "LONG_INTEGER";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_LENGTH] = TfDatabaseColumnType.LongInteger;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_LENGTH] = new() { TfDatabaseColumnType.LongInteger };

        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_CREATED_ON] = TfDatabaseColumnType.DateTime;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_CREATED_ON] = "DATETIME";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_CREATED_ON] = TfDatabaseColumnType.DateTime;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_CREATED_ON] = new() { TfDatabaseColumnType.DateTime };

        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_MODIFIED_ON] = TfDatabaseColumnType.DateTime;
        schemaInfo.SourceColumnDefaultSourceType[COLUMN_NAME_MODIFIED_ON] = "DATETIME";
        schemaInfo.SourceColumnDefaultDbType[COLUMN_NAME_MODIFIED_ON] = TfDatabaseColumnType.DateTime;
        schemaInfo.SourceTypeSupportedDbTypes[COLUMN_NAME_MODIFIED_ON] = new() { TfDatabaseColumnType.DateTime };

        return schemaInfo;
    }

    /// <summary>
    /// Validates its custom settings on user submit
    /// </summary>
    public List<ValidationError> Validate(string settingsJson)
    {
        return new List<ValidationError>();
    }

    class NetworkAccessHelper
    {
        public enum LogonUserErrorCode
        {
            ERROR_LOGON_FAILURE = 1326,
            ERROR_ACCOUNT_RESTRICTION = 1327,
            ERROR_INVALID_LOGON_HOURS = 1328,
            ERROR_INVALID_WORKSTATION = 1329,
            ERROR_PASSWORD_EXPIRED = 1330,
            ERROR_ACCOUNT_DISABLED = 1331,
            ERROR_NONE_MAPPED = 1332,
            ERROR_TOO_MANY_LUIDS_REQUESTED = 1333,
            ERROR_LUIDS_EXHAUSTED = 1334,
            ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1909,
            ERROR_NO_LOGON_SERVERS = 1789,
            ERROR_NO_SUCH_LOGON_SESSION = 1790,
            ERROR_NO_SUCH_PRIVILEGE = 1791,
            ERROR_PRIVILEGE_NOT_HELD = 1792
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(
            string username,
            string domain,
            string password,
            int logonType,
            int logonProvider,
            out SafeAccessTokenHandle tokenHandle);

        public static void RunAs(
            string? domain,
            string? username,
            string? password,
            string sharePath,
            Action action)
        {
            const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;

            if (string.IsNullOrWhiteSpace(username) &&
                string.IsNullOrWhiteSpace(password))
            {
                action();
                return;
            }

            bool success = LogonUser(username ?? string.Empty,
                                     domain ?? string.Empty,
                                     password ?? string.Empty,
                                     LOGON32_LOGON_NEW_CREDENTIALS,
                                     LOGON32_PROVIDER_WINNT50,
                                     out SafeAccessTokenHandle tokenHandle);

            if (!success)
            {
                var lastError = Marshal.GetLastWin32Error();
                switch (lastError)
                {
                    case (int)LogonUserErrorCode.ERROR_LOGON_FAILURE:
                        throw new Exception("The username or password is incorrect.");
                    case (int)LogonUserErrorCode.ERROR_ACCOUNT_RESTRICTION:
                        throw new Exception("Account restrictions prevent login (e.g., time-of-day restrictions).");
                    case (int)LogonUserErrorCode.ERROR_INVALID_LOGON_HOURS:
                        throw new Exception("The account is not allowed to log on at this time.");
                    case (int)LogonUserErrorCode.ERROR_INVALID_WORKSTATION:
                        throw new Exception("The account is not allowed to log on from this workstation.");
                    case (int)LogonUserErrorCode.ERROR_PASSWORD_EXPIRED:
                        throw new Exception("The password has expired.");
                    case (int)LogonUserErrorCode.ERROR_ACCOUNT_DISABLED:
                        throw new Exception("The account is disabled.");
                    case (int)LogonUserErrorCode.ERROR_NONE_MAPPED:
                        throw new Exception("No mapping between account names and security IDs.");
                    case (int)LogonUserErrorCode.ERROR_TOO_MANY_LUIDS_REQUESTED:
                        throw new Exception("Too many local unique identifiers requested.");
                    case (int)LogonUserErrorCode.ERROR_LUIDS_EXHAUSTED:
                        throw new Exception("Local unique identifiers exhausted.");
                    case (int)LogonUserErrorCode.ERROR_TRUSTED_RELATIONSHIP_FAILURE:
                        throw new Exception("The trust relationship between workstation and domain failed.");
                    case (int)LogonUserErrorCode.ERROR_NO_LOGON_SERVERS:
                        throw new Exception("No domain controllers are available to validate credentials.");
                    case (int)LogonUserErrorCode.ERROR_NO_SUCH_LOGON_SESSION:
                        throw new Exception("The logon session does not exist.");
                    case (int)LogonUserErrorCode.ERROR_NO_SUCH_PRIVILEGE:
                        throw new Exception("A specified privilege does not exist.");
                    case (int)LogonUserErrorCode.ERROR_PRIVILEGE_NOT_HELD:
                        throw new Exception("The caller does not have the required privilege.");
                    default:
                        throw new Exception("LogonUser failed with error code: " + lastError);
                }
                throw new System.ComponentModel.Win32Exception();
            }

            try
            {
                WindowsIdentity.RunImpersonated(tokenHandle, () =>
                {
                    try
                    {
                        Directory.GetFiles(sharePath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Access failed: {ex.Message}");
                    }

                    action();

                });
            }
            finally
            {
                tokenHandle.Dispose();
            }

        }

    }
}
