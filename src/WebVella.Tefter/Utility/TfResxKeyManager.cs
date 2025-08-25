using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebVella.Tefter.Services;
public class TfResxKeyManager
{
	public async Task<bool> AddMissingKeyToResxAsync(
		string rootPath,
		string filePath,
		string culture,
		string key,
		string value)
	{
		try
		{
			// Validate inputs
			if (string.IsNullOrWhiteSpace(filePath) ||
				string.IsNullOrWhiteSpace(rootPath) ||
				string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentException("File path, key, and value cannot be null or empty");
			}

			filePath = rootPath + filePath;

			// Get the directory and filename
			var directory = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileNameWithoutExtension(filePath);
			var extension = Path.GetExtension(filePath);

			// Handle culture-specific files (e.g., Resources.fr.resx)
			string resxFilePath;
			if (!string.IsNullOrWhiteSpace(culture))
			{
				var cultureInfo = CultureInfo.GetCultureInfo(culture);
				resxFilePath = Path.Combine(directory, $"{fileName}.{cultureInfo.Name}{extension}");
			}
			else
			{
				resxFilePath = filePath;
			}

			if (!File.Exists(resxFilePath))
			{
				throw new FileNotFoundException($"RESX file not found: {resxFilePath}");
			}

			// Create XmlReaderSettings for proper encoding handling
			var readerSettings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Auto,
				IgnoreWhitespace = true
			};

			// Read existing RESX content
			XmlDocument doc = new XmlDocument();
			using (var reader = XmlReader.Create(resxFilePath, readerSettings))
			{
				doc.Load(reader);
			}

			// Check if key already exists
			var rootNode = doc.DocumentElement;
			var dataNodes = rootNode.SelectNodes("data");

			foreach (XmlNode node in dataNodes)
			{
				if (node.Attributes?["name"]?.Value == key)
				{
					// Key already exists, return false to indicate no change
					return false;
				}
			}

			// Create new data element
			var newDataNode = doc.CreateElement("data");

			// Add name attribute
			var nameAttribute = doc.CreateAttribute("name");
			nameAttribute.Value = key;
			newDataNode.Attributes.Append(nameAttribute);

			// Add type attribute (optional but recommended)
			var typeAttribute = doc.CreateAttribute("type");
			typeAttribute.Value = "System.Resources.ResXFileRef, System.Windows.Forms";
			newDataNode.Attributes.Append(typeAttribute);

			// Create value element
			var valueElement = doc.CreateElement("value");
			valueElement.InnerText = value;
			newDataNode.AppendChild(valueElement);

			// Add the new node to the document
			rootNode.AppendChild(newDataNode);

			// Save the updated RESX file
			using (var writer = XmlWriter.Create(resxFilePath, new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = Environment.NewLine,
				NewLineHandling = NewLineHandling.Replace
			}))
			{
				doc.Save(writer);
			}

			return true; // Successfully added the key
		}
		catch (Exception ex)
		{
			// Log the exception or handle as needed
			Console.WriteLine($"Error adding key to RESX file: {ex.Message}");
			throw; // Re-throw to let caller handle it
		}
	}
}

