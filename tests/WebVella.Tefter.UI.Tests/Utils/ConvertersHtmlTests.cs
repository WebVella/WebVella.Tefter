using FluentAssertions;
using System;
using WebVella.Tefter.Web.Store;
using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.UI.Tests.Utils;
public class ConvertersHtmlTests
{
	[Fact]
	public void ConvertPlainTextToHtml_Tests()
	{
		//Given
		string input = null;
		string output = null;
		string expected = null;

		{ 
			output = TfConverters.ConvertPlainTextToHtml(input);
			output.Should().Be(expected);
		}
		{ 
			input = "";
			expected = "";
			output = TfConverters.ConvertPlainTextToHtml(input);
			output.Should().Be(expected);
		}
		{ 
			input = "text";
			expected = "<p>text</p>\n";
			output = TfConverters.ConvertPlainTextToHtml(input);
			output.Should().Be(expected);
		}
		{ 
			input = $"line1{Environment.NewLine}line2{Environment.NewLine}another line here too.";
			expected = "<p>line1\nline2\nanother line here too.</p>\n";
			output = TfConverters.ConvertPlainTextToHtml(input);
			output.Should().Be(expected);
		}
	}

	[Fact]
	public void ConvertHtmlToText_Tests()
	{
		//Given
		string input = null;
		string output = null;
		string expected = null;

		{ 
			output = TfConverters.ConvertHtmlToPlainText(input);
			output.Should().Be(expected);
		}
		{ 
			input = "";
			expected = "";
			output = TfConverters.ConvertHtmlToPlainText(input);
			output.Should().Be(expected);
		}
		{ 
			input = "<div>text</div>";
			expected = "text";
			output = TfConverters.ConvertHtmlToPlainText(input);
			output.Should().Be(expected);
		}
		{ 
			input = "<p>text</p>";
			expected = $"{Environment.NewLine}text";
			output = TfConverters.ConvertHtmlToPlainText(input);
			output.Should().Be(expected);
		}
		{ 
			input = "<span>text</span>";
			expected = "text";
			output = TfConverters.ConvertHtmlToPlainText(input);
			output.Should().Be(expected);
		}
		{ 
			input = "<a href='#'>text</a>";
			expected = "<#>text";
			output = TfConverters.ConvertHtmlToPlainText(input);
			output.Should().Be(expected);
		}
	}

}
