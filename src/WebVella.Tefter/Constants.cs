namespace WebVella.Tefter;

internal class Constants
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
    public static DateTime DB_INITIAL_LAST_COMMITED = new DateTime(2000,1,1,0,0,0,0,DateTimeKind.Utc);

	public const string TF_GENERIC_TEXT_COLUMN_TYPE_ID = "f061a3ce-7813-4fd6-98cb-a10cccea4797"; //used as default in view column creation, for convenience
	public const string TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID = "c28e933b-6800-4819-b22f-e091e3e3c961";
	public const string TF_GENERIC_DATETIME_COLUMN_TYPE_ID = "d41752c3-e356-4c51-83ed-7e1a4e5e5183";
	public const string TF_GENERIC_GUID_COLUMN_TYPE_ID = "b736b4f9-2138-44d2-a5d5-4a320b6556db";
	public const string TF_GENERIC_NUMBER_COLUMN_TYPE_ID = "5d246be4-d202-434c-961e-204e44ee0450";
}
