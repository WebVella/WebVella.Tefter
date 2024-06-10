﻿namespace WebVella.Tefter;

internal class Constants
{
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
}
