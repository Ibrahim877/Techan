using Microsoft.AspNetCore.Mvc;
using Techan.Areas.Admin.ViewModels;
using Techan.Areas.Admin.Services.Interfaces;


namespace Techan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidersController : Controller
    {

        private readonly ISliderService _sliderService;

        public SlidersController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }


        public async Task<IActionResult> Index()
        {
            var sliders = await _sliderService.GetAllAsync();
            return View(sliders); 
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(SliderViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var slider = await _sliderService.CreateAsync(model);
                TempData["Success"] = "Slayd uğurla əlavə olundu.";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sliderService.DeleteAsync(id);
                TempData["Success"] = "Slayd uğurla silindi.";
            }
            catch (Exception ex)
            {
                // Loqa yazmaq istəsəniz: _logger.LogError(ex, "Silinmə zamanı xəta");
                TempData["Error"] = $"Silinmə zamanı xəta baş verdi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var slider = await _sliderService.GetByIdAsync(id);

            if (slider == null)
            {
                return NotFound(); // id uyğun slide tapılmazsa 404 qaytar
            }

            var viewModel = new SliderUpdateViewModel
            {
                Id = slider.Id,
                BrandName = slider.BrandName,
                CurrentImage = slider.BrandImage
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SliderUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _sliderService.UpdateAsync(model);
                TempData["Success"] = "Slayd uğurla güncəlləndi.";
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }

    }
}
