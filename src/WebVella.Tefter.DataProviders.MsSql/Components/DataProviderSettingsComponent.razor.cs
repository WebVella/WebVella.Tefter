using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.MsSql.Components;

public partial class DataProviderSettingsComponent : TfFormBaseComponent, ITfCustomComponent
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;

	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private MsSqlDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Value) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(Value);
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Value != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Value) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(Value);
			base.InitForm(_form);
		}
	}

	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		if (String.IsNullOrWhiteSpace(_form.ConnectionString))
		{
			errors.Add(new ValidationError(nameof(MsSqlDataProviderSettings.ConnectionString), LOC("required")));
		}

		if (String.IsNullOrWhiteSpace(_form.SqlQuery))
		{
			errors.Add(new ValidationError(nameof(MsSqlDataProviderSettings.SqlQuery), LOC("required")));
		}

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Reason);
		}
		var isValid = EditContext.Validate();

		StateHasChanged();

		return errors;
	}

	private async Task _valueChanged()
	{
		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}
}