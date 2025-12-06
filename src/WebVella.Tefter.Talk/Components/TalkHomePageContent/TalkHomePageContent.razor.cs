using Microsoft.AspNetCore.Components.Routing;

namespace WebVella.Tefter.Talk.Components;

public partial class TalkHomePageContent : TfBaseComponent
{
    [Inject] public ITalkService TalkService { get; set; }
   
}