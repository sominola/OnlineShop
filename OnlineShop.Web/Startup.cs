using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShop.Data;
using OnlineShop.Data.Models;
using OnlineShop.Web.Extension;
using OnlineShop.Web.Services.Account;
using OnlineShop.Web.Services.File;
using OnlineShop.Web.Services.Products;

namespace OnlineShop.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            //connect config from json file
            Configuration.Bind("Project", new Config());

            services.AddDbContext<AppDbContext>(x => x.UseSqlite(Config.ConnectionString));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 5;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductsService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IImageService, ImageService>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/AccessDenied";
                options.Cookie.Name = "Authentication.Cookie";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddMvcOptions(opt =>
            {
                opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "The field is required!");
            });
            
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}