using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;

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
                })
                .AddEntityFrameworkStores<TeamManagerDbContext>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Developers", builder =>
                {
                    // TODO: Get Developer names from environment variables / appsettings
                    builder.RequireAuthenticatedUser();
                    builder.RequireUserName("emil.hegyi.19890802");
                });
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => $"This field is required.");

                    // Later can be refactored to use endpoint routing
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.0
                    options.EnableEndpointRouting = false;
                });

            services.AddScoped<CustomUserManager>();
            services.AddScoped<CustomSignInManager>();
            services.AddScoped<IRaceManager, RaceManager>();
            services.AddScoped<IUserRaceManager, UserRaceManager>();
            services.AddScoped<IPointManager, PointManager>();
            services.AddScoped<IPointCalculator, PointCalculator>();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(options =>
            {
                options.MapRoute("races", "{controller=Races}/{action=Index}/{year}/{month}");
                options.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
