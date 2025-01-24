using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.MsSql.Components;

public partial class DataProviderSettingsComponent : TfFormBaseComponent, 
	ITfDynamicComponent<TfDataProviderSettingsComponentContext>, 
	ITfComponentScope<MsSqlDataProvider>
{
	public Guid Id { get; set; } = new Guid("d03dfd93-866c-419d-bca4-fd7dbbec6034");
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfDataProviderSettingsComponentContext Context { get; set; }

	private MsSqlDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(Context.SettingsJson);
		Context.SetValidate(_validate);
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(Context.SettingsJson);
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
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
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		var isValid = EditContext.Validate();

		StateHasChanged();

		return errors;
	}

	private async Task _valueChanged()
	{
		Context.SettingsJson = JsonSerializer.Serialize(_form);
		await Context.SettingsJsonChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}
}