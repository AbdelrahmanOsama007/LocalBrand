using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class MyAppContext: IdentityDbContext<AppUser>

    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<ProductColorImage> ProductColorImages { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(pi => !pi.IsDeleted);
            modelBuilder.Entity<OrderDetails>().HasQueryFilter(od => !od.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<SubCategory>().HasQueryFilter(s => !s.IsDeleted);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<ProductColorImage>()
                .HasOne(pci => pci.Product)
                .WithMany(p => p.ProductColorImages)
                .HasForeignKey(pci => pci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductColorImage>()
                .HasOne(pci => pci.Color)
                .WithMany(c => c.ProductColorImages)
                .HasForeignKey(pci => pci.ColorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductImage>()
                .HasMany(pi => pi.ProductColorImages)
                .WithOne(pci => pci.Image)
                .HasForeignKey(pci => pci.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            List<Category> categories = new List<Category>() { new Category() { Id = 1, Name = "Men" },
                                                             {new Category(){ Id = 2 , Name = "Women"}}};

            List<SubCategory> subcategories = new List<SubCategory>() { new SubCategory() { Id =1, Name= "T-Shirt", CategoryId = 1},
                                                               new SubCategory() { Id =2, Name= "Hoddie", CategoryId = 1},
                                                               new SubCategory() { Id =3, Name= "Short", CategoryId = 1}};

            List<Color> colors = new List<Color>() { new Color() { Id = 1, ColorName = ColorEnum.black.ToString(), ColorCode = "#000"},
                                                     new Color() { Id = 2, ColorName = ColorEnum.white.ToString(), ColorCode = "#fff"},
                                                     new Color() { Id = 3, ColorName = ColorEnum.red.ToString(), ColorCode = "#FF0000"},
                                                     new Color() { Id = 4, ColorName = ColorEnum.blue.ToString(), ColorCode = "#0000FF"}};

            List<Size> sizes = new List<Size>() { new Size() { Id = 1, SizeName = SizeEnum.Small.ToString(), SizeKey = "S"},
                                                  new Size() {Id = 2, SizeName = SizeEnum.Medium.ToString(), SizeKey = "M"},
                                                  new Size() {Id = 3, SizeName = SizeEnum.Large.ToString(), SizeKey = "L"},
                                                  new Size() {Id = 4, SizeName = SizeEnum.XLarge.ToString(), SizeKey = "XL"},
                                                  new Size() {Id = 5, SizeName = SizeEnum.XXLarge.ToString(), SizeKey = "XXL"},
                                                  new Size() {Id = 6, SizeName = SizeEnum.Size32.ToString(), SizeKey = "32"},
                                                  new Size() {Id = 7, SizeName = SizeEnum.Size34.ToString(), SizeKey = "34"},
                                                  new Size() {Id = 8, SizeName = SizeEnum.Size36.ToString(), SizeKey = "36"},
                                                  new Size() {Id = 9, SizeName = SizeEnum.Size38.ToString(), SizeKey = "38"},
                                                  new Size() {Id = 10, SizeName = SizeEnum.Size40.ToString(), SizeKey = "40"}};

            modelBuilder.Entity<Category>()
            .HasData(categories);

            modelBuilder.Entity<SubCategory>()
                .HasData(subcategories);

            modelBuilder.Entity<Color>()
                .HasData(colors);

            modelBuilder.Entity<Size>()
                .HasData(sizes);
        }
    }
}