using Techan.Areas.Admin.ViewModels;
using Techan.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Techan.Areas.Admin.Services.Interfaces
{
    public interface ISliderService
    {
        Task<Slider> CreateAsync(SliderViewModel model);

        Task<List<Slider>> GetAllAsync();

        Task DeleteAsync(int id);

        Task<Slider> GetByIdAsync(int id);

        Task UpdateAsync(SliderUpdateViewModel model);

    }
}
