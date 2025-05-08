namespace WebVella.Tefter.Utility;
public static class AssemblyExtensions
{
	public static string GetFileFromResourceAndUploadLocally(this Assembly assembly, string resourceName)
	{
		using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
		{
			if (stream == null)
			{
				throw new FileNotFoundException($"Resource '{resourceName}' not found.");
			}

			// Specify the temporary folder path
			string tempPath = Path.GetTempPath();
			string fileName = Path.GetFileName(resourceName);
			string filePath = Path.Combine(tempPath, fileName);

			// Write the stream to a file in the temporary folder
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				stream.CopyTo(fileStream);
			}

			return filePath;
		}
	}


}
