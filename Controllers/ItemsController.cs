using DataParseApp.Models;
using DataParseApp.Services;
using Microsoft.AspNetCore.Mvc;
namespace DataParseApp.Controllers
{
    public class ItemsController : Controller
    {
        private readonly XmlService _xmlService;

        public ItemsController(XmlService xmlService)
        {
            _xmlService = xmlService;
        }
        public async Task<IActionResult> Index(string xmlUrl)
        {
            try
            {
                var items = await _xmlService.GetItemsFromXmlAsync(xmlUrl);
                var model = new MainItem
                {
                    Items = items,
                    SelectedUrl = xmlUrl
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}
