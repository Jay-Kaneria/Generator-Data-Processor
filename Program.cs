using GeneratorDataProcessing.Services;
namespace GeneratorDataProcessing;

internal class Program
{
    static async Task Main(string[] args)
    {
        //Initialize serilog logger
        LogService.ConfigureLogger();

        //Load reference data from file.
        ReferenceDataService.LoadReferenceData();

        //Initialize and Start the file watcher
        FileWatcherService fileWatcherService = new FileWatcherService();
        await fileWatcherService.StartAsync();
    }
}
