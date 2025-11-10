using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using WebVella.BlazorTrace.Utility;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Csv;

public static class AICsvDataProviderUtility
{
// Send raw data to LLM via Ollama API
    public static async Task<SchemaInference> InferSchemaAsync(this MemoryStream fileStream)
    {
        using var stream = new MemoryStream();
        fileStream.Position = 0;
        await fileStream.CopyToAsync(stream);
        stream.Position = 0;
        var config = new CsvConfiguration(CultureInfo.DefaultThreadCurrentCulture ?? new CultureInfo("en-US"))
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
            Encoding = Encoding.UTF8,
            IgnoreBlankLines = true,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim,
            HasHeaderRecord = false,
            MissingFieldFound = null,
            DetectDelimiter = true,
        };
        using var csvReader = new CsvReader(new StreamReader(stream), config);
        var limitRows = 20;
        var addedRows = 0;
        var result = new List<List<string?>>();

        while (await csvReader.ReadAsync())
        {
            var row = new List<string?>();
            for (int i = 0; i < csvReader.ColumnCount; i++)
            {
                row.Add(csvReader.GetField(i));
            }

            result.Add(row);

            addedRows++;
            if (addedRows >= limitRows)
            {
                break;
            }
        }

        var prompt = $@"
    You are a data schema inference assistant.
    Given the following sample rows from a CSV or Excel file, extract:
    - Column names (if not provided)
    - Data types for each column
    - Whether the first row is a header

    Sample data:
    {string.Join("\n", result.Select(r => $"[{string.Join(", ", r)}]"))}

    Return JSON format:
    {{
        ""HasHeader"": true,
        ""ColumnNames"": [""Name"", ""Age"", ""Email""],
        ""DataTypes"": [""string"", ""int"", ""string""]
    }}";

        var client = new HttpClient();
        var requestBody = new
        {
            model = "qwen/qwen3-vl-30b", // Adjust to your model
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.3,
            max_tokens = 10000,
            top_p = 0.9,
            stream = false
        };
        var response2 = await client.GetAsync("http://127.0.0.1:1234/v1/models");

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://127.0.0.1:1234/v1/chat/completions", content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var lmResponse = JsonSerializer.Deserialize<LmStudioResponse>(json) ?? new LmStudioResponse();
            if (lmResponse.choices.Count > 0)
            {
                var schemaJson = lmResponse.choices[0].message.content.Replace("```", "");
                if (schemaJson.StartsWith("json"))
                {
                    schemaJson = schemaJson.Substring(4);
                }

                return JsonSerializer.Deserialize<SchemaInference>(schemaJson) ?? new SchemaInference();
            }
        }

        throw new Exception("Failed to call LM Studio");
    }
}

public record SchemaInference
{
    public bool HasHeader { get; set; } = false;
    public List<string> ColumnNames { get; set; } = new();
    public List<string> DataTypes { get; set; } = new();
}

public class LmStudioResponse
{
    public List<LmStudioChoice> choices { get; set; }
}

public class LmStudioChoice
{
    public LmStudioMessage message { get; set; }
    public int index { get; set; }
    public string finish_reason { get; set; }
}

public class LmStudioMessage
{
    public string role { get; set; }
    public string content { get; set; }
}