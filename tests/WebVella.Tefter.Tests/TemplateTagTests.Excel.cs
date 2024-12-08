namespace WebVella.Tefter.Tests;
using ClosedXML.Excel;
using WebVella.Tefter.Models;
using WebVella.Tefter.Utility;

public partial class ExcelTemplatesTests : TemplateTagTestsBase
{
	protected static readonly AsyncLock locker = new AsyncLock();

	#region << Arguments >>
	[Fact]
	public async Task ShouldHaveTemplate()
	{
		using (await locker.LockAsync())
		{
			var result = new TfExcelTemplateProcessResult();
			Func<bool> action = () => { result.ProcessExcelTemplate(SampleData); return true;};
			action.Should().Throw<Exception>("No template provided!");
		}
	}

	[Fact]
	public async Task ShouldHaveDataSource()
	{
		using (await locker.LockAsync())
		{
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook("TemplatePlacement1.xlsx");
			Func<bool> action = () => { result.ProcessExcelTemplate(null); return true;};
			action.Should().Throw<Exception>("No datasource provided!");
		}
	}
	#endregion


	#region << Placement >>
	[Fact]
	public async Task Placement1()
	{
		// {{position}} | {{name}}
		using (await locker.LockAsync())
		{
			//Given
			var fileName = "TemplatePlacement1.xlsx";
			var result = new TfExcelTemplateProcessResult();
			result.TemplateWorkbook = _loadWorkbook(fileName);
			result.ProcessExcelTemplatePlacement(SampleData);

			_saveWorkbook(result.ResultWorkbook, fileName);
		}
	}

	#endregion

	#region << Private >>
	private XLWorkbook _loadWorkbook(string fileName)
	{
		var ds = SampleData;
		var path = Path.Combine(Environment.CurrentDirectory, $"Files\\{fileName}");
		var fi = new FileInfo(path);
		fi.Exists.Should().BeTrue();

		var templateWB = new XLWorkbook(path);
		templateWB.Should().NotBeNull();

		return templateWB;
	}
	private void _saveWorkbook(XLWorkbook workbook,string fileName)
	{
		DirectoryInfo debugFolder = Directory.GetParent(Environment.CurrentDirectory);
		var projectFolder = debugFolder.Parent.Parent;

		var path = Path.Combine(projectFolder.FullName, $"FileResults\\{fileName}");
		workbook.SaveAs(path);
	}
	#endregion
}
