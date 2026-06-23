using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fortissimo.Data;
using Fortissimo.Models;

namespace Fortissimo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var sliderImages = await _context.SliderImages
                .Where(s => s.IsActive)
                .OrderBy(s => s.Order)
                .ToListAsync();

            var upcomingEvents = await _context.Events
                .Where(e => e.IsActive && e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToListAsync();

            ViewBag.SliderImages = sliderImages;
            ViewBag.UpcomingEvents = upcomingEvents;
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Events()
        {
            var events = await _context.Events
                .Where(e => e.IsActive)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactMessages.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إرسال رسالتك بنجاح! سنتواصل معك قريباً.";
                return RedirectToAction(nameof(Contact));
            }
            return View(model);
        }

        public IActionResult CV()
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
