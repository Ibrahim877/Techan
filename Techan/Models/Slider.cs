using System;
namespace Techan.Models
{
    public class Slider
    {
        public int Id { get; set; }

        public string BrandName { get; set; }


        public string BrandImage { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
