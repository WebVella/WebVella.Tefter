namespace WebVella.Tefter.UI.Components;
public partial class TucEditor : TfBaseComponent,IAsyncDisposable
{
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public string Placeholder { get; set; } = String.Empty;
	[Parameter] public EventCallback OnEnter { get; set; }
	[Parameter] public bool ReadOnly { get; set; } = false;

	[Parameter] public TfEditorSize Size { get; set; } = TfEditorSize.Normal;

	private DotNetObjectReference<TucEditor> _objectRef;
	private Guid _componentId = Guid.NewGuid();

	private string _value = null;
	private bool _editorInited = false;
	private ElementReference divEditorElement;

	private string _style
	{
		get
		{
			var result = new StringBuilder();
			if (Size == TfEditorSize.Large)
			{
				result.Append($"height:200px;");
			}

			return result.ToString();
		}
	}

	public async ValueTask DisposeAsync()
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
				var placeHolder = LOC("Ctrl + Enter to send.");
				if (!String.IsNullOrWhiteSpace(Placeholder))
					placeHolder = Placeholder + Environment.NewLine + placeHolder;

				await JSRuntime.InvokeAsync<object>(
					"Tefter.createQuill", divEditorElement, _componentId.ToString(), _objectRef, textChangeMethodName, onEnterMethodName, placeHolder, ReadOnly);
			}
			else
			{
				await JSRuntime.InvokeAsync<object>(
					"Tefter.createQuill", divEditorElement, _componentId.ToString(), _objectRef, textChangeMethodName, null, Placeholder, ReadOnly);
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
			try
			{
				await JSRuntime.InvokeAsync<object>("Tefter.setQuillHtml", _componentId.ToString(), _value);
			}
			catch (Exception)
			{
				//ignored
			}			
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
		await OnEnter.InvokeAsync();
	}

}

public enum TfEditorSize
{
	Normal = 0,
	Large = 1
}