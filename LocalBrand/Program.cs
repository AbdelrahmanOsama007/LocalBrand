using Business;
using Business.AdminAuth.Helper;
using Business.AdminAuth.Interfaces;
using Business.AdminAuth.Validator;
using Business.Cart.Interfaces;
using Business.Cart.Validator;
using Business.Categories.Interfaces;
using Business.Categories.Validator;
using Business.Colors.Interfaces;
using Business.Colors.Validator;
using Business.Email.Validator;
using Business.OrderCleanUp.Validator;
using Business.Orders.Interfaces;
using Business.Orders.Validator;
using Business.Products.Interfaces;
using Business.Products.Validator;
using Business.Sizes.Interfaces;
using Business.Sizes.Validator;
using Business.Wishlist.Interfaces;
using Business.Wishlist.Validator;
using CloudinaryDotNet;
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
using System.Threading.RateLimiting;

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

            // Add logging
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
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IColorService, ColorService>();
            builder.Services.AddScoped<ISizeService, SizeService>();
            builder.Services.AddScoped<IGenericRepository<Color>, GenericRepository<Color>>();
            builder.Services.AddScoped<IGenericRepository<Size>, GenericRepository<Size>>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<ImageService, ImageService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHostedService<OrderCleanupService>();

            builder.Services.AddSingleton<SmtpSettings>();
            builder.Services.AddSingleton<JWT>();
            builder.Services.AddSingleton<AppSettings>();
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


            //Logger
            builder.Services.AddLogging(builder =>
            {
                builder.AddConsole(); // Log to the console
            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "LocalBrand Web Api",
                    Description = "LocalBrand Project"
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
        {
        Reference = new OpenApiReference
        {
        Type = ReferenceType.SecurityScheme,
        Id = "Bearer"
        }
        },
        new string[] {}
        }
    });
            });

            #region FileServer

            var cloudinaryCredentials = builder.Configuration.GetSection("_Cloudinary");
            var account = new Account(
                cloudinaryCredentials["CloudName"],
                cloudinaryCredentials["ApiKey"],
                cloudinaryCredentials["ApiSecret"]
            );
            builder.Services.AddScoped<IFileCloudService, FileCloudService>();
            builder.Services.AddSingleton(account);
            builder.Services.AddScoped<Cloudinary>();
            #endregion
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<MyAppContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(connectionString));

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("ContactUsPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2 
                        }));

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                };
            });

            ConfigureIdentity(builder.Services);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<MyAppContext>();

                try
                {
                    logger.LogInformation("Applying migrations...");
                    dbContext.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while applying migrations.");
                    throw;
                }
            }
            ConfigureHttpRequestPipeline(app);
            app.Run();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ICartService, CartService>();

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

            app.UseHttpsRedirection();
            app.UseCors("AllowAllDomains");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
