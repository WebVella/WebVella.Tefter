namespace WebVella.Tefter;

public class TfInstallData 
{
	public Guid RecipeId { get; set; }
	public string RecipeTypeFullName { get; set; }
	public DateTime AppliedOn { get; set; }
	

}

public enum TfInstallResultType{ 
	Success = 0,
	Error = 1,
}