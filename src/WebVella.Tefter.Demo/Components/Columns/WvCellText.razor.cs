namespace WebVella.Tefter.Demo.Components;
public partial class WvCellText : WvBaseComponent
{
	[Parameter]
	public SpaceViewColumn Meta { get; set; }

	[Parameter]
	public DataField Data { get; set; }

	[Parameter]
	public Action<(string,object)> ValueChanged { get; set; }

	private string _value = string.Empty;

	protected override void OnInitialized()
	{
		_value = string.Empty;
		if (Data is not null)
		{
			if (Data.Type == DataFieldType.Text)
			{
				_value = Data.Value.ToString();
			}
		}
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{


		}
	}

	private void _clicked(){ 
	
		ValueChanged?.Invoke((Meta.AppColumnName,"boz123"));
	}
}