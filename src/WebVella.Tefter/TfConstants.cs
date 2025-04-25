namespace WebVella.Tefter;

public partial class TfConstants
{
	public const string TEFTER_AUTH_COOKIE_NAME = "tefter-auth";

	public static Guid TEFTER_CORE_APP_ID = new Guid("7d272612-0824-496a-a02d-2c144959ae73");

	//because the postgres NUMERIC type can hold a value of up to 131,072 digits
	//before the decimal point 16,383 digits after the decimal point.
	//we need to fit value in C# decimal which allow only 28 numbers,
	//we limit default precision to 28 and scale only to 8 numbers
	private const int DB_DEFAULT_NUMBER_PRECISION = 28;
    private const int DB_DEFAULT_NUMBER_SCALE = 8;

    //postgres name boundaries for length
    //maximum length is 63 but we reserve 13 for prefixes
    public const int DB_MIN_OBJECT_NAME_LENGTH = 2; 
    public const int DB_MAX_OBJECT_NAME_LENGTH = 50;
    public const string DB_OBJECT_NAME_VALIDATION_PATTERN = @"^[a-z](?!.*__)[a-z0-9_]*[a-z0-9]$";

    public const string DB_GUID_COLUMN_AUTO_DEFAULT_VALUE = "uuid_generate_v1()";
    public const string DB_DATETIME_COLUMN_AUTO_DEFAULT_VALUE = "clock_timestamp()";

    public const string DB_TABLE_ID_COLUMN_NAME = "id";
    public const string DB_OPERATION_LOCK_KEY = "DB_OPERATION_LOCK_KEY";
	public const string DB_SYNC_OPERATION_LOCK_KEY = "DB_SYNC_OPERATION_LOCK_KEY";
	public static DateTime DB_INITIAL_LAST_COMMITED = new DateTime(2000,1,1,0,0,0,0,DateTimeKind.Utc);

	internal const string SHARED_KEY_SEPARATOR = "$$$";
	internal const string VALIDATION_INDEX_SEPARATOR = "$$||$$";

	public static CultureInfo TF_FILTER_CULTURE = CultureInfo.InvariantCulture;
	public static NumberStyles TF_FILTER_NUMBER_STYLE = 
		NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
		NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint ;

	public static Guid ADMIN_ROLE_ID = new Guid("3a0c26c5-bd28-4cca-aaf7-5d225b4c3136");
	public static Guid ADMIN_USER_ID = new Guid("9c9f7fdc-3ce9-4e2a-9b0c-ba23f0949149");


}
