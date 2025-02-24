using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using DataParseApp.Models;

namespace DataParseApp.Services
{
    public class XmlService
    {
        private readonly HttpClient _httpClient;

        public XmlService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<XmlDataItem>> GetItemsFromXmlAsync(string url)
        {
            var xmlDoc = await FetchXmlFromUrl(url);
            return ParseXmlData(xmlDoc);
        }

        private async Task<XDocument> FetchXmlFromUrl(string url)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/xml");
            string xmlContent = await _httpClient.GetStringAsync(url);
            return XDocument.Parse(xmlContent);
        }

        private List<XmlDataItem> ParseXmlData(XDocument xmlDoc)
        {
            var items = new List<XmlDataItem>();
            
            var simpleTypes = xmlDoc.Descendants(XName.Get("simpleType", "http://www.w3.org/2001/XMLSchema"));

            foreach (var simpleType in simpleTypes)
            {
                var typeName = simpleType.Attribute("name")?.Value ?? string.Empty;
                var typeDoc = simpleType
                    .Descendants(XName.Get("documentation", "http://www.w3.org/2001/XMLSchema"))
                    .FirstOrDefault()?.Value?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(typeName) && !string.IsNullOrWhiteSpace(typeDoc))
                {
                    items.Add(new XmlDataItem
                    {
                        Value = typeName,
                        Description = typeDoc,
                        IsHeader = true,
                        HeaderValue = typeName
                    });
                         var enumerations = simpleType
                        .Descendants(XName.Get("enumeration", "http://www.w3.org/2001/XMLSchema"))
                        .Where(e => e.Descendants(XName.Get("documentation", "http://www.w3.org/2001/XMLSchema"))
                                    .Any(d => !string.IsNullOrWhiteSpace(d.Value)));

                    foreach (var enumeration in enumerations)
                    {
                        var value = enumeration.Attribute("value")?.Value ?? string.Empty;
                        var documentation = enumeration
                            .Descendants(XName.Get("documentation", "http://www.w3.org/2001/XMLSchema"))
                            .FirstOrDefault()?.Value?.Trim() ?? string.Empty;

                        items.Add(new XmlDataItem
                        {
                            Value = value,
                            Description = documentation,
                            IsHeader = false,
                            HeaderValue = typeName
                        });
                    }
                }
            }
            
            return items;
        }
        public List<Dictionary<string, List<XmlData>>> ParseMultipleDatasets(string xmlUrl)
        {
            var result = new List<Dictionary<string, List<XmlData>>>();
            var datasetResult = new Dictionary<string, List<XmlData>>();
            
            try
            {
              
                using (var client = new HttpClient())
                {
                    var xmlContent = client.GetStringAsync(xmlUrl).Result;
                    Console.WriteLine("XML content fetched successfully");              
                    var doc = XDocument.Parse(xmlContent);
                    Console.WriteLine("XML parsed successfully");
                    XNamespace xs = "http://www.w3.org/2001/XMLSchema";                 
                    var simpleTypes = doc.Descendants(xs + "simpleType");
                    Console.WriteLine($"Found {simpleTypes.Count()} simpleType elements");
                    foreach (var simpleType in simpleTypes)
                    {
                        var typeName = simpleType.Attribute("name")?.Value;
                        if (!string.IsNullOrWhiteSpace(typeName))
                        {
                            Console.WriteLine($"Processing type: {typeName}");
                            var records = new List<XmlData>();             
                            var typeDoc = simpleType
                                .Descendants(xs + "documentation")
                                .FirstOrDefault()?.Value?.Trim();
                            records.Add(new XmlData
                            {
                                Name = typeName,
                                Value = typeDoc ?? string.Empty,
                                IsHeader = true
                            });
                            var enumerations = simpleType.Descendants(xs + "enumeration");
                            var hasValidData = false;
                            foreach (var enumeration in enumerations)
                            {
                                var value = enumeration.Attribute("value")?.Value;
                                var documentation = enumeration
                                    .Descendants(xs + "documentation")
                                    .FirstOrDefault()?.Value?.Trim();

                                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(documentation))
                                {
                                    records.Add(new XmlData
                                    {
                                        Name = value,
                                        Value = documentation ?? string.Empty,
                                        IsHeader = false
                                    });
                                    hasValidData = true;
                                    Console.WriteLine($"Added enumeration: {value}");
                                }
                            }
                            if (hasValidData)
                            {
                                datasetResult[typeName] = records;
                                Console.WriteLine($"Added {records.Count} records for type {typeName}");
                            }
                        }
                    }
                    if (datasetResult.Any())
                    {
                        result.Add(datasetResult);
                        Console.WriteLine($"Added dataset with {datasetResult.Count} types");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing XML: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            
            return result;
        }
    }
}
