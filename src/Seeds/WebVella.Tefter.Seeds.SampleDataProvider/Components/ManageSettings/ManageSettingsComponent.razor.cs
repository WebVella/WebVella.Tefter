using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Text.Json;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.Seeds.SampleDataProvider.Components;

public partial class ManageSettingsComponent : TfFormBaseComponent,
	ITfRegionComponent<TfDataProviderManageSettingsComponentContext>
{
	public Guid Id { get; init; } = new Guid("83b22c00-34d3-4883-af5b-7f4a4cd46f4c");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample Data Provider Manage Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(SampleDataProvider),null)
	};
	[Parameter] public TfDataProviderManageSettingsComponentContext Context { get; init; }

	private string _advancedSettings
	{
		get
		{
			if (_form.AdvancedSetting is null) return JsonSerializer.Serialize(new SampleDataProviderSettingsAdvanced());
			return JsonSerializer.Serialize(_form.AdvancedSetting, new JsonSerializerOptions { WriteIndented = true });
		}
	}

	private SampleDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleDataProviderSettings>(Context.SettingsJson);
		Context.SetValidate(_validate);
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleDataProviderSettings>(Context.SettingsJson);
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();

		if (String.IsNullOrWhiteSpace(_form.SampleSetting))
		{
			errors.Add(new ValidationError(nameof(SampleDataProviderSettings.SampleSetting), LOC("required")));
		}
	
		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}
		EditContext.Validate();
		StateHasChanged();
		EditContext.Validate();
		return errors;
	}

	private async Task _changeAdvancedSettings(string value)
	{
		try
		{
			_form.AdvancedSetting = JsonSerializer.Deserialize<SampleDataProviderSettingsAdvanced>(value);
			await _valueChanged();
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
	}

	private async Task _valueChanged()
	{
		Context.SettingsJson = JsonSerializer.Serialize(_form);
		await Context.SettingsJsonChanged.InvokeAsync(Context.SettingsJson);
	}
}