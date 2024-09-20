using Business.Categories.Interfaces;
using Business.Categories.Validator;
using Business.Orders.Interfaces;
using Business.Orders.Validator;
using Business.Products.Interfaces;
using Business.Products.Validator;
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

            // Register services and repositories
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

            // Add DbContext with SQL Server support
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<MyAppContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString));

            // Identity services
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MyAppContext>()
                .AddDefaultTokenProviders();

            // Configure JWT Authentication
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
            builder.Services.AddAuthentication(options =>
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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

            app.UseHttpsRedirection();
            app.UseCors("AllowAllDomains");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
