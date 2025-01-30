using Serilog;
using System.Configuration;
using System.Xml.Linq;

namespace GeneratorDataProcessing.Services;

public class FileWatcherService
{
    static string _inputFolderPath = ConfigurationManager.AppSettings["InputFolderPath"];
    static string _outputFolderPath = ConfigurationManager.AppSettings["OutputFolderPath"];

    private FileSystemWatcher _watcher;

    public FileWatcherService()
    {
        _watcher = new FileSystemWatcher(_inputFolderPath, "*.xml")
        {
            EnableRaisingEvents = true
        };
        _watcher.Created += OnNewFileDetected;
    }

    public async Task StartAsync()
    {
        Log.Information($"Monitoring new files in: {_inputFolderPath}");
        // Keeps the service running indefinitely
        await Task.Delay(-1);
    }

    private async void OnNewFileDetected(object sender, FileSystemEventArgs e)
    {
        try
        {
            Log.Information($"New file detected: {e.Name}");

            await Task.Delay(2000);

            XElement inputXml = XElement.Load(e.FullPath);

            if (ReportValidationService.IsValidGenerationReport(inputXml))
            {
                var resultXml = DataProcessingService.ProcessGeneratorData(inputXml);
                string outputFilePath = Path.Combine(_outputFolderPath, Path.GetFileNameWithoutExtension(e.Name) + "-Result.xml");
                resultXml.Save(outputFilePath);
                Log.Information($"Processed file saved to: {outputFilePath}");
            }
            else
            {
                Log.Warning($"Invalid XML file: {e.Name}. Skipping processing.");
            }
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error processing file {e.Name}: {ex.Message}");
        }
    }
}
