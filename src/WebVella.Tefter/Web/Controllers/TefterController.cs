namespace WebVella.Tefter.Web.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
[Route("api")]
[ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
public class ApiExcelController : ControllerBase
{
	internal readonly ExportUseCase UC;
	public ApiExcelController(
		ExportUseCase uc)
	{
		UC = uc;
	}

	[Route("export/{exportType}")]
	[HttpPost,HttpGet]
	public async Task<ActionResult> ExportHander([FromRoute] string exportType)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(exportType))
				throw new Exception("Export type not defined");

			if (Request.Protocol == "POST" && (!Request.Form.ContainsKey("data") || String.IsNullOrWhiteSpace(Request.Form["data"])))
				throw new Exception("Data Json not defined");



			byte[] bytes = null;
			switch (exportType)
			{
				case "export-view":
					{
						var data = JsonSerializer.Deserialize<TucExportViewData>(Request.Form["data"]);
						bytes = await UC.ExportViewToExcel(data);
					}
					break;
				default:
					throw new Exception("Export type not supported");
			}

			if (bytes == null)
				throw new Exception("No bytes were generated during the export");
			var random = new Random().Next(10, 99);
			DateTime dt = DateTime.Now;
			string time = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + dt.Millisecond.ToString();

			var fileName = $"{exportType}-{time}{random}.xlsx";
			System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
			{
				FileName = fileName,
				Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
			};
			Response.Headers.Append("Content-Type", $"application/vnd.openxmlformats-officedocument.spreadsheetml");
			Response.Headers.Append("Content-Disposition", cd.ToString());
			Response.Headers.Append("X-Content-Type-Options", "nosniff");

			return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml");
		}
		catch (Exception exception)
		{
			return new ContentResult
			{
				Content = $"Error: {exception.Message}",
				ContentType = "text/plain",
				// change to whatever status code you want to send out
				StatusCode = 500
			};
		}
	}


}