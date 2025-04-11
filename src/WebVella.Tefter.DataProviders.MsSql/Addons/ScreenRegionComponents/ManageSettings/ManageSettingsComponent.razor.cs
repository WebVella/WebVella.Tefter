using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.MsSql.Addons;
[LocalizationResource("WebVella.Tefter.DataProviders.MsSql.Addons.ScreenRegionComponents.ManageSettings.ManageSettingsComponent", "WebVella.Tefter.DataProviders.MsSql")]
public partial class ManageSettingsComponent : TfFormBaseComponent,	ITfRegionComponent<TfDataProviderManageSettingsScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("b543421c-c77f-45d9-8c81-ac0c5a0a303f");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "MSSQL Data Provider Manage Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(MsSqlDataProvider),null)
	};
	[Parameter] public TfDataProviderManageSettingsScreenRegionContext RegionContext { get; init; }

	private MsSqlDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(RegionContext.SettingsJson);
		RegionContext.SetValidate(_validate);
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(RegionContext.SettingsJson);
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
		RegionContext.SettingsJson = JsonSerializer.Serialize(_form);
		await RegionContext.SettingsJsonChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}
}