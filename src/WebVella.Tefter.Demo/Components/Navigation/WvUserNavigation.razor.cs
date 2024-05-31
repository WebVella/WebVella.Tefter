namespace WebVella.Tefter.Demo.Components;
public partial class WvUserNavigation : WvBaseComponent
{
    private bool _visible = false;

    private void _onClick(){ 
        _visible = !_visible;
    }
}