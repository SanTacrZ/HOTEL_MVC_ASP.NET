using System.Diagnostics;
using hotel_web_final.Models;
using hotel_web_final.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace hotel_web_final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HotelService _hotelService;

        public HomeController(ILogger<HomeController> logger, HotelService hotelService)
        {
            _logger = logger;
            _hotelService = hotelService;
        }

        public IActionResult Index()
        {
            var hotel = _hotelService.ObtenerHotel();
            return View(hotel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
