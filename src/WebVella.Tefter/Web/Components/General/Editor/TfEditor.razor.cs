namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Editor.TfEditor", "WebVella.Tefter")]
public partial class TfEditor : TfBaseComponent
{
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }

	private CancellationTokenSource inputThrottleCancalationToken = new();
	private DotNetObjectReference<TfEditor> _objectRef;
	private Guid _componentId = Guid.NewGuid();

	private string _value = null;
	private bool _editorInited = false;
	private ElementReference divEditorElement;
	private string EditorContent;
	private string EditorHTMLContent;
	private bool EditorEnabled = true;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		await JSRuntime.InvokeAsync<object>(
			"Tefter.removeEditorTextChangeListener", _componentId.ToString());
		_objectRef?.Dispose();
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_objectRef = DotNetObjectReference.Create(this);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JSRuntime.InvokeAsync<object>(
				"Tefter.createQuill", divEditorElement, _componentId.ToString(), _objectRef, "OnEditorChange");
			_editorInited = true;
		}
	}

	protected override bool ShouldRender() => _value != Value;
	

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if(_editorInited && Value != _value){ 
			_value = Value;
			await JSRuntime.InvokeAsync<object>("Tefter.setQuillHtml", _componentId.ToString(), _value);
		}
	}

	private async Task _valueChanged(string value)
	{
		inputThrottleCancalationToken.Cancel();
		inputThrottleCancalationToken = new();
		await Task.Delay(500, inputThrottleCancalationToken.Token).ContinueWith(
		async (task) =>
		{
			await ValueChanged.InvokeAsync(value);
		}, inputThrottleCancalationToken.Token);
	}

	[JSInvokable("OnEditorChange")]
	public async Task OnEditorChange()
	{
		var content = await JSRuntime.InvokeAsync<string>(
					"Tefter.getQuillHtml", _componentId.ToString());
		await _valueChanged(content);
	}
}