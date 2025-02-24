namespace DataParseApp.Models;

public class XmlDataItem
{
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsHeader { get; set; }
    public string HeaderValue { get; set; } = string.Empty;
}