
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

        public SliderService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Slider> CreateAsync(SliderViewModel model)
        {
            await ValidateSliderDataAsync(model.BrandName, model.BrandImage);

            string savedPath = await UploadFileAsync(model.BrandImage);

            var slider = new Slider
            {
                BrandName = model.BrandName.Trim(),
                BrandImage = savedPath,
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


            DeleteFile(slider.BrandImage);


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


            await ValidateSliderDataAsync(model.BrandName, model.BrandImage, model.Id);

            slider.BrandName = model.BrandName;

            if (model.BrandImage != null)
            {
                string savedPath = await UploadFileAsync(model.BrandImage);


                if (!string.IsNullOrWhiteSpace(slider.BrandImage))
                {
                    DeleteFile(slider.BrandImage);
                }

                slider.BrandImage = savedPath;
            }

            slider.CreatedAt = DateTime.Now;

            _context.Sliders.Update(slider);
            await _context.SaveChangesAsync();
        }

        public void DeleteFile(string relativePath)
        {
            string fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        public async Task ValidateSliderDataAsync(string BrandName, IFormFile? BrandImage, int? id = null)
        {
            string[] allowedTypes = { "image/jpeg", "image/png", "image/webp" };
            const int maxFileSize = 2 * 1024 * 1024; // 2 MB

            if (string.IsNullOrWhiteSpace(BrandName))
                throw new ArgumentException("Brand adı boş və ya yalnız boşluq ola bilməz.");


            bool isExist = false;

            if (id == null)
                isExist = await _context.Sliders.AnyAsync(s => s.BrandName.ToLower().Trim() == BrandName.ToLower().Trim());
            else
                isExist = await _context.Sliders.AnyAsync(s => s.Id != id && s.BrandName.ToLower().Trim() == BrandName.ToLower().Trim());


            if (isExist)
                throw new ArgumentException("Bu adda artıq bir slide mövcuddur.");

            if (BrandImage == null && id == null)
                throw new ArgumentException("Şəkil faylı tələb olunur.");

            if (BrandImage != null)
                if (BrandImage.Length > maxFileSize)
                    throw new ArgumentException("Şəklin ölçüsü 2MB-dan çox ola bilməz.");

            if (BrandImage != null)
                if (!allowedTypes.Contains(BrandImage.ContentType))
                    throw new ArgumentException("Yalnız .jpg, .png və .webp şəkillər qəbul olunur.");

        }


        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "sliders");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/sliders/{uniqueName}";
        }


    }
}
