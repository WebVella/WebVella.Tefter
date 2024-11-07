namespace WebVella.Tefter.Web.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.Net;

[ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
public class RepositoryController : ControllerBase
{
	private readonly ITfRepositoryService _repoService;

	public RepositoryController(
		ITfRepositoryService repoService)
	{
		_repoService = repoService;
	}


	[HttpGet]
	[Route("/fs/repository/{filename}")]
	public IActionResult Download([FromRoute] string filename)
	{
		if (string.IsNullOrWhiteSpace(filename))
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}


		var file = _repoService.GetFile(filename).Value;

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

		var extension = Path.GetExtension(filename).ToLowerInvariant();

		new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

		Stream fileContentStream = _repoService.GetFileContentAsFileStream(filename).Value;
		return File(fileContentStream, mimeType);
	}



}