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
                                                             {new Category(){ Id = 2 , Name = "Women"}},
                                                             {new Category(){Id = 3, Name = "Unisex" }},
                                                             {new Category(){Id = 4, Name = "Accessories" }}};

            List<SubCategory> Menssubcategories = new List<SubCategory>() { new SubCategory() { Id =1, Name= "MenT-Shirt", CategoryId = 1},
                                                               new SubCategory() { Id =2, Name= "MenHoddie", CategoryId = 1},
                                                               new SubCategory() { Id =3, Name= "MenShort", CategoryId = 1}};

            List<SubCategory> Womenssubcategories = new List<SubCategory>() { new SubCategory() { Id =4, Name= "WomenT-Shirt", CategoryId = 2},
                                                               new SubCategory() { Id =5, Name= "WomenHoddie", CategoryId = 2},
                                                               new SubCategory() { Id =6, Name= "Top", CategoryId = 2}};

            List<SubCategory> Unisexsubcategories = new List<SubCategory>() { new SubCategory() { Id = 7, Name = "UnisexHoddie", CategoryId = 3 }};

            List<SubCategory> accessoriessubcategories = new List<SubCategory>() { new SubCategory() { Id = 8, Name = "Perfume", CategoryId = 4 }};

            List<Color> colors = new List<Color>() { new Color() { Id = 1, ColorName = ColorEnum.Black.ToString(), ColorCode = "#000000"},
                                                     new Color() { Id = 2, ColorName = ColorEnum.White.ToString(), ColorCode = "#ffffff"},
                                                     new Color() { Id = 3, ColorName = ColorEnum.Red.ToString(), ColorCode = "#FF0000"},
                                                     new Color() { Id = 4, ColorName = ColorEnum.Blue.ToString(), ColorCode = "#4169e1"},
                                                     new Color() { Id = 5, ColorName = ColorEnum.Avocado.ToString(), ColorCode = "#7ea122"},
                                                     new Color() { Id = 6, ColorName = ColorEnum.Beige.ToString(), ColorCode = "#ede8d0"},
                                                     new Color() { Id = 7, ColorName = ColorEnum.Brown.ToString(), ColorCode = "#964B00"},
                                                     new Color() { Id = 8, ColorName = ColorEnum.MidNight.ToString(), ColorCode = "#152238"},
                                                     new Color() { Id = 9, ColorName = ColorEnum.Grey.ToString(), ColorCode = "#808080"},
                                                     new Color() { Id = 10, ColorName = ColorEnum.DarkGreen.ToString(), ColorCode = "#003200"},
                                                     new Color() { Id = 11, ColorName = ColorEnum.BabyBlue.ToString(), ColorCode = "#d1e5f4"},
                                                     new Color() { Id = 12, ColorName = ColorEnum.Pink.ToString(), ColorCode = "#ff69b4"},
                                                     new Color() { Id = 13, ColorName = ColorEnum.OffWhite.ToString(), ColorCode = "#fffff2"},};

            List<Size> sizes = new List<Size>() { new Size() { Id = 1, SizeName = SizeEnum.Small.ToString(), SizeKey = "S"},
                                                  new Size() {Id = 2, SizeName = SizeEnum.Medium.ToString(), SizeKey = "M"},
                                                  new Size() {Id = 3, SizeName = SizeEnum.Large.ToString(), SizeKey = "L"},
                                                  new Size() {Id = 4, SizeName = SizeEnum.XLarge.ToString(), SizeKey = "XL"},
                                                  new Size() {Id = 5, SizeName = SizeEnum.XXLarge.ToString(), SizeKey = "XXL"},
                                                  new Size() {Id = 6, SizeName = SizeEnum.Size32.ToString(), SizeKey = "32"},
                                                  new Size() {Id = 7, SizeName = SizeEnum.Size34.ToString(), SizeKey = "34"},
                                                  new Size() {Id = 8, SizeName = SizeEnum.Size36.ToString(), SizeKey = "36"},
                                                  new Size() {Id = 9, SizeName = SizeEnum.Size38.ToString(), SizeKey = "38"},
                                                  new Size() {Id = 10, SizeName = SizeEnum.Size40.ToString(), SizeKey = "40"},
                                                  new Size() {Id = 11, SizeName = SizeEnum.S100ML.ToString(), SizeKey = "100 ML"}};

            modelBuilder.Entity<Category>()
            .HasData(categories);

            modelBuilder.Entity<SubCategory>()
                .HasData(Menssubcategories);

            modelBuilder.Entity<SubCategory>()
            .HasData(Womenssubcategories);

            modelBuilder.Entity<SubCategory>()
            .HasData(Unisexsubcategories);

            modelBuilder.Entity<SubCategory>()
            .HasData(accessoriessubcategories);

            modelBuilder.Entity<Color>()
                .HasData(colors);

            modelBuilder.Entity<Size>()
                .HasData(sizes);
        }
    }
}