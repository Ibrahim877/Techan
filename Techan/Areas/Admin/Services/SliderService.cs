
using Techan.Areas.Admin.ViewModels;
using Techan.Models;
using Techan.Areas.Admin.Services.Interfaces;
using Techan.Data;
using Microsoft.EntityFrameworkCore;

namespace Techan.Areas.Admin.Services
{
    public class SliderService : ISliderService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        // private readonly AppDbContext _context;

        public SliderService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            // _context = context;
        }

        public async Task<Slider> CreateAsync(SliderViewModel model)
        {
            // ✅ Validasiyalar
            if (string.IsNullOrWhiteSpace(model.BrandName))
                throw new ArgumentException("Brand adı boş və ya yalnız boşluq ola bilməz.");

            if (model.BrandImage == null)
                throw new ArgumentException("Şəkil faylı tələb olunur.");

            string[] allowedTypes = { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(model.BrandImage.ContentType))
                throw new ArgumentException("Yalnız .jpg, .png və .webp şəkillər qəbul olunur.");

            const int maxFileSize = 2 * 1024 * 1024; // 2 MB
            if (model.BrandImage.Length > maxFileSize)
                throw new ArgumentException("Şəklin ölçüsü 2MB-dan çox ola bilməz.");

            // ✅ Faylı yaddaşa yaz
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "sliders");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(model.BrandImage.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.BrandImage.CopyToAsync(stream);
            }

            // ✅ Slider obyektini yarat
            var slider = new Slider
            {
                BrandName = model.BrandName.Trim(),
                BrandImage = "/uploads/sliders/" + uniqueName,
                CreatedAt = DateTime.Now
            };

            _context.Sliders.Add(slider);
            await _context.SaveChangesAsync();

            return slider;
        }

        public async Task<List<Slider>> GetAllAsync()
        {
            return await _context.Sliders
                                 .OrderByDescending(s => s.CreatedAt)
                                 .ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
                throw new ArgumentException("Slide tapılmadı");

            // Faylı sil
            string filePath = Path.Combine(_env.WebRootPath, slider.BrandImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // DB-dən sil
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
        }

        public async Task<Slider> GetByIdAsync(int id)
        {
            return await _context.Sliders.FindAsync(id);
        }

        public async Task UpdateAsync(SliderUpdateViewModel model)
        {
            var slider = await _context.Sliders.FindAsync(model.Id);
            if (slider == null)
                throw new ArgumentException("Slider tapılmadı.");

            bool hasName = !string.IsNullOrWhiteSpace(model.BrandName);
            bool hasImage = model.BrandImage != null;

            if (!hasName && !hasImage)
                throw new ArgumentException("Ən azı bir dəyişiklik tələb olunur (ad və ya şəkil).");

            if (hasName)
                slider.BrandName = model.BrandName.Trim();

            if (hasImage)
            {
                // ✅ Validasiya
                string[] allowedTypes = { "image/jpeg", "image/png", "image/webp" };
                if (!allowedTypes.Contains(model.BrandImage.ContentType))
                    throw new ArgumentException("Yalnız .jpg, .png və .webp şəkillər qəbul olunur.");

                const int maxFileSize = 2 * 1024 * 1024; // 2MB
                if (model.BrandImage.Length > maxFileSize)
                    throw new ArgumentException("Şəkil 2MB-dan böyük ola bilməz.");

                // ✅ Yeni şəkili yaddaşa yaz
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "sliders");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BrandImage.FileName);
                string newFilePath = Path.Combine(uploadsFolder, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await model.BrandImage.CopyToAsync(stream);
                }

                // ✅ Köhnə şəkili sil
                if (!string.IsNullOrWhiteSpace(slider.BrandImage))
                {
                    string oldImagePath = Path.Combine(_env.WebRootPath, slider.BrandImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                // Yeni şəkil path
                slider.BrandImage = "/uploads/sliders/" + newFileName;
            }

            slider.CreatedAt = DateTime.Now;

            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();
        }

    }
}
