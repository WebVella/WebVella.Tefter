namespace WebVella.Tefter.Tests;

using System.Runtime.InteropServices;
using WebVella.Tefter.Models;
using WebVella.Tefter.Utility;

public partial class HtmlTemplatesTests : TemplateTagTestsBase
{
	protected static readonly AsyncLock locker = new AsyncLock();


	#region << Arguments >>
	[Fact]
	public async Task ShouldHaveDataSource()
	{
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			Func<bool> action = () => { result.ProcessHtmlTemplate(null); return true; };
			action.Should().Throw<Exception>("No datasource provided!");
		}
	}
	[Fact]
	public async Task ShouldProcessEmpty()
	{
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			var data = SampleData.NewTable(new int[]{ 1 });
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();
			result.ResultHtml.Should().Be(result.TemplateHtml);
		}
	}
	#endregion

	#region << Plain text >>

	[Fact]
	public async Task Text_NoTag()
	{
		var text = "test";
		var data = SampleData.NewTable(new int[]{ 1 });
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			result.TemplateHtml = text;
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();

			result.ResultHtml.Should().NotBeNullOrWhiteSpace();
			result.ResultHtml.Should().Be(result.TemplateHtml);

			
		}
	}

	[Fact]
	public async Task Text_NoTag2()
	{
		var text = "test<br>test2";
		var data = SampleData.NewTable(new int[]{ 1 });
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			result.TemplateHtml = text;
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();

			result.ResultHtml.Should().NotBeNullOrWhiteSpace();
			result.ResultHtml.Should().Be(result.TemplateHtml);

			
		}
	}
	[Fact]
	public async Task Text_NoTag3()
	{
		var text = "test<br>{{name[0]}}";
		var data = SampleData;
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			result.TemplateHtml = text;
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();

			result.ResultHtml.Should().NotBeNullOrWhiteSpace();
			result.ResultHtml.Should().Be("test<br>item1");

			
		}
	}
	#endregion

	#region << Plain Div >>

	[Fact]
	public async Task Text_Div1()
	{
		var text = "<div>test</div><div>{{name}}</div>";
		var data = SampleData.NewTable(new int[]{ 0,1 });
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			result.TemplateHtml = text;
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();

			result.ResultHtml.Should().NotBeNullOrWhiteSpace();
			result.ResultHtml.Should().Be("<div>test</div><div>item1</div><div>item2</div>");

			
		}
	}

	[Fact]
	public async Task Text_Div2()
	{
		var text = "<div>test</div><div><div>{{name}}</div></div>";
		var data = SampleData.NewTable(new int[]{ 0,1 });
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			result.TemplateHtml = text;
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();

			result.ResultHtml.Should().NotBeNullOrWhiteSpace();
			result.ResultHtml.Should().Be("<div>test</div><div><div>item1</div></div><div><div>item2</div></div>");

			
		}
	}

	#endregion

	#region << Plain Paragraph >>

	[Fact]
	public async Task Text_Paragraph1()
	{
		var text = "<p>test</p><p>{{name}}</p>";
		var data = SampleData.NewTable(new int[]{ 0,1 });
		using (await locker.LockAsync())
		{
			var result = new TfHtmlTemplateProcessResult();
			result.TemplateHtml = text;
			Func<bool> action = () => { result.ProcessHtmlTemplate(data); return true; };
			action.Should().NotThrow();

			result.ResultHtml.Should().NotBeNullOrWhiteSpace();
			result.ResultHtml.Should().Be("<p>test</p><p>item1</p><p>item2</p>");

			
		}
	}
	#endregion


}
