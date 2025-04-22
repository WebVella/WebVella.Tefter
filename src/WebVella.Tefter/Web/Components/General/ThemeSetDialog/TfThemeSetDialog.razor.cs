namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.ThemeSetDialog.TfThemeSetDialog","WebVella.Tefter")]
public partial class TfThemeSetDialog : TfFormBaseComponent, IDialogContentComponent<TucThemeSettings>
{
    [Parameter]
    public TucThemeSettings Content { get; set; }

    [CascadingParameter]
    public FluentDialog Dialog { get; set; }

    public DesignThemeModes _themeMode = DesignThemeModes.System;
    public TfColor _themeColor = TfColor.Emerald500;

    protected override void OnParametersSet()
    {
        if (Content is null)
        {
            _themeMode = DesignThemeModes.System;
            _themeColor = TfColor.Emerald500;
        }
        else
        {
            _themeMode = Content.ThemeMode;
            _themeColor = Content.ThemeColor;
        }
    }

    private async Task _save()
    {
        await Dialog.CloseAsync(new TucThemeSettings{ ThemeMode = _themeMode, ThemeColor = _themeColor });
    }
    private async Task _cancel()
    {
        await Dialog.CancelAsync();
    }
}