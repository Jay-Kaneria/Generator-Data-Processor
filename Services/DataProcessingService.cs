using Serilog;
using System.Xml.Linq;

namespace GeneratorDataProcessing.Services;

public static class DataProcessingService
{
    /// <summary>
    /// Process energy generation data
    /// </summary>
    /// <param name="inputXml"></param>
    /// <returns></returns>
    public static XElement ProcessGeneratorData(XElement inputXml)
    {
        // Create lists to hold the results
        List<XElement> totalGenerationList = new List<XElement>();
        List<XElement> maxEmissionList = new List<XElement>();
        List<XElement> actualHeatRates = new List<XElement>();

        // Dictionary to track max emission per day
        Dictionary<string, (string GeneratorName, decimal Emission)> dailyMaxEmissions = new Dictionary<string, (string, decimal)>();

        try
        {
            // Process Wind Generators
            var windGenerators = inputXml.Descendants("Wind").Descendants("WindGenerator");
            foreach (var windGenerator in windGenerators)
            {
                string name = windGenerator.Element("Name")?.Value;
                var generation = windGenerator.Element("Generation");
                decimal totalGeneration = 0;

                if (generation != null)
                {
                    string location = windGenerator.Element("Location")?.Value;
                    if (string.IsNullOrEmpty(location))
                    {
                        Log.Warning("Wind generator '{Name}' has no location. Skipping.", name);
                        continue;
                    }

                    if (!ReferenceDataService.referenceData.ContainsKey(location))
                    {
                        Log.Warning("Location '{Location}' not found in reference data. Skipping generator '{Name}'.", location, name);
                        continue;
                    }

                    decimal valueFactor = ReferenceDataService.referenceData[location].ValueFactor;
                    foreach (var day in generation.Elements("Day"))
                    {
                        try
                        {
                            decimal energy = decimal.Parse(day.Element("Energy").Value);
                            decimal price = decimal.Parse(day.Element("Price").Value);
                            totalGeneration += energy * price * valueFactor;
                        }
                        catch (FormatException ex)
                        {
                            Log.Error(ex, "Invalid format in wind generator '{Name}' for day element.");
                            continue;
                        }
                    }
                }

                totalGenerationList.Add(new XElement("Generator",
                    new XElement("Name", name),
                    new XElement("Total", totalGeneration)
                ));
            }

            // Process Gas Generators
            var gasGenerators = inputXml.Descendants("Gas").Descendants("GasGenerator");
            foreach (var gasGenerator in gasGenerators)
            {
                string name = gasGenerator.Element("Name")?.Value;
                var generation = gasGenerator.Element("Generation");
                decimal emissionsRating = decimal.Parse(gasGenerator.Element("EmissionsRating").Value);
                decimal emissionFactor = ReferenceDataService.referenceData["Gas"].EmissionFactor;
                decimal totalGeneration = 0;

                if (generation != null)
                {
                    foreach (var day in generation.Elements("Day"))
                    {
                        string date = day.Element("Date")?.Value;
                        decimal energy = decimal.Parse(day.Element("Energy").Value);
                        decimal price = decimal.Parse(day.Element("Price").Value);

                        // Calculate total generation
                        string location = "Gas"; // Since it's a Gas generator
                        decimal valueFactor = ReferenceDataService.referenceData[location].ValueFactor;
                        totalGeneration += energy * price * valueFactor;

                        // Calculate emissions for each day
                        decimal dailyEmissions = energy * emissionsRating * emissionFactor;

                        // if emission for that date is not present, add to dictionary
                        if (!dailyMaxEmissions.ContainsKey(date) || dailyEmissions > dailyMaxEmissions[date].Emission)
                        {
                            dailyMaxEmissions[date] = (name, dailyEmissions);
                        }
                    }
                }

                totalGenerationList.Add(new XElement("Generator",
                    new XElement("Name", name),
                    new XElement("Total", totalGeneration)
                ));
            }

            // Process Coal Generators
            var coalGenerators = inputXml.Descendants("Coal").Descendants("CoalGenerator");
            foreach (var coalGenerator in coalGenerators)
            {
                string name = coalGenerator.Element("Name")?.Value;
                var generation = coalGenerator.Element("Generation");
                decimal totalHeatInput = decimal.Parse(coalGenerator.Element("TotalHeatInput").Value);
                decimal actualNetGeneration = decimal.Parse(coalGenerator.Element("ActualNetGeneration").Value);
                decimal emissionsRating = decimal.Parse(coalGenerator.Element("EmissionsRating").Value);
                decimal emissionFactor = ReferenceDataService.referenceData["Coal"].EmissionFactor;

                decimal totalGeneration = 0;
                if (generation != null)
                {
                    foreach (var day in generation.Elements("Day"))
                    {
                        string date = day.Element("Date")?.Value;
                        decimal energy = decimal.Parse(day.Element("Energy").Value);
                        decimal price = decimal.Parse(day.Element("Price").Value);

                        // Calculate total generation for Coal generator
                        string location = "Coal"; // Since it's a Coal generator
                        decimal valueFactor = ReferenceDataService.referenceData[location].ValueFactor;
                        totalGeneration += energy * price * valueFactor;

                        // Calculate emissions for each day
                        decimal dailyEmissions = energy * emissionsRating * emissionFactor;

                        // if emission for that date is not present, add to dictionary
                        if (!dailyMaxEmissions.ContainsKey(date) || dailyEmissions > dailyMaxEmissions[date].Emission)
                        {
                            dailyMaxEmissions[date] = (name, dailyEmissions);
                        }
                    }
                }

                totalGenerationList.Add(new XElement("Generator",
                    new XElement("Name", name),
                    new XElement("Total", totalGeneration)
                ));

                // Calculate Actual Heat Rate
                decimal actualHeatRate = totalHeatInput / actualNetGeneration;
                actualHeatRates.Add(new XElement("ActualHeatRate",
                    new XElement("Name", name),
                    new XElement("HeatRate", actualHeatRate)
                ));
            }

            // Highest daily emission
            foreach (var entry in dailyMaxEmissions)
            {
                maxEmissionList.Add(new XElement("Day",
                    new XElement("Name", entry.Value.GeneratorName),
                    new XElement("Date", entry.Key),
                    new XElement("Emission", entry.Value.Emission)
                ));
            }

            // Build the final output XML structure
            XElement generationOutput = new XElement("GenerationOutput",
                new XElement("Totals", totalGenerationList),
                new XElement("MaxEmissionGenerators", maxEmissionList),
                new XElement("ActualHeatRates", actualHeatRates)
            );

            return generationOutput;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while processing the generator data.");
            throw;
        }
    }

}
