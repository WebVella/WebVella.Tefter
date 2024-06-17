using Boz.Tefter.Components;

namespace Boz.Tefter.Actions;
public partial class BozViewMenuItem : TfBaseComponent
{
	
	private void onClick(){
        ToastService.ShowToast(ToastIntent.Warning, "Hello from Boz App");

    }
}