namespace WebVella.Tefter.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.Net;

[ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
public class RepositoryController : ControllerBase
{
	private readonly ITfRepositoryService _repoService;
	private readonly ITfBlobManager _blobManager;

	public RepositoryController(
	ITfBlobManager blobManager,
		ITfRepositoryService repoService)
	{
		_blobManager = blobManager;
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

		HttpContext.Response.Headers.Add("last-modified", file.LastModifiedOn.ToString(cultureInfo));

		const int durationInSeconds = 60 * 60 * 24 * 30; //30 days caching of these resources

		HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;

		var extension = Path.GetExtension(filename).ToLowerInvariant();

		new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

		Stream fileContentStream = _repoService.GetFileContentAsFileStream(filename).Value;
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

		if (_blobManager.ExistsBlob(blobId, true))
		{
			stream = _blobManager.GetBlobStream(blobId, true);
		}
		else if (_blobManager.ExistsBlob(blobId, false))
		{
			stream = _blobManager.GetBlobStream(blobId, false);
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

		return File(stream, mimeType);
	}


}