using System.Text;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

// Debug: Verify directories
Console.WriteLine($"Current Directory: {currentDirectory}");
Console.WriteLine($"Stores Directory: {storesDirectory}");
Console.WriteLine($"Sales Total Directory: {salesTotalDir}");

// Ensure stores directory exists
if (!Directory.Exists(storesDirectory))
{
    Console.WriteLine("Stores directory does not exist.");
    return;
}

var salesFiles = FindFiles(storesDirectory);

// Debug: Check found files
Console.WriteLine("Sales Files:");
foreach (var file in salesFiles)
{
    Console.WriteLine(file);
}

var salesTotal = CalculateSalesTotal(salesFiles);

// Debug: Check sales total
Console.WriteLine($"Sales Total: {salesTotal}");

var totalsFilePath = Path.Combine(salesTotalDir, "totals.txt");
File.AppendAllText(totalsFilePath, $"{salesTotal}{Environment.NewLine}");

// Debug: Confirm file writing
Console.WriteLine($"Sales total written to: {totalsFilePath}");

// Generate sales summary report
var reportFilePath = Path.Combine(salesTotalDir, "SalesSummary.txt");
GenerateSalesSummaryReport(salesFiles, salesTotal, reportFilePath);
Console.WriteLine($"Sales summary report written to: {reportFilePath}");

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
        
        // Debug: Check sales JSON content
        Console.WriteLine($"Reading file: {file}");
        Console.WriteLine(salesJson);
    
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, double totalSales, string reportFilePath)
{
    var report = new StringBuilder();
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {totalSales.ToString("C")}");
    report.AppendLine();
    report.AppendLine(" Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        var total = data?.Total ?? 0;
        report.AppendLine($"  {Path.GetFileName(file)}: {total.ToString("C")}");
    }

    File.WriteAllText(reportFilePath, report.ToString());
}

record SalesData(double Total);
