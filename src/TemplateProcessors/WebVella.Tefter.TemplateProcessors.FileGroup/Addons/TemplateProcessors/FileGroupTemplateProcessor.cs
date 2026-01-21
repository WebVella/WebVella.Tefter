using System.Globalization;
using System.IO.Compression;
using System.Text;
using WebVella.DocumentTemplates.Core;
using WebVella.DocumentTemplates.Engines.DocumentFile;
using WebVella.DocumentTemplates.Engines.Email.Models;
using WebVella.DocumentTemplates.Engines.SpreadsheetFile;
using WebVella.DocumentTemplates.Engines.TextFile;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Addons;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Models;
using WebVella.Tefter.TemplateProcessors.TextFile.Addons;
using WebVella.Tefter.TemplateProcessors.TextFile.Models;
using WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;
using WebVella.Tefter.TemplateProcessors.DocumentFile.Models;


namespace WebVella.Tefter.TemplateProcessors.FileGroup.Addons;

public class FileGroupTemplateProcessor : ITfTemplateProcessorAddon
{
    public const string ID = "2B6C7791-E427-4AAF-BE7A-E110B34D020F";
    public const string NAME = "File Group";
    public const string DESCRIPTION = "returns several file templates as result";
    public const string FLUENT_ICON_NAME = "Folder";
    public const TfTemplateResultType RESULT_TYPE = TfTemplateResultType.FileGroup;

    public Guid Id { get; init; } = new Guid(ID);
    public string Name { get; init; } = NAME;
    public string Description { get; init; } = DESCRIPTION;
    public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
    public TfTemplateResultType ResultType { get; init; } = RESULT_TYPE;


    public void ValidatePreview(
        TfTemplate template,
        ITfTemplatePreviewResult preview,
        IServiceProvider serviceProvider)
    {
        var previewResult = (FileGroupTemplatePreviewResult)preview;
        foreach (var item in previewResult.Items)
        {
            item.Errors.Clear();

            // if (!string.IsNullOrWhiteSpace(item.Sender) && !item.Sender.IsEmail())
            // {
            //     item.Errors.Add(new ValidationError("Sender", "Sender is not a valid email address."));
            // }
        }
    }

    public ITfTemplatePreviewResult GenerateTemplatePreviewResult(
        TfTemplate template,
        TfDataTable dataTable,
        List<Guid> tfRecordIds,
        List<Guid> tfDatasetIds,
        List<Guid> tfSpaceIds,
        Guid sessionId,
        Guid userId,
        IServiceProvider serviceProvider)
    {
        var result =
            GenerateResultInternal(template, dataTable, tfRecordIds, tfDatasetIds, tfSpaceIds, userId, serviceProvider);
        FileGroupTemplatePreviewResult previewResult = new FileGroupTemplatePreviewResult
        {
            Items = ((FileGroupTemplateResult)result).Items,
            Errors = result.Errors,
        };
        ValidatePreview(template, previewResult, serviceProvider);
        return previewResult;
    }

    public ITfTemplateResult ProcessTemplate(
        TfTemplate template,
        TfDataTable dataTable,
        List<Guid> tfRecordIds,
        List<Guid> tfDatasetIds,
        List<Guid> tfSpaceIds,
        Guid sessionId,
        Guid userId,
        ITfTemplatePreviewResult preview,
        IServiceProvider serviceProvider)
    {
        var tfService = serviceProvider.GetService<ITfService>();
        if(tfService is null) 
            throw new Exception("tfService not found");
        
        var result =
            GenerateResultInternal(template, dataTable, tfRecordIds, tfDatasetIds, tfSpaceIds, userId, serviceProvider);
        FileGroupTemplatePreviewResult previewResult = new FileGroupTemplatePreviewResult
        {
            Items = ((FileGroupTemplateResult)result).Items,
            Errors = result.Errors,
        };
        var fileCounter = 0;
        foreach (var item in previewResult.Items)
        {
            var fileName = fileCounter == 0 ? "package.zip" : $"package-{fileCounter}.zip";
            GenerateZipFile(fileName, item, tfService);
            fileCounter++;
        }

        return result;
    }

