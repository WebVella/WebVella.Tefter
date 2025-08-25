using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Text.Json;
using System.Xml.Linq;

namespace LOCResourceExtractor;

class Program
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

			string folderPath = config.FolderPath;
			string resxFilePath = config.ResxFilePath;

			if (!Directory.Exists(folderPath))
			{
				Console.WriteLine($"Folder does not exist: {folderPath}");
				return;
			}

			if (!File.Exists(resxFilePath))
			{
				Console.WriteLine($"Resx file does not exist: {resxFilePath}");
				return;
			}

			// Find all .cs and .razor files
			var csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
			var razorFiles = Directory.GetFiles(folderPath, "*.razor", SearchOption.AllDirectories);
			var allFiles = csFiles.Concat(razorFiles);

			// Extract LOC strings from files
			var locStrings = new HashSet<string>();
			foreach (string file in allFiles)
			{
				ExtractLocStringsFromFile(file, locStrings);
			}

			Console.WriteLine($"Found {locStrings.Count} unique LOC strings.");

			// Read existing resx entries
			var existingKeys = ReadResxKeys(resxFilePath);
			var existingKeysHS = new HashSet<string>();
			foreach (var key in existingKeys)
			{
				var keyLower = key.ToLowerInvariant();
				if (!existingKeysHS.Contains(keyLower))
				{
					existingKeysHS.Add(keyLower);
				}
			}

			// Find missing keys
			var missingKeys = new List<string>();
			foreach (string key in locStrings)
			{
				var keyLower = key.ToLowerInvariant();
				if (!existingKeysHS.Contains(keyLower))
				{
					missingKeys.Add(key);
				}
			}
			if (missingKeys.Any())
			{
				Console.WriteLine($"Adding {missingKeys.Count} missing keys to resx file.");

				AddMissingKeysToResx(resxFilePath, missingKeys);

				Console.WriteLine("Done. Missing keys have been added to the resx file.");
			}
			else
			{
				Console.WriteLine("All LOC strings are already present in the resx file.");
			}

			var locKeysHS = new HashSet<string>();
			foreach (var key in locStrings)
			{
				var keyLower = key.ToLowerInvariant();
				if (!locKeysHS.Contains(keyLower))
				{
					locKeysHS.Add(keyLower);
				}
			}

			//Find old keys
			var oldKeys = new List<string>();
			foreach (string key in existingKeys)
			{
				var keyLower = key.ToLowerInvariant();
				if (!locKeysHS.Contains(keyLower))
				{
					oldKeys.Add(key);
				}
			}


			if (oldKeys.Any())
			{
				Console.WriteLine($"Removing {oldKeys.Count} old keys from resx file.");

				CleanOldKeysFromResx(resxFilePath, oldKeys);

				Console.WriteLine("Done. Old keys have been removed from the resx file.");
			}
			else
			{
				Console.WriteLine("No old LOC strings are found in the resx file.");
			}
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

	static void ExtractLocStringsFromFile(string filePath, HashSet<string> locStrings)
	{
		try
		{

			string content = File.ReadAllText(filePath);
			var results = LocExtractor.ExtractLocValues(content);


			foreach (string value in results)
			{
				if (!string.IsNullOrEmpty(value))
				{
					locStrings.Add(value);
				}

			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
		}
	}

	static HashSet<string> ReadResxKeys(string resxFilePath)
	{
		var keys = new HashSet<string>();

		try
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(resxFilePath);

			XmlNodeList nodes = doc.SelectNodes("//data");
			foreach (XmlNode node in nodes)
			{
				XmlAttribute keyAttr = node.Attributes["name"];
				if (keyAttr != null && !string.IsNullOrEmpty(keyAttr.Value))
				{
					keys.Add(keyAttr.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error reading resx file {resxFilePath}: {ex.Message}");
		}

		return keys;
	}

	static void AddMissingKeysToResx(string resxFilePath, List<string> missingKeys)
	{
		try
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(resxFilePath);

			XmlElement root = doc.DocumentElement;
			XmlElement dataNode = doc.CreateElement("data");
			var addedKeys = new HashSet<string>();
			foreach (string key in missingKeys)
			{
				var lowerKey = key.ToLowerInvariant();
				if (addedKeys.Contains(lowerKey)) continue;
				addedKeys.Add(lowerKey);
				// Create a new data element
				XmlElement newDataNode = doc.CreateElement("data");
				newDataNode.SetAttribute("name", key);
				newDataNode.SetAttribute("xml:space", "preserve");

				XmlElement valueNode = doc.CreateElement("value");
				valueNode.InnerText = ""; // Empty value

				newDataNode.AppendChild(valueNode);
				root.AppendChild(newDataNode);
			}

			doc.Save(resxFilePath);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error writing to resx file {resxFilePath}: {ex.Message}");
		}
	}

	static void CleanOldKeysFromResx(string resxFilePath, List<string> oldKeys)
	{
		try
		{

			// Load the .resx file into an XDocument
			XDocument resxDoc = XDocument.Load(resxFilePath);

			// Access the Root element and then call Descendants() on it
			XElement rootElement = resxDoc.Root;
			foreach (string key in oldKeys)
			{
				var elementToRemove = rootElement.Descendants("data")
								.FirstOrDefault(d => d.Attribute("name")?
								.Value.Equals(key, StringComparison.OrdinalIgnoreCase) == true);
				if (elementToRemove != null)
				{
					// Remove the element and save the changes
					elementToRemove.Remove();
				}
			}

			resxDoc.Save(resxFilePath);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error writing to resx file {resxFilePath}: {ex.Message}");
		}
	}

}

// Configuration class
class Config
{
	public string FolderPath { get; set; }
	public string ResxFilePath { get; set; }
}
