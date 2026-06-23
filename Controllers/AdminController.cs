using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fortissimo.Data;
using Fortissimo.Models;

namespace Fortissimo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.EventsCount = await _context.Events.CountAsync();
            ViewBag.MessagesCount = await _context.ContactMessages.Where(m => !m.IsRead).CountAsync();
            ViewBag.SliderCount = await _context.SliderImages.CountAsync();
            return View();
        }

        // ===== EVENTS =====
        public async Task<IActionResult> Events()
        {
            var events = await _context.Events.OrderByDescending(e => e.EventDate).ToListAsync();
            return View(events);
        }

        public IActionResult CreateEvent() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event model, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsDir = Path.Combine(_env.WebRootPath, "images", "events");
                    Directory.CreateDirectory(uploadsDir);
                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await imageFile.CopyToAsync(stream);
                    model.ImageUrl = "/images/events/" + fileName;
                }
                model.CreatedAt = DateTime.Now;
                _context.Events.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة الفعالية بنجاح!";
                return RedirectToAction(nameof(Events));
            }
            return View(model);
        }

        public async Task<IActionResult> EditEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, Event model, IFormFile? imageFile)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsDir = Path.Combine(_env.WebRootPath, "images", "events");
                    Directory.CreateDirectory(uploadsDir);
                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await imageFile.CopyToAsync(stream);
                    model.ImageUrl = "/images/events/" + fileName;
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم تحديث الفعالية بنجاح!";
                return RedirectToAction(nameof(Events));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الفعالية.";
            }
            return RedirectToAction(nameof(Events));
        }

        // ===== SLIDER IMAGES =====
        public async Task<IActionResult> SliderImages()
        {
            var images = await _context.SliderImages.OrderBy(s => s.Order).ToListAsync();
            return View(images);
        }

        public IActionResult CreateSlider() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSlider(SliderImage model, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "images", "slider");
                Directory.CreateDirectory(uploadsDir);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                model.ImageUrl = "/images/slider/" + fileName;
                ModelState.Remove("ImageUrl");
            }

            if (ModelState.IsValid)
            {
                _context.SliderImages.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة الصورة بنجاح!";
                return RedirectToAction(nameof(SliderImages));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var img = await _context.SliderImages.FindAsync(id);
            if (img != null)
            {
                _context.SliderImages.Remove(img);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الصورة.";
            }
            return RedirectToAction(nameof(SliderImages));
        }

        // ===== MESSAGES =====
        public async Task<IActionResult> Messages()
        {
            var messages = await _context.ContactMessages.OrderByDescending(m => m.SentAt).ToListAsync();
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg != null)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Messages));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg != null)
            {
                _context.ContactMessages.Remove(msg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الرسالة.";
            }
            return RedirectToAction(nameof(Messages));
        }
    }
}
