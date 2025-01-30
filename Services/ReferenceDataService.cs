using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeneratorDataProcessing.Services;

public static class ReferenceDataService
{
    public static Dictionary<string, (decimal ValueFactor, decimal EmissionFactor)> referenceData;
    public static void LoadReferenceData()
    {
        try
        {
            if (referenceData != null)
            {
                Log.Information("Reference data already loaded.");
                return;
            }

            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string referenceDataFilePath = Path.Combine(directory, "ReferenceData.xml");

            if (!File.Exists(referenceDataFilePath))
            {
                Log.Error("Error: ReferenceData.xml not found at {ReferenceDataFilePath}", referenceDataFilePath);
                return;
            }

            XElement referenceXml = XElement.Load(referenceDataFilePath);

            var valueFactors = referenceXml.Descendants("ValueFactor")
                .Elements()
                .ToDictionary(
                    element => element.Name.LocalName,
                    element => decimal.Parse(element.Value)
                );

            var emissionsFactors = referenceXml.Descendants("EmissionsFactor")
                .Elements()
                .ToDictionary(
                    element => element.Name.LocalName,
                    element => decimal.Parse(element.Value)
                );

            referenceData = new Dictionary<string, (decimal ValueFactor, decimal EmissionFactor)>
            {
                { "Offshore", (valueFactors["Low"], 0) },  // Offshore Wind has no emissions factor
                { "Onshore", (valueFactors["High"], 0) },    // Onshore Wind has no emissions factor
                { "Gas", (valueFactors["Medium"], emissionsFactors["Medium"]) },
                { "Coal", (valueFactors["Medium"], emissionsFactors["High"]) }
            };

            Log.Information("Reference data loaded successfully.");
        }
        catch (FileNotFoundException fnfEx)
        {
            Log.Error(fnfEx, "File not found: {Message}", fnfEx.Message);
        }
        catch (FormatException formatEx)
        {
            Log.Error(formatEx, "Data format error: {Message}", formatEx.Message);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred: {Message}", ex.Message);
        }
    }
}
