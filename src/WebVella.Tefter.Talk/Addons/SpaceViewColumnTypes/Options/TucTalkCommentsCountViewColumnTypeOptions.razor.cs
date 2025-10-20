using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.UI.Addons;

public partial class TucTalkCommentsCountViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
    [Parameter] public TfSpaceViewColumnOptionsModeContext Context { get; set; } = null!;

    [Parameter] public EventCallback<TfTalkCommentsCountViewColumnTypeSettings> SettingsChanged { get; set; }
    [Parameter] public EventCallback<Tuple<string, string?>> DataMappingChanged { get; set; }
    [Parameter] public List<TalkChannel> ChannelOptions { get; set; } = new();


    private async Task _folderSelectHandler(TalkChannel? channel)
    {
        var settings = Context.GetSettings<TfTalkCommentsCountViewColumnTypeSettings>();
        settings.ChannelId = channel?.Id;
        await SettingsChanged.InvokeAsync(settings);
        if (channel is not null)
            await DataMappingChanged.InvokeAsync(new Tuple<string, string?>("Value",
                $"{channel.DataIdentity}.{channel.CountSharedColumnName}"));
        else
            await DataMappingChanged.InvokeAsync(new Tuple<string, string?>("Value", null));
    }
}