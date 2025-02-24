namespace DataParseApp.Models
{
    public class MainItem
    {
        public List<XmlDataItem> Items { get; set; } = new();
        public string SelectedUrl { get; set; } = string.Empty; 
    }
}
