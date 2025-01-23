namespace WebVella.Tefter.Assets.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Net;

[ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
public class AssetsController : ControllerBase
{
	private readonly IAssetsService _assetsService;
	private readonly ITfBlobManager _blobManager;

	public AssetsController(
		IAssetsService assetsService,
		ITfBlobManager blobManager)
	{
		_assetsService = assetsService;
		_blobManager = blobManager;
	}


	[HttpGet]
	[Route("/fs/assets/{assetId}/{filename}")]
	public IActionResult Download(
		[FromRoute] Guid assetId,
		[FromRoute] string filename)
	{
		
		if (string.IsNullOrWhiteSpace(filename) || 
			assetId == Guid.Empty )
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}


		var assetResult = _assetsService.GetAsset(assetId);
		if(!assetResult.IsSuccess)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			return new JsonResult(new { });
		}

		var asset = assetResult.Value;

		if (asset is null || asset.Type != AssetType.File )
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}

		var fileAssetContent = ((FileAssetContent)asset.Content);

		if(fileAssetContent.Filename.ToLowerInvariant()  != filename.ToLowerInvariant() )
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}


		string headerModifiedSince = Request.Headers["If-Modified-Since"];

		if (headerModifiedSince != null)
		{
			if (DateTime.TryParse(headerModifiedSince, out DateTime isModifiedSince))
			{
				if (isModifiedSince <= asset.ModifiedOn)
				{
					Response.StatusCode = 304;
					return new EmptyResult();
				}
			}
		}

		
		if(!_blobManager.ExistsBlob(fileAssetContent.BlobId).Value)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return new JsonResult(new { });
		}

		var cultureInfo = new CultureInfo("en-US");

		HttpContext.Response.Headers.Add("last-modified", asset.ModifiedOn.ToString(cultureInfo));

		const int durationInSeconds = 60 * 60 * 24 * 30; //30 days caching of these resources

		HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;

		var extension = Path.GetExtension(filename).ToLowerInvariant();

		new FileExtensionContentTypeProvider().Mappings.TryGetValue(extension, out string mimeType);

		Stream fileContentStream = _blobManager.GetBlobStream(fileAssetContent.BlobId).Value;

		return File(fileContentStream, mimeType);
	}



}