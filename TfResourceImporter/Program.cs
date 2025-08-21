using System.Text.Json;
using System.Xml;

namespace TfResourceImporter;

internal class Program
{
	static void Main(string[] args)
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

			string basePath = config.FolderPath;
			string targetResxPath = config.ResxFilePath;

			if (string.IsNullOrEmpty(basePath) || string.IsNullOrEmpty(targetResxPath))
			{
				Console.WriteLine("Configuration error: BasePath or TargetResxPath not found in app.config");
				return;
			}

			// Validate paths
			if (!Directory.Exists(basePath))
			{
				Console.WriteLine($"Base path does not exist: {basePath}");
				return;
			}

			if (!File.Exists(targetResxPath))
			{
				Console.WriteLine($"Target RESX file does not exist: {targetResxPath}");
				return;
			}

			// Extract all key-value pairs from RESX files
			var keyValueDictionary = ExtractAllResxValues(basePath, targetResxPath);
			Console.WriteLine($"Extracted {keyValueDictionary.Count} key-value pairs");

			// Update target RESX file
			UpdateTargetResx(targetResxPath, keyValueDictionary);
			Console.WriteLine("RESX file update completed successfully");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();
	}

	static Dictionary<string, string> ExtractAllResxValues(string basePath, string skipPath)
	{
		var keyValueDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Get all RESX files recursively
		var resxFiles = Directory.GetFiles(basePath, "*.resx", SearchOption.AllDirectories);

		foreach (string resxFile in resxFiles)
		{
			if (resxFile == skipPath) continue;

			try
			{
				var doc = new XmlDocument();
				doc.Load(resxFile);

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
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to process {resxFile}: {ex.Message}");
			}
		}

		return keyValueDictionary;
	}

	static void UpdateTargetResx(string targetResxPath, Dictionary<string, string> keyValueDictionary)
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
					FolderPath = @"C:\path\to\your\project",
					ResxFilePath = @"C:\path\to\resources.resx"
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
				return JsonSerializer.Deserialize<Config>(jsonString);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error reading configuration: {ex.Message}");
			return null;
		}
	}
}

// Configuration class
class Config
{
	public string FolderPath { get; set; }
	public string ResxFilePath { get; set; }
}

