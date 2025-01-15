namespace WebVella.Tefter.Website.Tests;

using FluentAssertions;
using WebVella.Tefter.Website.Models;
using WebVella.Tefter.Website.Utils;
public class UrlHelperTests
{
	[Fact]
	public void GenerateLocalizedUrl_1()
	{
		var url = "/terms";
		var culture = WvConstants.DefaultLang.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be(url);
	}

	[Fact]
	public void GenerateLocalizedUrl_2()
	{
		var url = "/terms/test";
		var culture = WvConstants.DefaultLang.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be(url);
	}

	[Fact]
	public void GenerateLocalizedUrl_13()
	{
		var url = "/en/terms";
		var culture = CookieLang.Eng.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be($"/en/terms");
	}

	[Fact]
	public void GenerateLocalizedUrl_3()
	{
		var url = "/en/terms";
		var culture = CookieLang.Bg.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be($"/bg/terms");
	}

	[Fact]
	public void GenerateLocalizedUrl_4()
	{
		var url = "/bg/terms";
		var culture = CookieLang.Eng.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be($"/en/terms");
	}

	[Fact]
	public void GenerateLocalizedUrl_5()
	{
		var url = "";
		var culture = WvConstants.DefaultLang.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be("");
	}

	[Fact]
	public void GenerateLocalizedUrl_6()
	{
		var url = "/";
		var culture = WvConstants.DefaultLang.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be("/");
	}

	[Fact]
	public void GenerateLocalizedUrl_7()
	{
		var url = "/";
		var culture = CookieLang.Bg.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be("/bg");
	}

	[Fact]
	public void GenerateLocalizedUrl_8()
	{
		var url = "/bg";
		var culture = CookieLang.Eng.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture);
		result.Should().Be("/");
	}

	[Fact]
	public void GenerateLocalizedUrl_81()
	{
		var url = "https://domain.com/en/terms";
		var culture = WvConstants.DefaultLang.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture,true);
		result.Should().Be(url);
	}
	[Fact]
	public void GenerateLocalizedUrl_82()
	{
		var url = "https://domain.com/en/terms";
		var culture = CookieLang.Bg.GetCulture();
		var result = UrlHelper.GenerateLocalizedUrl(url,culture,true);
		result.Should().Be("https://domain.com/bg/terms");
	}
}