    private ITfTemplateResult GenerateResultInternal(
        TfTemplate template,
        TfDataTable dataTable,
        List<Guid> tfRecordIds,
        List<Guid> tfDatasetIds,
        List<Guid> tfSpaceIds,
        Guid userId,
        IServiceProvider serviceProvider)
    {
        var result = new FileGroupTemplateResult();

        var tfService = serviceProvider.GetService<ITfService>()!;

        if (string.IsNullOrWhiteSpace(template.SettingsJson))
        {
            result.Errors.Add(new ValidationError("", $"Template '{template.Name}' settings are not set."));
            return result;
        }

        var settings = JsonSerializer.Deserialize<FileGroupTemplateSettings>(template.SettingsJson);
        if (settings is null)
        {
            result.Errors.Add(new ValidationError("", $"Template '{template.Name}' settings invalid."));
            return result;
        }

        var templateDict = tfService.GetTemplates().ToDictionary(x => x.Id);

        var templateFilesToGenerated = new List<TfTemplate>();
        foreach (var attachment in settings.AttachmentItems)
        {
            if (!templateDict.ContainsKey(attachment.TemplateId)) continue;
            try
            {
                FillInTemplates(templateDict[attachment.TemplateId], templateFilesToGenerated, templateDict);
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationError("", ex.Message));
                return result;
            }
        }

        if (templateFilesToGenerated.Count == 0) return result;

        var groupedDt = GroupDataTable(settings.GroupBy, dataTable);
        int dsIndex = 0;
        foreach (var groupKey in groupedDt.Keys)
        {
            var groupDt = groupedDt[groupKey].ToDataTable();
            var resultItem = new FileGroupTemplateResultItem();
            foreach (DataRow row in groupDt.Rows)
            {
                resultItem.RelatedRowIds.Add((Guid)row[TfConstants.TEFTER_ID_COLUMN_NAME]);
            }

            foreach (var attachedTemplate in templateFilesToGenerated)
            {
                if (((TfTemplate)attachedTemplate).ContentProcessorType == typeof(ExcelFileTemplateProcessor))
                {
                    var attachment = ProcessSpreadsheetAttachment(attachedTemplate, dsIndex, groupDt, tfService);
                    if (attachment is null)
                        resultItem.Errors.Add(new ValidationError("",
                            $"Template '{attachedTemplate.Name}' - Could not generate result "));
                    else
                        resultItem.Attachments.Add(attachment);
                }
                else if (((TfTemplate)attachedTemplate).ContentProcessorType == typeof(DocumentFileTemplateProcessor))
                {
                    var attachment = ProcessDocumentAttachment(attachedTemplate, dsIndex, groupDt, tfService);
                    if (attachment is null)
                        resultItem.Errors.Add(new ValidationError("",
                            $"Template '{attachedTemplate.Name}' - Could not generate result "));
                    else
                        resultItem.Attachments.Add(attachment);
                }
                else if (((TfTemplate)attachedTemplate).ContentProcessorType == typeof(TextFileTemplateProcessor))
                {
                    var attachment = ProcessTextFileAttachment(attachedTemplate, dsIndex, groupDt, tfService);
                    if (attachment is null)
                        resultItem.Errors.Add(new ValidationError("",
                            $"Template '{attachedTemplate.Name}' - Could not generate result "));
                    else
                        resultItem.Attachments.Add(attachment);
                }
            }

            result.Items.Add(resultItem);

            dsIndex++;
        }


