using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Services;

namespace TeamManager.Manual.NotificationSender
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceProvider serviceProvider = BuildServiceProvider();
                IEmailSender emailSender = serviceProvider.GetRequiredService<IEmailSender>();
                emailSender.SendContactEmailAsync("hegyi.emil.peter@gmail.com", $"Job test {DateTime.UtcNow.ToString()}", "hegyi.emil.peter@gmail.com").Wait();
                Console.WriteLine("Email sent successfully.");
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
            services.AddDbContext<>();
            services.AddSingleton(configuration);
            services.AddScoped<IEmailSender, EmailSender>();

            return services.BuildServiceProvider();
        }
    }
}
