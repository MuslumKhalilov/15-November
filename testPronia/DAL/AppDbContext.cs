﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using testPronia.Models;

namespace testPronia.DAL
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) 
        {
        
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Slide> Slides { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet <ProductSize> ProductSizes { get; set; }
        public DbSet<Size> Sizes {  get; set; }  
        public DbSet<Category> Category { get; set; }
        public DbSet<Settings> Settings { get; set; }






    }
}
