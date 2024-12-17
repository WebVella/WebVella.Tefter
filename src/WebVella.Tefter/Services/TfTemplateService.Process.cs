namespace WebVella.Tefter.Services;

public partial interface ITfTemplateService
{
	public Result<ITfTemplateResult> GenerateTemplateResult(
		Guid templateId,
		TfDataTable data);

	public Result ProcessTemplateResult(
		Guid templateId,
		ITfTemplateResult result);
}

internal partial class TfTemplateService : ITfTemplateService
{
	public Result<ITfTemplateResult> GenerateTemplateResult(
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
		ITfTemplateResult result)
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
