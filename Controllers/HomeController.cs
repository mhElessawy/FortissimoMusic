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
        private readonly IWebHostEnvironment _env;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var dbSliderImages = await _context.SliderImages
                .Where(s => s.IsActive)
                .OrderBy(s => s.Order)
                .ToListAsync();

            var staticImages = new List<SliderImage>();
            var imagesFolder = Path.Combine(_env.WebRootPath, "images");
            string[] extensions = { ".jpg", ".jpeg", ".png", ".webp" };

            for (int i = 1; i <= 10; i++)
            {
                foreach (var ext in extensions)
                {
                    var filePath = Path.Combine(imagesFolder, $"{i}{ext}");
                    if (System.IO.File.Exists(filePath))
                    {
                        staticImages.Add(new SliderImage
                        {
                            Id = -i,
                            Title = $"Fortissimo Music ",
                            ImageUrl = $"/images/{i}{ext}",
                            Order = i,
                            IsActive = true
                        });
                        break;
                    }
                }
            }

            var sliderImages = staticImages.Any() ? staticImages : dbSliderImages;

            var upcomingEvents = await _context.Events
                .Where(e => e.IsActive && e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToListAsync();

            ViewBag.SliderImages = sliderImages;
            ViewBag.UpcomingEvents = upcomingEvents;

            var galleryPhotos = await _context.GalleryPhotos.OrderBy(p => p.Order).ToListAsync();
            var photoDir = Path.Combine(_env.WebRootPath, "images", "photo");
            if (Directory.Exists(photoDir))
            {
                var dbFileNames = galleryPhotos.Select(p => p.FileName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                string[] imgExts = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var folderFiles = Directory.GetFiles(photoDir)
                    .Where(f => imgExts.Contains(Path.GetExtension(f).ToLower()))
                    .Select(Path.GetFileName)
                    .Where(f => !dbFileNames.Contains(f!))
                    .ToList();
                if (folderFiles.Any())
                {
                    int nextOrder = galleryPhotos.Count > 0 ? galleryPhotos.Max(p => p.Order) + 1 : 1;
                    var extraPhotos = galleryPhotos.ToList();
                    foreach (var file in folderFiles)
                    {
                        extraPhotos.Add(new GalleryPhoto { FileName = file!, Order = nextOrder++ });
                    }
                    galleryPhotos = extraPhotos.OrderBy(p => p.Order).ToList();
                }
            }
            ViewBag.GalleryPhotos = galleryPhotos;

            var videos = await _context.VideoClips.OrderBy(v => v.Order).ToListAsync();
            var videoDir = Path.Combine(_env.WebRootPath, "Vedio");
            if (Directory.Exists(videoDir))
            {
                var dbVideoNames = videos.Select(v => v.FileName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                string[] vidExts = { ".mp4", ".webm", ".ogg", ".mov", ".avi" };
                var folderVideos = Directory.GetFiles(videoDir)
                    .Where(f => vidExts.Contains(Path.GetExtension(f).ToLower()))
                    .Select(Path.GetFileName)
                    .Where(f => !dbVideoNames.Contains(f!))
                    .ToList();
                if (folderVideos.Any())
                {
                    int nextOrder = videos.Count > 0 ? videos.Max(v => v.Order) + 1 : 1;
                    var allVideos = videos.ToList();
                    foreach (var file in folderVideos)
                    {
                        allVideos.Add(new VideoClip
                        {
                            FileName = file!,
                            Title = Path.GetFileNameWithoutExtension(file!),
                            Order = nextOrder++
                        });
                    }
                    videos = allVideos.OrderBy(v => v.Order).ToList();
                }
            }
            ViewBag.Videos = videos;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Gallery()
        {
            var photos = await _context.GalleryPhotos.OrderBy(p => p.Order).ToListAsync();
            return View(photos);
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