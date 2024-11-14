namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Editor.TfEditor", "WebVella.Tefter")]
public partial class TfEditor : TfBaseComponent
{
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public string Placeholder { get; set; } = String.Empty;
	[Parameter] public EventCallback OnEnter { get; set; }

	private CancellationTokenSource inputThrottleCancalationToken = new();
	private DotNetObjectReference<TfEditor> _objectRef;
	private Guid _componentId = Guid.NewGuid();

	private string _value = null;
	private bool _editorInited = false;
	private ElementReference divEditorElement;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		try
		{
			await JSRuntime.InvokeAsync<object>("Tefter.removeQuill", _componentId.ToString());
		}
		catch
		{
			//In rare ocasions the item is disposed after the JSRuntime is no longer avaible
		}
		_objectRef?.Dispose();
		await base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_objectRef = DotNetObjectReference.Create(this);
		_value = Value;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var textChangeMethodName = "OnEditorChange";
			if (OnEnter.HasDelegate)
			{
				var onEnterMethodName = "OnEnterHandler";
				var placeHolder = LOC("Shift + Enter for new line. Enter to send.");
				if (!String.IsNullOrWhiteSpace(Placeholder))
					placeHolder = Placeholder + Environment.NewLine + placeHolder;

				await JSRuntime.InvokeAsync<object>(
					"Tefter.createQuill", divEditorElement, _componentId.ToString(), _objectRef, textChangeMethodName, onEnterMethodName, placeHolder);
			}
			else{ 
				await JSRuntime.InvokeAsync<object>(
					"Tefter.createQuill", divEditorElement, _componentId.ToString(), _objectRef, textChangeMethodName, null, Placeholder);			
			}



			_editorInited = true;
		}
	}

	protected override bool ShouldRender() => _value != Value;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (_editorInited && Value != _value)
		{
			_value = Value;
			await JSRuntime.InvokeAsync<object>("Tefter.setQuillHtml", _componentId.ToString(), _value);
		}
	}
	private async Task _valueChanged(string value)
	{
		_value = value;
		await ValueChanged.InvokeAsync(value);
		//BOZ: there is problem with the throttle and the Enter key
		//inputThrottleCancalationToken.Cancel();
		//inputThrottleCancalationToken = new();
		//await Task.Delay(500, inputThrottleCancalationToken.Token).ContinueWith(
		//async (task) =>
		//{
		//	await ValueChanged.InvokeAsync(value);
		//}, inputThrottleCancalationToken.Token);
	}

	public async Task Focus()
	{
		await JSRuntime.InvokeAsync<object>(
			"Tefter.focusQuill", _componentId.ToString());
	}

	[JSInvokable("OnEditorChange")]
	public async Task OnEditorChange()
	{
		var content = await JSRuntime.InvokeAsync<string>(
					"Tefter.getQuillHtml", _componentId.ToString());
		await _valueChanged(content);
	}

	[JSInvokable("OnEnterHandler")]
	public async Task OnEnterHandler()
	{
		Console.WriteLine($"OnEnterHandler {_componentId}");
		await OnEnter.InvokeAsync();
	}
}