using System.Text;
using WebVella.Tefter.Services;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Blank.Addons;

public class BlankDataProvider : ITfDataProviderAddon
{

    public const string ID = "d4191b4b-9f18-40cb-b2ca-fc33a5750124";
    public const string NAME = "Blank Data Provider";
    public const string DESCRIPTION = "Providers way to create blank data provider with ability to create manually structure";
    public const string FLUENT_ICON_NAME = "DocumentTable";

    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;

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
        throw new Exception("This dataprovider does not support synchronization");
    }

    /// <summary>
    /// Gets the data source schema
    /// </summary>
    public TfDataProviderSourceSchemaInfo GetDataProviderSourceSchema(TfDataProvider provider)
    {
        return new TfDataProviderSourceSchemaInfo();
    }

    /// <summary>
    /// Validates its custom settings on user submit
    /// </summary>
    public List<ValidationError> Validate(string settingsJson)
    {
        return new List<ValidationError>();
    }
}
