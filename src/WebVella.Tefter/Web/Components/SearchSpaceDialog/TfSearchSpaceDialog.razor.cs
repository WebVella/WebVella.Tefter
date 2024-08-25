using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Components.SearchSpaceDialog;
public partial class TfSearchSpaceDialog : TfFormBaseComponent, IDialogContentComponent<bool>
{
	[Inject] private SpaceUseCase UC { get; set; }
	[Parameter] public bool Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
	

			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
