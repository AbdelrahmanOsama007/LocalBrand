using Business.Categories.Interfaces;
using Business.Categories.Validator;
using Business.Orders.Interfaces;
using Business.Orders.Validator;
using Business.Products.Interfaces;
using Business.Products.Validator;
using Infrastructure.Context;
using Infrastructure.GenericRepository;
using Infrastructure.IgenericRepository;
using Infrastructure.IGenericRepository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Model.Models;
using System.Security.Claims;
using System.Text;

namespace LocalBrand
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllDomains", p =>
                {
                    p.AllowAnyOrigin();
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                });
            });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LocalBrand API", Version = "v1" });
            });

            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            builder.Services.AddScoped<IGenericRepository<ProductImage>, GenericRepository<ProductImage>>();
            builder.Services.AddScoped<IGenericRepository<Stock>, GenericRepository<Stock>>();
            builder.Services.AddScoped<IGenericRepository<ProductColorImage>, GenericRepository<ProductColorImage>>();
            builder.Services.AddScoped<IGenericRepository<Order>, GenericRepository<Order>>();
            builder.Services.AddScoped<IGenericRepository<OrderDetails>, GenericRepository<OrderDetails>>();
            builder.Services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
            builder.Services.AddScoped<IGenericRepository<SubCategory>, GenericRepository<SubCategory>>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<MyAppContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MyAppContext>()
                .AddDefaultTokenProviders();
            // Configure JWT Authentication
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role // Add this line
                };

                options.UseLazyLoadingProxies().UseSqlServer(connectionString);
            });
            //////////end////////////



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Show detailed error pages in development
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocalBrand API V1");
                });
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        if (exceptionHandlerPathFeature?.Error != null)
                        {
                            logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception occurred");
                        }
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllDomains");
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
            static async Task SeedAdminUser(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                var adminUsername = "admin"; // static username
                var adminPassword = "Admin@123"; // static password

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                var adminUser = await userManager.FindByNameAsync(adminUsername);
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = adminUsername,
                        Email = "admin@example.com",
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}
