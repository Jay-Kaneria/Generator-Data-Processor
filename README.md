# Generator Data Processor

Overview
This C# console application processes incoming XML files containing generator data. It calculates key metrics such as total generation values, daily emissions, and heat rates for coal generators. The results are then outputted in a specified XML format. The app monitors input and output folders, as well as logs activities to a configured log path.

Features:
  Automatically detects and processes XML files placed in the input folder.
  Calculates the following:
  Total Generation Value for each generator.
  Generator with the highest Daily Emissions for each day.
  Actual Heat Rate for each coal generator.
  Outputs the results in an XML format as specified by the example.
  Supports dynamic file naming with timestamps to avoid overwriting results.
  Logs application activity to the console and a log file.
  Exception handling for runtime errors.

Configuration:
app.config: Set the inputFolderPath, outputFolderPath, and logFolderPath to your desired directory paths.

<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="InputFolderPath" value="C:\Brady\Input\" />
    <add key="OutputFolderPath" value="C:\Brady\Output\" />
    <add key="LogFolderPath" value="C:\Brady\Logs\" />
  </appSettings>
</configuration>

Input File Assumptions:

All generators have a name.
All Wind generators have a location.
All day elements contain an energy element.
All day elements contain a price element.
A generator may have no generation element.
All gas generators have an emissions rating element.
All coal generators have TotalHeatInput, ActualNetGeneration, and EmissionsRating elements.

Logging:
Serilog is integrated to handle logging, providing both console and file outputs.
Log output directory can be configured in app.config under the logFolderPath setting.
Logs runtime exceptions and other application activities to assist in debugging.

Exception Handling:
The application includes exception handling to catch and log runtime errors, which helps in diagnosing and addressing issues during production.

Running the Application

1. Run the console application.
2. Place the input xml file in the input folder.
   The application will:
    -> Parse the XML file(s).
    -> Calculate the required metrics.
    -> Output the results as an XML file in the output folder.
View logs for any potential issues or information in the log folder.
