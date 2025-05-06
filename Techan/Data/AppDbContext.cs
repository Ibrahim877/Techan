using Techan.Models;
using Microsoft.EntityFrameworkCore;

namespace Techan.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
