using Boz.Tefter.Components;

namespace Boz.Tefter.Columns;
public partial class BozCommentColumn : TfBaseColumn
{
	private int _value = 0;
	private IDialogReference? _dialog;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
		if(firstRender){
            _value = 0;
            if (Data is not null && Data.Fields is not null && Data.Fields.ContainsKey(Meta.ColumnName))
            {
                if (Data.Fields[Meta.ColumnName].Type == DataFieldType.Number)
                {
                    _value = (int)(decimal)Data.Fields[Meta.ColumnName].Value;
                }
            }
			StateHasChanged();
        }
    }

    private async Task OpenPanelRightAsync()
	{
		if (_dialog is not null)
		{
			await _dialog.CloseAsync();
		}

		DialogParameters<string> parameters = new()
		{
			Content = "data 1",
			Title = $"Hello 123",
			Alignment = HorizontalAlignment.Right,
			PrimaryAction = "Maybe",
			SecondaryAction = "Cancel",
			Width = "800px",
		};
		_dialog = await DialogService.ShowPanelAsync<BozCommentPanel>("test", parameters);

		DialogResult result = await _dialog.Result;
		await HandlePanelAsync(result);
	}

	private async Task HandlePanelAsync(DialogResult result)
	{
		_dialog = null;
		//if (result.Cancelled)
		//{
		//    await Task.Run(() => DemoLogger.WriteLine($"Panel cancelled"));
		//    return;
		//}

		//if (result.Data is not null)
		//{
		//    var simplePerson = result.Data as SimplePerson;
		//    await Task.Run(() => DemoLogger.WriteLine($"Panel closed by {simplePerson?.Firstname} {simplePerson?.Lastname} ({simplePerson?.Age})"));
		//    return;
		//}

		//await Task.Run(() => DemoLogger.WriteLine($"Panel closed"));
	}

}