namespace WebVella.Tefter.Tests;
using WebVella.Tefter.Models;
using WebVella.Tefter.Utility;

public partial class TextTemplatesTests : TemplateTagTestsBase
{
	protected static readonly AsyncLock locker = new AsyncLock();


	#region << Arguments >>
	[Fact]
	public async Task ShouldHaveDataSource()
	{
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			Func<bool> action = () => { result.ProcessTextTemplate(null); return true; };
			action.Should().Throw<Exception>("No datasource provided!");
		}
	}
	#endregion

	#region << Data >>
	[Fact]
	public async Task Template1_Repeat()
	{
		var fileName = "Template1.txt";
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			result.TemplateText = _loadText(fileName);
			Func<bool> action = () => { result.ProcessTextTemplate(SampleData); return true; };
			action.Should().NotThrow();

			result.ResultText.Should().NotBeNullOrWhiteSpace();
			var lines = _getLines(result.ResultText);
			lines.Should().HaveCount(10);
			for (var i = 0; i < 5; i++)
				lines[i].Should().Be(SampleData.Rows[i]["position"]?.ToString());	

			for (var i = 0; i < 5; i++)
				lines[5 + i].Should().Be(SampleData.Rows[i]["name"]?.ToString());	

			_saveText(result.ResultText,fileName);
		}
	}
	[Fact]
	public async Task Template2_Repeat()
	{
		var fileName = "Template2.txt";
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			result.TemplateText = _loadText(fileName);
			Func<bool> action = () => { result.ProcessTextTemplate(SampleData); return true; };
			action.Should().NotThrow();

			result.ResultText.Should().NotBeNullOrWhiteSpace();
			var lines = _getLines(result.ResultText);
			lines.Should().HaveCount(5);
			for (var i = 0; i < 5; i++)
				lines[i].Should().StartWith(SampleData.Rows[i]["position"]?.ToString());	
			_saveText(result.ResultText,fileName);
		}
	}

	[Fact]
	public async Task Template3_SingleRow()
	{
		var fileName = "Template3.txt";
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			result.TemplateText = _loadText(fileName);
			var oneRowDT = new TfDataTable(SampleData,0);
			Func<bool> action = () => { result.ProcessTextTemplate(oneRowDT); return true; };
			action.Should().NotThrow();

			result.ResultText.Should().NotBeNullOrWhiteSpace();
			var lines = _getLines(result.ResultText);
			lines.Should().HaveCount(1);
			lines[0].Should().StartWith(SampleData.Rows[0]["position"]?.ToString());	
			_saveText(result.ResultText,fileName);
		}
	}

	[Fact]
	public async Task Template4_AllIndexed()
	{
		var fileName = "Template4.txt";
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			result.TemplateText = _loadText(fileName);
			Func<bool> action = () => { result.ProcessTextTemplate(SampleData); return true; };
			action.Should().NotThrow();

			result.ResultText.Should().NotBeNullOrWhiteSpace();
			var lines = _getLines(result.ResultText);
			lines.Should().HaveCount(1);
			lines[0].Should().StartWith(SampleData.Rows[0]["position"]?.ToString());	
			_saveText(result.ResultText,fileName);
		}
	}

	[Fact]
	public async Task Template5_AllIndexedWithNotPresentIndexShouldNotFail()
	{
		var fileName = "Template5.txt";
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			result.TemplateText = _loadText(fileName);
			var oneRowDT = new TfDataTable(SampleData,0);
			Func<bool> action = () => { result.ProcessTextTemplate(oneRowDT); return true; };
			action.Should().NotThrow();
			var lines = _getLines(result.ResultText);
			lines.Should().HaveCount(1);
			lines[0].Should().StartWith("{{position[3]}}");
		}
	}

	[Fact]
	public async Task Template6_AllIndexedWithNotPresentIndexShouldNotFail()
	{
		var fileName = "Template6.txt";
		using (await locker.LockAsync())
		{
			var result = new TfTextTemplateProcessResult();
			result.TemplateText = _loadText(fileName);
			var oneRowDT = new TfDataTable(SampleData,0);
			Func<bool> action = () => { result.ProcessTextTemplate(oneRowDT); return true; };
			action.Should().NotThrow();
			var lines = _getLines(result.ResultText);
			lines.Should().HaveCount(1);
			lines[0].Should().StartWith("{{position[3]}}");
		}
	}

	#endregion
	#region << Private >>
	private string _loadText(string fileName)
	{
		var path = Path.Combine(Environment.CurrentDirectory, $"Files\\{fileName}");
		var fi = new FileInfo(path);
		fi.Exists.Should().BeTrue();
		StreamReader sr = new StreamReader(path);
		var templateWB = sr.ReadToEnd();
		templateWB.Should().NotBeNull();
		sr.Close();
		return templateWB;
	}
	private void _saveText(string content, string fileName)
	{
		DirectoryInfo debugFolder = Directory.GetParent(Environment.CurrentDirectory);
		var projectFolder = debugFolder.Parent.Parent;

		var path = Path.Combine(projectFolder.FullName, $"FileResults\\result-{fileName}");
		StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII);
		sw.Write(content);
		sw.Close();
	}

	private List<string> _getLines(string content)
	 { 
		if(string.IsNullOrEmpty(content)) return new List<string>();
		return content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
	 }

	#endregion
}
