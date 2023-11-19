using Microsoft.EntityFrameworkCore;
using testPronia.Models;

namespace testPronia.DAL
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) 
        {
        
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Slide> Slides { get; set; }




    }
}
