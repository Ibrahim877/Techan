using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Techan.Areas.Admin.ViewModels
{
    public class SliderUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand adı boş ola bilməz.")]
        public string BrandName { get; set; }

        // Yeni şəkil yükləmək üçün
        public IFormFile? BrandImage { get; set; }

        // Mövcud şəkili göstərmək üçün (db-dən gəlir)
        public string CurrentImage { get; set; }
    }
}
