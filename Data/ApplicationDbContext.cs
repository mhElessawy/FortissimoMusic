using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fortissimo.Models;

namespace Fortissimo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SliderImage>().HasData(
                new SliderImage { Id = 1, Title = "مرحباً بكم في فورتيسيمو", ImageUrl = "/images/slider/slide1.jpg", Order = 1, IsActive = true },
                new SliderImage { Id = 2, Title = "تعلم البيانو", ImageUrl = "/images/slider/slide2.jpg", Order = 2, IsActive = true },
                new SliderImage { Id = 3, Title = "تعلم الكلارنيت", ImageUrl = "/images/slider/slide3.jpg", Order = 3, IsActive = true }
            );

            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "حفل موسيقي ربيعي",
                    Description = "انضم إلينا في حفلنا الموسيقي السنوي لطلابنا المتميزين",
                    EventDate = new DateTime(2026, 7, 15, 18, 0, 0),
                    Location = "قاعة الموسيقى الكبرى - القاهرة",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Event
                {
                    Id = 2,
                    Title = "ورشة عمل البيانو",
                    Description = "ورشة عمل مكثفة لتعلم أساسيات العزف على البيانو",
                    EventDate = new DateTime(2026, 8, 1, 10, 0, 0),
                    Location = "أكاديمية فورتيسيمو للموسيقى",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
