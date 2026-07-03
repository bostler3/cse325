using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

var salesSummary = GenerateSalesSummaryReport();

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");
File.WriteAllText(Path.Combine(currentDirectory, "salesSummary.txt"), $"{salesSummary}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

string GenerateSalesSummaryReport()
{
    var salesTotalCurrency = salesTotal.ToString("C");
    StringBuilder sb = new StringBuilder();
    sb.Append("Sales Summary");
    sb.AppendLine();
    sb.Append("-----------------------------");
    sb.AppendLine();
    sb.Append($"Total Sales: {salesTotalCurrency}");
    sb.AppendLine();
    sb.AppendLine();
    sb.Append("Details:");
    sb.AppendLine();
    // Using only the sales.json files and NOT salestotals.json files like I think the tutorial did
    List<string> salesJsonFiles = new List<string>();
    var foundSalesJsonFiles = Directory.EnumerateFiles(currentDirectory, "*", SearchOption.AllDirectories);
    foreach (var file in foundSalesJsonFiles)
    {
        if (file.EndsWith("sales.json"))
        {
            salesJsonFiles.Add(file);
        }
    }

    foreach (var file in salesJsonFiles)
    {
        FileInfo info = new FileInfo(file);
        string fullFileName = info.FullName;
        // Got help from a Bing search for: "remove a part of a string in c#"
        string displayFileName = fullFileName.Remove(0, 24);
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        double sales = data?.Total ?? 0;
        var salesCurrency = sales.ToString("C");
        sb.Append($"  {displayFileName}: {salesCurrency}");
        sb.AppendLine();
    }
    string result = sb.ToString();
    return result;
}

record SalesData (double Total);
