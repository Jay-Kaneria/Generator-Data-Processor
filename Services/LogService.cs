using Serilog;
using System.Configuration;

namespace GeneratorDataProcessing.Services;

public static class LogService
{
    static string logFolderPath = ConfigurationManager.AppSettings["LogFolderPath"];
    public static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File(Path.Combine(logFolderPath, "log-.txt"),
                                          rollingInterval: RollingInterval.Day)
                            .MinimumLevel.Debug()
                            .CreateLogger();
    }
}
