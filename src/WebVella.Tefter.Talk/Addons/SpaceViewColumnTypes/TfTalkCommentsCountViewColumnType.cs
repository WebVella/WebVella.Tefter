using System.Collections;
using ClosedXML.Excel;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.UI.Addons;

namespace WebVella.Tefter.Talk;

public class TfTalkCommentsCountViewColumnType : ITfSpaceViewColumnTypeAddon
{
    #region << INIT >>

    public const string ID = "60ab4197-be46-4ebd-a6de-02e8d25450d3";
    public const string NAME = "Talk Comments count";
    public const string DESCRIPTION = "displays related comments count";
    public const string FLUENT_ICON_NAME = "CommentMultiple";
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
        var settings = args.GetSettings<TfTalkCommentsCountViewColumnTypeSettings>();
        if (settings.ChannelId is null) return (values,null);        

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
            var talkSrv = args.ServiceProvider.GetRequiredService<ITalkService>();
            var channel = talkSrv.GetChannel(settings.ChannelId.Value);
            sharedColumnName = channel?.CountSharedColumnName;
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
            builder.OpenComponent<TucTalkCommentsCountViewColumnTypeRead>(0);
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
        var talkSrv = context.ServiceProvider.GetRequiredService<ITalkService>();
        return builder =>
        {
            builder.OpenComponent<TucTalkCommentsCountViewColumnTypeOptions>(0);
            builder.AddAttribute(1, "Context", context);
            builder.AddAttribute(2, "ChannelOptions", talkSrv.GetChannels());
            builder.AddAttribute(3, "SettingsChanged", EventCallback.Factory.Create<TfTalkCommentsCountViewColumnTypeSettings>(this,
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

public class TfTalkCommentsCountViewColumnTypeSettings
{
    [JsonPropertyName("ChannelId")] public Guid? ChannelId { get; set; } = null;
}