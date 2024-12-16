namespace WebVella.Tefter.Tests;

using WebVella.Tefter.Templates.Models;
using WebVella.Tefter.Templates.Services;
using WebVella.Tefter.Templates.TemplateProcessors;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

public partial class WordTemplatesTests
{
	protected static readonly AsyncLock locker = new AsyncLock();

	static WordTemplatesTests()
	{

	}

	[Fact]
	public async Task LoadWordFile()
	{
		using (await locker.LockAsync())
		{
			var path = Path.Combine(Environment.CurrentDirectory, "Files\\template2.docx");
			WordprocessingDocument wordprocessingDocument =
					WordprocessingDocument.Open(path, true);
			Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
		}
	}
}