        return result;
    }

    public List<ValidationError> ValidateSettings(
        string settingsJson,
        IServiceProvider serviceProvider)
    {
        var result = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(settingsJson))
        {
            return result;
        }

        var settings = JsonSerializer.Deserialize<FileGroupTemplateSettings>(settingsJson);

        // if (string.IsNullOrWhiteSpace(settings.Recipients))
        // {
        //     result.Add(new ValidationError(nameof(settings.Recipients), "Recipient(s) is/are required."));
        // }

        return result;
    }

    public List<ValidationError> OnCreate(
        TfManageTemplateModel template,
        IServiceProvider serviceProvider)
    {
        return new List<ValidationError>();
    }

    public void OnCreated(
        TfTemplate template,
        IServiceProvider serviceProvider)
    {
    }

    public List<ValidationError> OnUpdate(
        TfManageTemplateModel template,
        IServiceProvider serviceProvider)
    {
        return new List<ValidationError>();
    }

    public void OnUpdated(
        TfTemplate template,
        IServiceProvider serviceProvider)
    {
    }

    public List<ValidationError> OnDelete(
        TfTemplate template,
        IServiceProvider serviceProvider)
    {
        return new List<ValidationError>();
    }

    public void OnDeleted(
        TfTemplate template,
        IServiceProvider serviceProvider)
    {
    }

    public List<TfTemplate> GetTemplateSelectionList(
        Guid? templateId,
        ITfService _tfService)
    {
        var result = new List<TfTemplate>();
        var allTemplates = _tfService.GetTemplates();
        foreach (var item in allTemplates)
        {
            if (item.ResultType != TfTemplateResultType.File) continue;
            if (!item.IsSelectable) continue;
            if (item.Id == templateId) continue;
            result.Add(item);
        }

        result = result.OrderBy(x => x.Name).ToList();
        return result;
    }

    private void FillInTemplates(TfTemplate template, List<TfTemplate> templateFilesToGenerated,
        Dictionary<Guid, TfTemplate> templateDict)
    {
        if (templateFilesToGenerated.Any(x => x.Id == template.Id)) return;
        if (template.ContentProcessorType.FullName != typeof(ExcelFileTemplateProcessor).FullName
            && template.ContentProcessorType.FullName != typeof(DocumentFileTemplateProcessor).FullName
            && template.ContentProcessorType.FullName != typeof(TextFileTemplateProcessor).FullName
            && template.ContentProcessorType.FullName != typeof(FileGroupTemplateProcessor).FullName) return;

        if (template.ContentProcessorType.FullName == typeof(FileGroupTemplateProcessor).FullName)
        {
            if (string.IsNullOrWhiteSpace(template.SettingsJson))
            {
                throw new Exception($"Template '{template.Name}' settings are not set.");
            }

            var settings = JsonSerializer.Deserialize<FileGroupTemplateSettings>(template.SettingsJson);
            if (settings is null)
            {
                throw new Exception($"Template '{template.Name}' settings invalid.");
            }

            foreach (var attachment in settings.AttachmentItems)
            {
                if (!templateDict.ContainsKey(attachment.TemplateId)) continue;
                try
                {
                    FillInTemplates(templateDict[attachment.TemplateId], templateFilesToGenerated, templateDict);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return;
        }

        templateFilesToGenerated.Add(template);
    }

    private Dictionary<string, TfDataTable> GroupDataTable(
        List<string> groupColumns,
        TfDataTable dataTable)
    {
        var result = new Dictionary<string, TfDataTable>();
        if (groupColumns is null || groupColumns.Count == 0)
        {
            result.Add(string.Empty, dataTable);
        }
        else
        {
            foreach (TfDataRow row in dataTable.Rows)
            {
                var sbKey = new StringBuilder();

                foreach (var column in groupColumns)
                {
                    sbKey.Append($"{row[column]}$$$|||$$$");
                }

                var key = sbKey.ToString();

                if (!result.ContainsKey(key))
                {
                    result.Add(key, dataTable.NewTable());
                }

                result[key].Rows.Add(row);
            }
        }

        return result;
    }


    private FileGroupTemplateResultItemAttachment ProcessSpreadsheetAttachment(TfTemplate template, int dsIndex,
        DataTable dataSource, ITfService tfService)
    {
        var attachment = new FileGroupTemplateResultItemAttachment
        {
            TemplateId = template.Id,
        };
        if (String.IsNullOrWhiteSpace(template.SettingsJson))
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "settings not found"));
            return attachment;
        }

        var settings =
            JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);
        if (settings is null)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "settings format invalid"));
            return attachment;
        }

        if (settings.TemplateFileBlobId is null)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "no template file present"));
            return attachment;
        }

        byte[] bytes = [];
        try
        {
            bytes = tfService.GetBlobByteArray(settings.TemplateFileBlobId.Value);
        }
        catch (Exception ex)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson",
                $"uploaded file is not found on the file system"));
            return attachment;
        }

        if (bytes.Length == 0)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson",
                $"uploaded file is empty on the file system"));
            return attachment;
        }

        var attachmentTemplate = new WvSpreadsheetFileTemplate
        {
            Template = new MemoryStream(bytes),
            GroupDataByColumns = new List<string>() //grouping already done
        };

        var culture = new CultureInfo("en-US");
        var attachmentTemplateResult = attachmentTemplate.Process(dataSource, culture);
        if (attachmentTemplateResult.ResultItems.Count == 0
            || attachmentTemplateResult.ResultItems[0].Result is null)
        {
            return attachment;
        }
        var valErrors = attachmentTemplateResult.ResultItems[0].Validate();
        if (valErrors.Count > 0)
        {
            foreach (var valError in valErrors)
            {
                attachment.Errors.Add(new ValidationError("",valError.Description));
            }

            return attachment;
        }

        var ext = Path.GetExtension(settings.FileName);
        var name = Path.GetFileNameWithoutExtension(settings.FileName);
        var filename = dsIndex == 0 ? settings.FileName : $"{name}-{dsIndex}{ext}";
        var blobId = tfService.CreateBlob(attachmentTemplateResult.ResultItems[0].Result!, temporary: true);
        
        
        attachment = new FileGroupTemplateResultItemAttachment
        {
            FileName = filename,
            BlobId = blobId,
            DownloadUrl = $"/fs/blob/{blobId}/{filename}",
        };

        return attachment;
    }

    private FileGroupTemplateResultItemAttachment ProcessDocumentAttachment(TfTemplate template, int dsIndex,
        DataTable dataSource, ITfService tfService)
    {
        var attachment = new FileGroupTemplateResultItemAttachment
        {
            TemplateId = template.Id,
        };
        if (String.IsNullOrWhiteSpace(template.SettingsJson))
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "settings not found"));
            return attachment;
        }

        var settings =
            JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);
        if (settings is null)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "settings format invalid"));
            return attachment;
        }

        if (settings.TemplateFileBlobId is null)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "no template file present"));
            return attachment;
        }

        byte[] bytes = [];
        try
        {
            bytes = tfService.GetBlobByteArray(settings.TemplateFileBlobId.Value);
        }
        catch (Exception ex)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson",
                $"uploaded file is not found on the file system"));
            return attachment;
        }

        if (bytes.Length == 0)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson",
                $"uploaded file is empty on the file system"));
            return attachment;
        }
        var attachmentTemplate = new WvDocumentFileTemplate
        {
            Template = new MemoryStream(bytes),
            GroupDataByColumns = new List<string>() //grouping already done
        };

        var culture = new CultureInfo("en-US");
        var attachmentTemplateResult = attachmentTemplate.Process(dataSource, culture);
        if (attachmentTemplateResult.ResultItems.Count == 0
            || attachmentTemplateResult.ResultItems[0].Result is null)
        {
            return attachment;
        }
        var valErrors = attachmentTemplateResult.ResultItems[0].Validate();
        if (valErrors.Count > 0)
        {
            foreach (var valError in valErrors)
            {
                attachment.Errors.Add(new ValidationError("",valError.Description));
            }

            return attachment;
        }
        var ext = Path.GetExtension(settings.FileName);
        var name = Path.GetFileNameWithoutExtension(settings.FileName);
        var filename = dsIndex == 0 ? settings.FileName : $"{name}-{dsIndex}{ext}";
        var blobId = tfService.CreateBlob(attachmentTemplateResult.ResultItems[0].Result!, temporary: true);
        attachment = new FileGroupTemplateResultItemAttachment
        {
            FileName = filename,
            BlobId = blobId,
            DownloadUrl = $"/fs/blob/{blobId}/{filename}",
        };

        return attachment;
    }

    private FileGroupTemplateResultItemAttachment ProcessTextFileAttachment(TfTemplate template, int dsIndex,
        DataTable dataSource, ITfService tfService)
    {
        var attachment = new FileGroupTemplateResultItemAttachment
        {
            TemplateId = template.Id,
        };
        if (String.IsNullOrWhiteSpace(template.SettingsJson))
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "settings not found"));
            return attachment;
        }

        var settings =
            JsonSerializer.Deserialize<ExcelFileTemplateSettings>(template.SettingsJson);
        if (settings is null)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "settings format invalid"));
            return attachment;
        }

        if (settings.TemplateFileBlobId is null)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson", "no template file present"));
            return attachment;
        }

        byte[] bytes = [];
        try
        {
            bytes = tfService.GetBlobByteArray(settings.TemplateFileBlobId.Value);
        }
        catch (Exception ex)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson",
                $"uploaded file is not found on the file system"));
            return attachment;
        }

        if (bytes.Length == 0)
        {
            attachment.Errors.Add(new ValidationError("SettingsJson",
                $"uploaded file is empty on the file system"));
            return attachment;
        }
        var attachmentTemplate = new WvTextFileTemplate
        {
            Template = new MemoryStream(bytes),
            GroupDataByColumns = new List<string>() //grouping already done
        };

        var culture = new CultureInfo("en-US");
        var attachmentTemplateResult = attachmentTemplate.Process(dataSource, culture);
        if (attachmentTemplateResult.ResultItems.Count == 0
            || attachmentTemplateResult.ResultItems[0].Result is null)
        {
            return attachment;
        }

        var ext = Path.GetExtension(settings.FileName);
        var name = Path.GetFileNameWithoutExtension(settings.FileName);
        var filename = dsIndex == 0 ? settings.FileName : $"{name}-{dsIndex}{ext}";
        var blobId = tfService.CreateBlob(attachmentTemplateResult.ResultItems[0].Result!, temporary: true);
        attachment = new FileGroupTemplateResultItemAttachment
        {
            FileName = filename,
            BlobId = blobId,
            DownloadUrl = $"/fs/blob/{blobId}/{filename}",
        };

        return attachment;
    }
    
    private void GenerateZipFile(
        string filename,
        FileGroupTemplateResultItem result,
        ITfService tfService)
    {
        var validItems = result.Attachments
            .Where(x => x.Errors.Count == 0 && x.BlobId.HasValue)
            .ToList();

        if (validItems.Count == 0)
        {
            return;
        }

        using MemoryStream zipMs = new MemoryStream();

        using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, true))
        {
            foreach (var item in validItems)
            {
                var fileBytes = tfService.GetBlobByteArray(item.BlobId.Value, temporary: true);
                var zipArchiveEntry = archive.CreateEntry(item.FileName, CompressionLevel.Fastest);
                using var zipStream = zipArchiveEntry.Open();
                zipStream.Write(fileBytes, 0, fileBytes.Length);
                zipStream.Close();
            }
        }

        var name = Path.GetFileNameWithoutExtension(filename);

        var zipBlobId = tfService.CreateBlob(zipMs, temporary: true);
        result.FileName = filename;
        result.BlobId = zipBlobId;
        result.DownloadUrl = $"/fs/blob/{zipBlobId}/{filename}";
    }    
}