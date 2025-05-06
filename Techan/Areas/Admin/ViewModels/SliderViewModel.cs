using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Techan.Areas.Admin.ViewModels
{
    public class SliderViewModel
    {
        [Required]
        public string BrandName { get; set; }

        [Required]
        public IFormFile BrandImage { get; set; }
    }
}
