namespace WebVella.Tefter.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.Net;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
public class RepositoryController : ControllerBase
{
	private readonly ITfService _tfService;

	public RepositoryController(
		ITfService tfService)
	{
		_tfService = tfService;
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

		var file = _tfService.GetRepositoryFile(filename);

		if (file == null)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return NotFound();
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

		HttpContext.Response.Headers.Remove("last-modified");
		HttpContext.Response.Headers.Append("last-modified", file.LastModifiedOn.ToString(cultureInfo));

		const int durationInSeconds = 60 * 60 * 24 * 30; //30 days caching of these resources

		HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;

		var extension = Path.GetExtension(filename).ToLowerInvariant();

		new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

		Stream fileContentStream = _tfService.GetRepositoryFileContentAsFileStream(filename);

		if (string.IsNullOrWhiteSpace(mimeType))
			mimeType = "text/plain";

		return File(fileContentStream, mimeType);
	}

	[HttpGet]
	[Route("/fs/blob/{blobId}/{fileName}")]
	public IActionResult Download([FromRoute] Guid blobId, [FromRoute] string filename)
	{
		if (blobId == Guid.Empty || string.IsNullOrWhiteSpace(filename))
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return NotFound();
		}

		Stream stream = null;

		if (_tfService.ExistsBlob(blobId, true))
		{
			stream = _tfService.GetBlobStream(blobId, true);
		}
		else if (_tfService.ExistsBlob(blobId, false))
		{
			stream = _tfService.GetBlobStream(blobId, false);
		}
		else
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return NotFound();
		}

		if (stream is null)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return NotFound();
		}

		string headerModifiedSince = Request.Headers["If-Modified-Since"];

		var cultureInfo = new CultureInfo("en-US");

		const int durationInSeconds = 60 * 60 * 24 * 30; //30 days caching of these resources

		HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;

		var extension = Path.GetExtension(filename).ToLowerInvariant();

		new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

		if (string.IsNullOrWhiteSpace(mimeType))
			mimeType = "text/plain";

		return File(stream, mimeType);
	}


}