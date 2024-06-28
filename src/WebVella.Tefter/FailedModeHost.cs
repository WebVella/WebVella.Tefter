namespace WebVella.Tefter;

public class FailedModeHost
{
	public static void CreateAndRun(Exception ex, string[] args)
	{
		try
		{
			Serilog.Log.Information("TEFTER: Application enter FAIL_TO_START mode");

			var app = WebApplication.CreateBuilder(args).Build();

			app.MapGet("/{*url}", () => GenerateFailedHtmlResponse(ex));

			app.Run();
		}
		finally
		{
			Serilog.Log.Information("TEFTER: Application exit FAIL_TO_START mode");
		}
	}

	static string GenerateFailedHtmlResponse(Exception ex)
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("TEFTER FAILED TO START NORMAL");

		if (ex is DatabaseUpdateException)
		{
			var dbUpdateException = ex as DatabaseUpdateException;
			sb.AppendLine("========== MIGRATION DATABASE UPDATE FAILED ============");

			foreach (var log in dbUpdateException.Result.Log)
			{
				sb.AppendLine($"STATEMENT: {log.Statement} - SUCCESS: {log.Success}");
			}
		}
		else
		{
			sb.AppendLine("========== UNEXPECTED ERROR OCCURRED ============");
			sb.AppendLine($"{ex.Message}");
			sb.AppendLine($"{ex.StackTrace}");
		}

		return sb.ToString();
	}

}
