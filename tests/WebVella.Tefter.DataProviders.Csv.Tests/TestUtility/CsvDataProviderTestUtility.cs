using System.Text;
using Microsoft.FluentUI.AspNetCore.Components;
using WebVella.Tefter.DataProviders.Csv.Addons;
using WebVella.Tefter.Models;

namespace WebVella.Tefter.DataProviders.Csv.Tests.Utils;

public class CsvDataProviderTestUtility
{
    public TfSpacePageCreateFromFileContextItem CreateFileContext(string fileName)
    {
        var result = new TfSpacePageCreateFromFileContextItem
        {
            User = new(),
            File = new FluentInputFileEventArgs(),
            FileName = fileName,
            LocalPath = "",
            FileContent = LoadFileAsStream(fileName),
            IsProcessed = false,
            ProcessStream = new(),
            IsSelected = false,
            IsSuccess = true,
            ProcessContext = new TfImportFileToPageResultProcessContext()
            {
                UsedDataProviderAddon = new CsvDataProvider()
            }
        };
        return result; 
    }    
    
    public byte[] LoadFile(string fileName)
    {
        var path = Path.Combine(Environment.CurrentDirectory, $"Files\\{fileName}");
        var fi = new FileInfo(path);
        if (!fi.Exists) throw new FileNotFoundException();
        return File.ReadAllBytes(path);
    }

    public MemoryStream LoadFileStream(string fileName)
    {
        return new MemoryStream(LoadFile(fileName));
    }

    public MemoryStream LoadFileAsStream(string fileName)
    {
        var path = Path.Combine(Environment.CurrentDirectory, $"Files\\{fileName}");
        var fi = new FileInfo(path);
        if (!fi.Exists) throw new FileNotFoundException();
        MemoryStream ms = new MemoryStream();
        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            file.CopyTo(ms);

        ms.Position = 0;
        return ms;
    }
    
    public MemoryStream ChangeStreamEncoding(MemoryStream original,Encoding newEncoding)
    {
        using var reader = new StreamReader(original, Encoding.UTF8);
        var content = reader.ReadToEnd();
        byte[] encodedBytes = newEncoding.GetBytes(content);
        return new MemoryStream(encodedBytes);
    }
    
}