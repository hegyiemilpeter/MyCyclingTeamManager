using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Services;
using TeamManager.Manual.Data;
using Microsoft.EntityFrameworkCore;

namespace TeamManager.Manual.NotificationSender
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceProvider serviceProvider = BuildServiceProvider();
                INotificationSender notificationSender = serviceProvider.GetRequiredService<INotificationSender>();
                notificationSender.SendEntryDeadlineNotificationsAsync().Wait();
                Console.WriteLine("Notifications sent successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static ServiceProvider BuildServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            IConfiguration configuration = builder.Build();
            services.AddLogging();
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
