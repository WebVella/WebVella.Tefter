namespace WebVella.Tefter.Web.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net;

[Route("api")]
[ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
public class ApiExcelController : ControllerBase
{
	private readonly ITfFileManager _fileManager;
	internal readonly ExportUseCase UC;
	public ApiExcelController(
		ExportUseCase uc,
		ITfFileManager fileManager)
	{
		UC = uc;
		_fileManager = fileManager;
	}

	[Route("export/{exportType}")]
	[HttpPost, HttpGet]
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


	[HttpGet]
	[Route("/fs/{*filePath}")]
	public IActionResult Download([FromRoute] string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}


		var file = _fileManager.FindFile(filePath).Value;

		if (file == null)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}
		
		string headerModifiedSince = Request.Headers["If-Modified-Since"];

		if (headerModifiedSince != null)
		{
			if (DateTime.TryParse(headerModifiedSince, out DateTime isModifiedSince))
			{
				if (isModifiedSince <= file.LastModifiedOn)
				{
					Response.StatusCode = 304;
					return new EmptyResult();
				}
			}
		}
		var cultureInfo = new CultureInfo("en-US");
		
		HttpContext.Response.Headers.Add("last-modified", file.LastModifiedOn.ToString(cultureInfo));
		
		const int durationInSeconds = 60 * 60 * 24 * 30; //30 days caching of these resources
		
		HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;

		var extension = Path.GetExtension(filePath).ToLowerInvariant();
		
		new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

		Stream fileContentStream = _fileManager.GetFileContentAsFileStream(file).Value;
		return File( fileContentStream, mimeType);
	}



}