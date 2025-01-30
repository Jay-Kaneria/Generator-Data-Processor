using System.Xml.Linq;

namespace GeneratorDataProcessing.Services;

public static class ReportValidationService
{
    /// <summary>
    /// Validates if the root element of the XML is <GenerationReport>
    /// </summary>
    public static bool IsValidGenerationReport(XElement inputXml)
    {
        return inputXml.Name.LocalName == "GenerationReport";
    }
}
