using System.Collections;
using ClosedXML.Excel;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.UI.Addons;

namespace WebVella.Tefter.Assets.Addons;

public class TfFolderAssetsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
    #region << INIT >>

    public const string ID = "aafd5f8a-95d0-4f6b-8b43-c75a80316504";
    public const string NAME = "Assets Folder Count";
    public const string DESCRIPTION = "displays related files count";
    public const string FLUENT_ICON_NAME = "DocumentCopy";
    private const string VALUE_ALIAS = "Value";

    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
    public List<TfSpaceViewColumnDataMappingDefinition> DataMappingDefinitions { get; init; } = new()
    {
        new TfSpaceViewColumnDataMappingDefinition
        {
            Alias = VALUE_ALIAS,
            IsHidden = true,
            Description =
                "this column is compatible with all integer database column types",
            SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType>
            {
                TfDatabaseColumnType.ShortInteger, TfDatabaseColumnType.Integer, TfDatabaseColumnType.LongInteger,
            }
        }
    };        

    #endregion

    #region << PRIVATE >>

    private List<ValidationError> _validationErrors = new();

    #endregion

    #region << PUBLIC >>

    public void ProcessExcelCell(TfSpaceViewColumnBaseContext args)
    {
        if (args is not TfSpaceViewColumnExportExcelModeContext)
            throw new Exception("Wrong context type. TfSpaceViewColumnExportExcelModeContext is expected");
        if (args is TfSpaceViewColumnExportExcelModeContext context)
            context.ExcelCell.SetValue(XLCellValue.FromObject(String.Join(", ", _initValue(args))));
    }

    //Returns Value/s as string usually for CSV export
    public string GetValueAsString(TfSpaceViewColumnBaseContext args)
    {
        if (args is not TfSpaceViewColumnExportCsvModeContext)
            throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

        return String.Join(", ", _initValue(args));
    }

    public RenderFragment Render(TfSpaceViewColumnBaseContext args)
    {
        if (args is null)
            throw new Exception("TfSpaceViewColumnBaseContext is expected");
        if (args is TfSpaceViewColumnReadModeContext readContext)
            return _renderReadMode(readContext);
        if (args is TfSpaceViewColumnEditModeContext editContext)
            return _renderEditMode(editContext);
        if (args is TfSpaceViewColumnOptionsModeContext optionsContext)
            return _renderOptionsMode(optionsContext);

        throw new Exception("Unsupported render mode");
    }

    public List<ValidationError> ValidateTypeOptions(TfSpaceViewColumnOptionsModeContext args)
    {
        _validationErrors = new();
        var settings = args.GetSettings<TfFolderAssetsCountViewColumnTypeSettings>();
        if(settings.FolderId is null || settings.FolderId == Guid.Empty)
            _validationErrors.Add(new ValidationError("FolderId", "required"));        
        return _validationErrors;
    }

    #endregion

    #region << Private >>

    //Value
    private List<long?> _initValue(TfSpaceViewColumnBaseContext args)
    {
        var values = new List<long?>();

        var (column, columnData) = args.GetColumnAndDataByAlias(VALUE_ALIAS);
        if (column is null) return values;
        if (columnData is null)
        {
            values.Add(null);
        }
        else if (column.Origin == TfDataColumnOriginType.JoinedProviderColumn)
        {
            if (columnData.GetType().ImplementsInterface(typeof(IList)))
            {
                if (columnData is IList joinValues)
                {
                    foreach (var joinValue in joinValues)
                        values.Add((long?)joinValue);
                }
            }
        }
        else
            values.Add(Convert.ToInt64(columnData));

        return values;
    }
    
    //Render
    private RenderFragment _renderReadMode(TfSpaceViewColumnReadModeContext context)
    {
        var values = _initValue(context);
        string? columnName = null;
        if (context.ViewColumn.DataMapping.TryGetValue(VALUE_ALIAS, out columnName))
        {
        }
        return builder =>
        {
            builder.OpenComponent<TucFolderAssetsCountViewColumnTypeRead>(0);
            builder.AddAttribute(1, "Value", values);
            builder.AddAttribute(2, "Context", context);
            builder.AddAttribute(3, "ColumnName", columnName);
            builder.CloseComponent();
        };
    }

    private RenderFragment _renderEditMode(TfSpaceViewColumnEditModeContext context)
    {
        //Non Editable columns
        return _renderReadMode(new TfSpaceViewColumnReadModeContext(context.ViewData)
        {
            TfService = context.TfService,
            ServiceProvider = context.ServiceProvider,
            ViewColumn = context.ViewColumn,
            DataTable = context.DataTable,
            RowId = context.RowId,
        });
    }

    private RenderFragment _renderOptionsMode(TfSpaceViewColumnOptionsModeContext context)
    {
        var assetSrv = context.ServiceProvider.GetRequiredService<IAssetsService>();
        return builder =>
        {
            builder.OpenComponent<TucFolderAssetsCountViewColumnTypeOptions>(0);
            builder.AddAttribute(1, "Context", context);
            builder.AddAttribute(2, "FolderOptions", assetSrv.GetFolders());
            builder.AddAttribute(3, "SettingsChanged", EventCallback.Factory.Create<TfFolderAssetsCountViewColumnTypeSettings>(this,
                async (options) =>
                {
                    context.SetColumnTypeOptions(options);
                    await context.SettingsChanged.InvokeAsync(context.ViewColumn.TypeOptionsJson);
                }));
            builder.AddAttribute(4, "DataMappingChanged", EventCallback.Factory.Create<Tuple<string,string?>>(this,
                async (mapping) =>
                {
                    await context.DataMappingChanged.InvokeAsync(mapping);
                }));            
            builder.CloseComponent();
        };
    }

    #endregion
}

public class TfFolderAssetsCountViewColumnTypeSettings
{
    [JsonPropertyName("FolderId")] public Guid? FolderId { get; set; } = null;
}