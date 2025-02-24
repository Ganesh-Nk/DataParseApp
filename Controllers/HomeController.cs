using DataParseApp.Models;
using DataParseApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataParseApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly XmlService _xmlService;
        private const int PageSize = 10;

        public HomeController(XmlService xmlService)
        {
            _xmlService = xmlService;
        }

        public IActionResult Index(string? xmlUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(xmlUrl))
                {
                    xmlUrl = "https://receiptservice.egretail.cloud/ARTSPOSLogSchema/2.2.1";
                }
                ViewBag.SelectedUrl = xmlUrl;
                var datasets = _xmlService.ParseMultipleDatasets(xmlUrl);
                return View(datasets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Index action: {ex.Message}");
                return View(new List<Dictionary<string, List<XmlData>>>());
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
