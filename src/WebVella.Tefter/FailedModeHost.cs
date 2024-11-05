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

		if (ex.InnerException is TfDatabaseUpdateException)
		{
			var dbUpdateException = ex.InnerException as TfDatabaseUpdateException;
			sb.AppendLine("========== MIGRATION DATABASE UPDATE FAILED ============");

			if(dbUpdateException.Result.Log.Any(x=>x.Success))
				sb.AppendLine("========== SUCCEEDED ============");

			foreach (var log in dbUpdateException.Result.Log)
			{
				var sqlStatement = log.Statement;

				//cut comments on sql
				var indexOfComment = log.Statement.IndexOf("COMMENT ON");
				if(indexOfComment != -1)
					sqlStatement = sqlStatement.Substring(0, indexOfComment);
				
				if(log.Success)
					sb.AppendLine($"STATEMENT: {sqlStatement}");
				else
				{
					sb.AppendLine("========== FAILED ============");
					sb.AppendLine($"STATEMENT: {sqlStatement}");
					sb.AppendLine($"SQL ERROR: {log.SqlError}");
				}
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
