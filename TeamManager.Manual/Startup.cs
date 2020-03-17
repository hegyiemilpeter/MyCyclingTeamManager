using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Services;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Web;

namespace TeamManager.Manual
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TeamManagerDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services
                .AddIdentity<User, IdentityRole<int>>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<TeamManagerDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Developers", builder =>
                {
                    // TODO: Get Developer names from environment variables / appsettings
                    builder.RequireAuthenticatedUser();
                    builder.RequireUserName("emilpeter.hegyi.19890802");
                });
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(24);
                o.Name = "TeamManager";
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSession();

            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => $"This field is required.");

                    // Later can be refactored to use endpoint routing
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.0
                    options.EnableEndpointRouting = false;
                })
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.SubFolder)
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResources));
                        });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[]
                {
                new CultureInfo("en"),
                new CultureInfo("hu")
            };
                options.DefaultRequestCulture = new RequestCulture("hu");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            services.AddScoped<CustomUserManager>();
            services.AddScoped<CustomSignInManager>();
            services.AddScoped<IRaceManager, RaceManager>();
            services.AddScoped<IUserRaceManager, UserRaceManager>();
            services.AddScoped<IPointManager, PointManager>();
            services.AddScoped<IPointCalculator, PointCalculator>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IImageStore, AzureImageStore>();
            services.AddScoped<IBillManager, BillManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();
            app.UseStaticFiles();
            app.UseStatusCodePages();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseMvc(options =>
            {
                options.MapRoute("races", "{controller=Races}/{action=Index}/{year}/{month}");
                options.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
