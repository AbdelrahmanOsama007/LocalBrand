using Business.Cart.Interfaces;
using Business.Cart.Validator;
using Business.Categories.Interfaces;
using Business.Categories.Validator;
using Business.Orders.Interfaces;
using Business.Orders.Validator;
using Business.Products.Interfaces;
using Business.Products.Validator;
using Business.Wishlist.Interfaces;
using Business.Wishlist.Validator;
using Infrastructure.Context;
using Infrastructure.GenericRepository;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

            // Configure CORS to allow all domains
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllDomains", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            // Add services to the container
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LocalBrand API", Version = "v1" });
            });

            // Add logging
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Register application services and repositories
            RegisterServices(builder.Services);

            // Configure the DbContext with SQL Server
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<MyAppContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString));

            // Configure Identity services
            ConfigureIdentity(builder.Services);

            // Configure JWT Authentication
            ConfigureJwtAuthentication(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            ConfigureHttpRequestPipeline(app);

            // Run the application
            app.Run();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Application services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ICartService, CartService>();

            // Repositories
            services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            services.AddScoped<IGenericRepository<ProductImage>, GenericRepository<ProductImage>>();
            services.AddScoped<IGenericRepository<Stock>, GenericRepository<Stock>>();
            services.AddScoped<IGenericRepository<ProductColorImage>, GenericRepository<ProductColorImage>>();
            services.AddScoped<IGenericRepository<Order>, GenericRepository<Order>>();
            services.AddScoped<IGenericRepository<OrderDetails>, GenericRepository<OrderDetails>>();
            services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
            services.AddScoped<IGenericRepository<SubCategory>, GenericRepository<SubCategory>>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MyAppContext>()
                .AddDefaultTokenProviders();
        }

        private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role // Ensure this line is present
                };
            });
        }

        private static void ConfigureHttpRequestPipeline(WebApplication app)
        {
            // Enable exception handling and configure Swagger in Development mode
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            // app.UseHttpsRedirection(); // Uncomment if using HTTPS
            app.UseCors("AllowAllDomains");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
