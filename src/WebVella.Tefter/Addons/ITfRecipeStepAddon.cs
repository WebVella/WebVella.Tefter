namespace WebVella.Tefter;


public interface ITfRecipeStepAddon : ITfAddon
{
	/// <summary>
	/// Initialized by the system during the render
	/// </summary>
	public TfRecipeStepInstance Instance { get; set; }

	/// <summary>
	/// Set by the engine. Used in the recipe management screen.
	/// </summary>
	public Type FormComponent { get; set; }
	
	/// <summary>
	/// Initialized by the system during the render
	/// </summary>
	public ITfRecipeStepAddonData Data { get; set; }

	/// <summary>
	/// Triggered by the service when this step needs to be applied
	/// </summary>
	/// <param name="serviceProvider">provides all injected services</param>
	/// <param name="stepBase">will provide the step model as provided by the form component</param>
	/// <param name="stepResult">result object that needs to be filled in</param>
	/// <returns></returns>
	public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon stepBase, TfRecipeStepResult stepResult);

	/// <summary>
	/// Triggered by the service on exception. It is in database transaction but some steps may need to manually remove stuff
	/// </summary>
	/// <param name="serviceProvider">provides all injected services</param>
	/// <param name="stepBase">will provide the step model as provided by the form component</param>
	/// <param name="stepResult">result object that needs to be filled in</param>
	/// <returns></returns>
	public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon stepBase, TfRecipeStepResult? stepResult);
}

public class TfRecipeStepInstance
{
	/// <summary>
	/// Unique identifier of the step
	/// </summary>
	public Guid StepId { get; set; }

	/// <summary>
	/// The Title of the step in the navigation menu
	/// </summary>
	public string StepMenuTitle { get; set; }

	/// <summary>
	/// Title of the step presented in the step content area
	/// </summary>
	public string StepContentTitle { get; set; }
	/// <summary>
	/// Description of the step presented under the title in the step content area
	/// </summary>
	public string StepContentDescription { get; set; }
	/// <summary>
	/// Is this step visible for the user during the recipe management
	/// </summary>
	public bool Visible { get; set; }
	/// <summary>
	/// Position in the visible steps menu
	/// </summary>
	public int Position { get; set; }
	/// <summary>
	/// is the first visible step
	/// </summary>
	public bool IsFirst { get; set; }
	/// <summary>
	/// is the last visible step
	/// </summary>
	public bool IsLast { get; set; }

	/// <summary>
	/// Set by the engine. Used in the recipe management screen.
	/// </summary>
	public TfRecipeStepFormBase FormComponent { get; set; }
	/// <summary>
	/// Set by the engine. Used in the recipe management screen.
	/// </summary>
	public List<ValidationError> Errors { get; set; } = new();
}

public abstract class TfRecipeStepFormBase : TfFormBaseComponent
{

	/// <summary>
	/// Called when the user submits the step
	/// </summary>
	public abstract void SubmitForm();

}

public interface ITfRecipeStepAddonData
{

}