namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Editor.TfEditor", "WebVella.Tefter")]
public partial class TfEditor : TfBaseComponent
{
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	private CancellationTokenSource inputThrottleCancalationToken = new();

	private Guid _componentId = Guid.NewGuid();

	private string strSavedContent = "";
	private ElementReference divEditorElement;
	private string EditorContent;
	private string EditorHTMLContent;
	private bool EditorEnabled = true;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JSRuntime.InvokeAsync<string>(
				"Tefter.createQuill", divEditorElement, _componentId.ToString());
		}
	}

	private async Task _valueChanged(string value)
	{
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(1000, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await ValueChanged.InvokeAsync(value);
		}, inputThrottleCancalationToken.Token);
	}

}