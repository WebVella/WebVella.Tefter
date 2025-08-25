using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace TfResourceTranslator;

internal class Program
{
	static readonly HttpClient _client = new(new HttpClientHandler());
	static async Task Main(string[] args)
	{
		try
		{
			// Read configuration from file
			var config = ReadConfiguration();
			if (config == null)
			{
				Console.WriteLine("Failed to read configuration file.");
				return;
			}
			var resxDict = ReadResxKeys(config.ResxFilePath);
			foreach (var key in resxDict.Keys)
			{
				if (!config.Regenerate && !String.IsNullOrWhiteSpace(resxDict[key])) continue;
				resxDict[key] = await GetChatAsync(config.SystemMessage, key, config);
			}
			UpdateTargetResx(config.ResxFilePath, resxDict, config);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();
	}

	static Config ReadConfiguration()
	{
		try
		{
			string configPath = "config.json";
			if (!File.Exists(configPath))
			{
				// Create default configuration file
				var defaultConfig = new Config
				{
					ResxFilePath = "C:\\source\\WebVella.Tefter\\src\\WebVella.Tefter\\Resources\\SharedResource.bg-BG.resx",
					CultureCode = "bg-BG",
					Port = 1234
				};

				string jsonString = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(configPath, jsonString);
				Console.WriteLine($"Created default configuration file: {configPath}");
				Console.WriteLine("Please update the paths in config.json and run again.");
				return null;
			}
			else
			{
				string jsonString = File.ReadAllText(configPath);
				var config = JsonSerializer.Deserialize<Config>(jsonString);
				var culture = new CultureInfo(config.CultureCode);
				config.SystemMessage = String.Format("You are a language translator for a software product localization resource file. " +
				"You get each user message that is in English, and translate it in {0}. " +
				"You should do only direct translation of the messages and nothing more. " +
				"If translation cannot be done, return empty string. " +
				"The product allows team collaboration around data from data providers and user spaces that how different pages that present the data and allow collaboration.",
				culture.EnglishName);

				return config;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error reading configuration: {ex.Message}");
			return null;
		}
	}
	static async Task<string> GetChatAsync(string systemMessage, string userMessage, Config config)
	{
		var requestBody = new
		{
			String.Empty,//for the current
			messages = new[]
			{
				new { role = "system", content = systemMessage },
				new { role = "user",   content = userMessage }
			},
			temperature = 0.7f,
			max_tokens = 200,
			stream = false
		};

		var json = JsonSerializer.Serialize(requestBody);
		var response = await _client.PostAsync(
			$"http://localhost:{config.Port}/v1/chat/completions",
			new StringContent(json, System.Text.Encoding.UTF8, "application/json")
		);

		response.EnsureSuccessStatusCode();

		var stream = await response.Content.ReadAsStreamAsync();
		using var reader = new StreamReader(stream);
		string jsonResponse = await reader.ReadToEndAsync();

		// The response is an object with `choices[0].message.content`
		var chatResp = JsonSerializer.Deserialize<ChatCompletionResponse>(jsonResponse);

		return chatResp.Choices[0].Message.Content;
	}
	static Dictionary<string, string> ReadResxKeys(string resxFilePath)
	{
		var keyValueDictionary = new Dictionary<string, string>();

		try
		{
			var doc = new XmlDocument();
			doc.Load(resxFilePath);

			XmlNodeList dataNodes = doc.SelectNodes("//data");
			if (dataNodes != null)
			{
				foreach (XmlNode node in dataNodes)
				{
					XmlAttribute keyAttribute = node.Attributes?["name"];
					if (keyAttribute != null && !string.IsNullOrEmpty(keyAttribute.Value))
					{
						string key = keyAttribute.Value;

						XmlNode valueNode = node.SelectSingleNode("value");
						string value = valueNode?.InnerText ?? string.Empty;

						keyValueDictionary[key] = value;
					}
				}
			}
			return keyValueDictionary;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error reading resx file {resxFilePath}: {ex.Message}");
		}

		return keyValueDictionary;
	}
	static void UpdateTargetResx(string targetResxPath, Dictionary<string, string> keyValueDictionary,Config config)
	{
		var doc = new XmlDocument();
		doc.Load(targetResxPath);

		XmlNodeList dataNodes = doc.SelectNodes("//data");
		if (dataNodes == null) return;

		int updatedCount = 0;
		foreach (XmlNode node in dataNodes)
		{
			XmlAttribute keyAttribute = node.Attributes?["name"];
			if (keyAttribute != null && !string.IsNullOrEmpty(keyAttribute.Value))
			{
				string key = keyAttribute.Value;
				if (keyValueDictionary.TryGetValue(key, out string value))
				{
					XmlNode valueNode = node.SelectSingleNode("value");
					if (valueNode != null)
					{
						valueNode.InnerText = value;
						updatedCount++;
					}
				}
			}
		}

		// Save the modified document
		doc.Save(targetResxPath);
		Console.WriteLine($"Updated {updatedCount} entries in target RESX file");
	}
}

// Configuration class
class Config
{
	public string CultureCode { get; set; }
	public int Port { get; set; } = 8080;
	public string ResxFilePath { get; set; }
	public string SystemMessage { get; set; }
	public bool Regenerate { get; set; } = false;
}
record ModelsResponse(string Object, List<ModelInfo> Data);
record ModelInfo(string Id, string Object, DateTime Created, Dictionary<string, string> Root);

class ChatCompletionResponse
{
	[JsonPropertyName("choices")]
	public Choice[] Choices { get; set; }
}

class Choice
{
	[JsonPropertyName("message")]
	public Message Message { get; set; }
}

class Message
{
	[JsonPropertyName("content")]
	public string Content { get; set; }
}