using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.DataProviders.MsSql.Addons;
public partial class ManageSettingsComponent : TfFormBaseComponent,	ITfScreenRegionComponent<TfDataProviderManageSettingsScreenRegionContext>
{
	public const string ID = "b543421c-c77f-45d9-8c81-ac0c5a0a303f";
	public const string NAME = "MSSQL Data Provider Manage Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(MsSqlDataProvider),null)
	};
	[Parameter] public TfDataProviderManageSettingsScreenRegionContext RegionContext { get; set; }

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
		EditContext.Validate();

		StateHasChanged();

		return errors;
	}

	private async Task _valueChanged()
	{
		RegionContext.SettingsJson = JsonSerializer.Serialize(_form);
		await RegionContext.SettingsJsonChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}
}