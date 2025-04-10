﻿namespace WebVella.Tefter;

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

	public const string TF_GENERIC_TEXT_COLUMN_TYPE_ID = "f061a3ce-7813-4fd6-98cb-a10cccea4797"; //used as default in view column creation, for convenience
	public const string TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID = "c28e933b-6800-4819-b22f-e091e3e3c961";
	public const string TF_GENERIC_DATEONLY_COLUMN_TYPE_ID = "59037088-e8b7-4ec6-858c-016f4eacf58a";
	public const string TF_GENERIC_DATETIME_COLUMN_TYPE_ID = "d41752c3-e356-4c51-83ed-7e1a4e5e5183";
	public const string TF_GENERIC_GUID_COLUMN_TYPE_ID = "b736b4f9-2138-44d2-a5d5-4a320b6556db";
	public const string TF_GENERIC_NUMBER_COLUMN_TYPE_ID = "5d246be4-d202-434c-961e-204e44ee0450";
	public const string TF_GENERIC_LONG_INTEGER_COLUMN_TYPE_ID = "22d5436c-1dec-4d1d-bb4d-f197f19c9d12";
	public const string TF_GENERIC_INTEGER_COLUMN_TYPE_ID = "a0708248-ebfc-4ba9-84e9-3f959c06989c";
	public const string TF_GENERIC_SHORT_INTEGER_COLUMN_TYPE_ID = "e8a52dfe-fcb8-4fd2-8011-bd9e0a5a81d9";

	public const string TF_COLUMN_COMPONENT_DISPLAY_TEXT_ID = "f7f6a912-f670-4275-8794-13a483cac2c9";
	public const string TF_COLUMN_COMPONENT_DISPLAY_BOOLEAN_ID = "a1119d49-aa69-4140-aaa3-de2b9d6a78bb";
	public const string TF_COLUMN_COMPONENT_DISPLAY_DATEONLY_ID = "6ee59177-2aad-4c90-a5b9-702b91ff358d";
	public const string TF_COLUMN_COMPONENT_DISPLAY_DATETIME_ID = "5676b4f0-a33a-455a-8ab1-e338b2e68c97";
	public const string TF_COLUMN_COMPONENT_DISPLAY_GUID_ID = "fb66f025-6701-400e-81b9-54d4046be8a6";
	public const string TF_COLUMN_COMPONENT_DISPLAY_INTEGER_ID = "40a51679-0e22-477e-bf60-9b0142043b7c";
	public const string TF_COLUMN_COMPONENT_DISPLAY_LONG_ID = "aeb523ff-a796-4158-bb59-73aa07e380e6";
	public const string TF_COLUMN_COMPONENT_DISPLAY_NUMBER_ID = "46deff56-8b26-4f93-8575-f929d2e2e7d7";
	public const string TF_COLUMN_COMPONENT_DISPLAY_SHORT_ID = "02f6f53c-cbe9-42e6-92a1-9fc5c7efb229";

	public static CultureInfo TF_FILTER_CULTURE = CultureInfo.InvariantCulture;
	public static NumberStyles TF_FILTER_NUMBER_STYLE = 
		NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
		NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint ;


}
