namespace WebVella.Tefter.Demo.Components;

public partial class WvSetThemeDialog : WvBaseComponent, IDialogContentComponent<User>
{
    [Parameter]
    public User Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog Dialog { get; set; }


    private async Task _save(){
        await Dialog.CloseAsync(Content);
    }
    private async Task _cancel(){
        await Dialog.CancelAsync();
    }
}