using System.ComponentModel.Design;

namespace WebVella.Tefter.UI.Addons;

public partial class TucTextViewColumnTypeEdit : ComponentBase
{
	[Parameter] public List<string?>? Value { get; set; }
	[Parameter] public EventCallback<List<string?>> ValueChanged { get; set; }

	private string _valueInputId = "input-" + Guid.NewGuid();
	private Dictionary<int, string?> _valueDict = new();


	protected override void OnInitialized()
	{
		if (Value is not null)
		{
			for (int i = 0; i < Value.Count; i++)
			{
				_valueDict[i] = Value[i];
			}
		}
	}

	private async Task _valueChanged()
	{
		var values = new List<string?>();
		foreach (var index in _valueDict.Keys)
		{
			values.Add(_valueDict[index]);
		}
		await ValueChanged.InvokeAsync(values);
	}
}