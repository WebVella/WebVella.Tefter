using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.MsSql.Components;
[LocalizationResource("WebVella.Tefter.DataProviders.MsSql.Components.ManageSettings.ManageSettingsComponent", "WebVella.Tefter.DataProviders.MsSql")]
public partial class ManageSettingsComponent : TfFormBaseComponent,
	ITfRegionComponent<TfDataProviderManageSettingsComponentContext>
{
	public Guid Id { get; init; } = new Guid("b543421c-c77f-45d9-8c81-ac0c5a0a303f");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "MSSQL Data Provider Manage Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(MsSqlDataProvider),null)
	};
	[Parameter] public TfDataProviderManageSettingsComponentContext Context { get; init; }

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