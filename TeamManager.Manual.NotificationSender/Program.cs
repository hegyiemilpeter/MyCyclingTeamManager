using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Services;
using TeamManager.Manual.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TeamManager.Manual.NotificationSender
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceProvider serviceProvider = BuildServiceProvider();
            ILogger logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Notification sender successfully started.");
            INotificationSender notificationSender = serviceProvider.GetRequiredService<INotificationSender>();
            notificationSender.SendEntryDeadlineNotificationsAsync().Wait();
            logger.LogInformation("Notifications sent successfully.");
        }

        private static ServiceProvider BuildServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);


            IConfiguration configuration = builder.Build();
            services.AddLogging(builder => builder.AddConsole());
            services.AddDbContext<TeamManagerDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton(configuration);
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<INotificationSender, TeamManager.Manual.Core.Services.NotificationSender>();

            return services.BuildServiceProvider();
        }
    }
}
