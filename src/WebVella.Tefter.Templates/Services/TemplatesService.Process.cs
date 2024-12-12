namespace WebVella.Tefter.Templates.Services;

public partial interface ITemplatesService
{
	public Result<ITemplateResult> GenerateTemplateResult(
		Guid templateId,
		TfDataTable data);

	public Result ProcessTemplateResult(
		Guid templateId,
		ITemplateResult result);
}

internal partial class TemplatesService : ITemplatesService
{
	public Result<ITemplateResult> GenerateTemplateResult(
		Guid templateId,
		TfDataTable data)
	{
		try
		{
			var templateResult = GetTemplate(templateId);
			if (!templateResult.IsSuccess || templateResult.Value == null)
			{
				return Result.Fail(new Error("Failed to get template " +
					"for specified template identifier")
						.CausedBy(templateResult.Errors));

			}

			var template = templateResult.Value;

			

			return null;
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to generate template result").CausedBy(ex));
		}
	}

	public Result ProcessTemplateResult(
		Guid templateId,
		ITemplateResult result)
	{
		try
		{
			return Result.Ok();
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to process template result").CausedBy(ex));
		}
	}
}
