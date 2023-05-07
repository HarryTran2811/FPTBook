using FPTBook.Data;
using FPTBook.Data.Cart;
using FPTBook.Data.Services;
using FPTBook.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [Obsolete]
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // Add framework services.
            services.AddMvc();
            services.AddPaging(
                options =>
                {
                    options.ViewName = "Bootstrap4";
                    options.PageParameterName = "page";
                }
                );
            //Connect SQL server
            services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(Configuration.GetConnectionString("DevConnection")));

            // Configure Identity for app
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddMemoryCache();
            services.AddSession();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            // Connect IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Password configuration
                options.Password.RequireDigit = false; // No require number for password
                options.Password.RequireLowercase = false; // No require lowercase char for password
                options.Password.RequireNonAlphanumeric = false; // No require special char for password
                options.Password.RequireUppercase = false; // No require uppercase char for password
                options.Password.RequiredLength = 8; // Minimum number of char for password
                options.Password.RequiredUniqueChars = 5; // Number of distinct char

                // Lockout Configuration - Lockout user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Log the account 5 minutes
                options.Lockout.MaxFailedAccessAttempts = 5; // Failed 5 times will be log the account
                options.Lockout.AllowedForNewUsers = true;

                // User configuration
                options.User.AllowedUserNameCharacters = // Character allowance for User Name
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true; // Email is unique

                // Login configuration
                options.SignIn.RequireConfirmedEmail = true; // Validate phone number (email is existed)
                options.SignIn.RequireConfirmedPhoneNumber = false; // Validate phone number

            });

            // Cookie configuration
            services.ConfigureApplicationCookie(options => {
                // options.Cookie.HttpOnly = true;  
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = $"/login/";                                 // Url to login page
                options.LogoutPath = $"/logout/";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";   // Url for access denied page
            });
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // After 5 seconds will update User (Role)
                // SecurityStamp in User đổi -> update Security
                options.ValidationInterval = TimeSpan.FromSeconds(5);
            });
            //Add VM
            services.AddScoped<IBooksService, BooksService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddControllersWithViews();
            //Configure session for cart and identity
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //Using session for cart and order
            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();   //Recover login information (authenticate)
            app.UseAuthorization();   // Recover login information  User roles (authorize)


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Books}/{action=Index}/{id?}");
            });

            //Seed Database
            AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();
        }
    }
}
