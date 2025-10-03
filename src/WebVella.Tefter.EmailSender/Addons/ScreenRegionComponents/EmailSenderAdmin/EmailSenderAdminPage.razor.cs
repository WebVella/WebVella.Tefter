namespace WebVella.Tefter.EmailSender.Addons;

public partial class EmailSenderAdminPage : TfBaseComponent, ITfScreenRegionComponent<TfAdminPageScreenRegionContext>
{
	public const string ID = "1f6e544e-6a53-4fa1-98ef-9c51a569c2b5";
	public const string NAME = "Email Sender";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Mail";
	public const int POSITION_RANK = 100;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfAdminPageScreenRegionContext RegionContext { get; set; }

}