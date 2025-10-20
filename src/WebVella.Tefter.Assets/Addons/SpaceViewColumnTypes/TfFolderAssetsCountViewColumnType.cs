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
            context.ExcelCell.SetValue(XLCellValue.FromObject(String.Join(", ", _initValue(args).Item1)));
    }

    //Returns Value/s as string usually for CSV export
    public string GetValueAsString(TfSpaceViewColumnBaseContext args)
    {
        if (args is not TfSpaceViewColumnExportCsvModeContext)
            throw new Exception("Wrong context type. TfSpaceViewColumnExportCsvModeContext is expected");

        return String.Join(", ", _initValue(args).Item1);
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
        return _validationErrors;
    }

    #endregion

    #region << Private >>

    //Value
    private (List<long?>, string?) _initValue(TfSpaceViewColumnBaseContext args)
    {
        var values = new List<long?>();
        var settings = args.GetSettings<TfFolderAssetsCountViewColumnTypeSettings>();
        if (settings.FolderId is null) return (values,null);        

        string? sharedColumnName = null;
        //Try get shareColumnName from ViewData
        var storageKey = this.GetType().FullName + "_" + args.ViewColumn.Id;
        Dictionary<string, object?>? viewData = null;
        if (args is TfSpaceViewColumnReadModeContext readContext)
            viewData = readContext.ViewData;
        else if (args is TfSpaceViewColumnEditModeContext editContext)
            viewData = editContext.ViewData;

        if (viewData is not null && viewData.ContainsKey(storageKey))
        {
            if (!viewData.ContainsKey(storageKey) || viewData[storageKey] is not List<TfSelectOption>)
                throw new Exception($"ViewData in the context is not List<TfSelectOption> for key: {storageKey}");
            sharedColumnName = (string?)viewData[storageKey];
        }        
        
        //try get from service
        if (sharedColumnName is null)
        {
            var assetSrv = args.ServiceProvider.GetRequiredService<IAssetsService>();
            var folder = assetSrv.GetFolder(settings.FolderId.Value);
            sharedColumnName = folder?.CountSharedColumnName;
        }
        if(String.IsNullOrWhiteSpace(sharedColumnName))
            return (values,sharedColumnName);
        
        var (column, columnData) = args.GetColumnAndData(sharedColumnName);
        if (column is null) return (values,sharedColumnName);
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
            values.Add((long?)columnData);

        return (values,sharedColumnName);
    }

    //Render
    private RenderFragment _renderReadMode(TfSpaceViewColumnReadModeContext context)
    {
        var (values,columnName) = _initValue(context);
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