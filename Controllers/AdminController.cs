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
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<AdminController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.EventsCount = await _context.Events.CountAsync();
            ViewBag.MessagesCount = await _context.ContactMessages.Where(m => !m.IsRead).CountAsync();

            ViewBag.PhotosCount = await _context.GalleryPhotos.CountAsync();
            ViewBag.VideosCount = await _context.VideoClips.CountAsync();
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

        // ===== GALLERY PHOTOS =====
        public async Task<IActionResult> GalleryPhotos()
        {
            var photoDir = Path.Combine(_env.WebRootPath, "images", "photo");
            if (Directory.Exists(photoDir))
            {
                var dbFileNames = (await _context.GalleryPhotos.Select(p => p.FileName).ToListAsync())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                string[] imgExts = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var newFiles = Directory.GetFiles(photoDir)
                    .Where(f => imgExts.Contains(Path.GetExtension(f).ToLower()))
                    .Select(Path.GetFileName)
                    .Where(f => !dbFileNames.Contains(f!))
                    .ToList();
                if (newFiles.Any())
                {
                    int maxOrder = await _context.GalleryPhotos.MaxAsync(p => (int?)p.Order) ?? 0;
                    foreach (var file in newFiles)
                    {
                        _context.GalleryPhotos.Add(new GalleryPhoto
                        {
                            FileName = file!,
                            Order = ++maxOrder,
                            UploadedAt = DateTime.Now
                        });
                    }
                    await _context.SaveChangesAsync();
                }
            }
            var photos = await _context.GalleryPhotos.OrderBy(p => p.Order).ToListAsync();
            return View(photos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadGalleryPhoto(IFormFile imageFile, string? title, int order = 0)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    var uploadsDir = Path.Combine(_env.WebRootPath, "images", "photo");
                    Directory.CreateDirectory(uploadsDir);
                    var ext = Path.GetExtension(imageFile.FileName).ToLower();
                    var fileName = Guid.NewGuid() + ext;
                    var filePath = Path.Combine(uploadsDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    _context.GalleryPhotos.Add(new GalleryPhoto
                    {
                        FileName = fileName,
                        Title = title,
                        Order = order,
                        UploadedAt = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم رفع الصورة بنجاح!";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload gallery photo.");
                    TempData["Error"] = "فشل رفع الصورة: " + ex.Message;
                }
            }
            return RedirectToAction(nameof(GalleryPhotos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryPhoto(int id)
        {
            var photo = await _context.GalleryPhotos.FindAsync(id);
            if (photo != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, "images", "photo", photo.FileName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                _context.GalleryPhotos.Remove(photo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الصورة.";
            }
            return RedirectToAction(nameof(GalleryPhotos));
        }

        // ===== VIDEO CLIPS =====
        public async Task<IActionResult> Videos()
        {
            var videoDir = Path.Combine(_env.WebRootPath, "Vedio");
            if (Directory.Exists(videoDir))
            {
                var dbFileNames = (await _context.VideoClips.Select(v => v.FileName).ToListAsync())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                string[] vidExts = { ".mp4", ".webm", ".ogg", ".mov", ".avi" };
                var newFiles = Directory.GetFiles(videoDir)
                    .Where(f => vidExts.Contains(Path.GetExtension(f).ToLower()))
                    .Select(Path.GetFileName)
                    .Where(f => !dbFileNames.Contains(f!))
                    .ToList();
                if (newFiles.Any())
                {
                    int maxOrder = await _context.VideoClips.MaxAsync(v => (int?)v.Order) ?? 0;
                    foreach (var file in newFiles)
                    {
                        _context.VideoClips.Add(new VideoClip
                        {
                            FileName = file!,
                            Title = Path.GetFileNameWithoutExtension(file!),
                            Order = ++maxOrder,
                            UploadedAt = DateTime.Now
                        });
                    }
                    await _context.SaveChangesAsync();
                }
            }
            var videos = await _context.VideoClips.OrderBy(v => v.Order).ToListAsync();
            return View(videos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile, string title, string? description, int order = 0)
        {
            if (videoFile != null && videoFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "Vedio");
                Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(videoFile.FileName).ToLower();
                var fileName = Guid.NewGuid() + ext;
                var filePath = Path.Combine(uploadsDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await videoFile.CopyToAsync(stream);

                _context.VideoClips.Add(new VideoClip
                {
                    FileName = fileName,
                    Title = title,
                    Description = description,
                    Order = order,
                    UploadedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم رفع الفيديو بنجاح!";
            }
            return RedirectToAction(nameof(Videos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _context.VideoClips.FindAsync(id);
            if (video != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, "Vedio", video.FileName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                _context.VideoClips.Remove(video);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الفيديو.";
            }
            return RedirectToAction(nameof(Videos));
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
