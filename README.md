# Generator Data Processor

## Overview
This C# console application processes incoming XML files containing generator data. It calculates key metrics such as total generation values, daily emissions, and heat rates for coal generators. The results are then outputted in a specified XML format. The app monitors input and output folders, and logs activities to a configured log path.

## Features
- Automatically detects and processes XML files placed in the input folder.
- Calculates the following:
  - Total Generation Value for each generator.
  - Generator with the highest Daily Emissions for each day.
  - Actual Heat Rate for each coal generator.
- Outputs the results in an XML format as specified by the example.
- Logs application activity to the console and a log file.
- Exception handling for runtime errors.

## Configuration
Set the `inputFolderPath`, `outputFolderPath`, and `logFolderPath` to your desired directory paths in the `app.config` file.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="InputFolderPath" value="C:\Brady\Input\" />
    <add key="OutputFolderPath" value="C:\Brady\Output\" />
    <add key="LogFolderPath" value="C:\Brady\Logs\" />
  </appSettings>
</configuration>
```

## Assumptions for the Input XML File
- Generators: All generators have a name attribute.
- Wind Generators: Must have a location attribute.
- Day Elements: Each day element must contain an energy and a price element.
- Generation Element: A generator may not always have a generation element.
- Gas Generators: Must have an emission rating element.
- Coal Generators: Must have TotalHeatInput, ActualNetGeneration, and EmissionsRating elements.
