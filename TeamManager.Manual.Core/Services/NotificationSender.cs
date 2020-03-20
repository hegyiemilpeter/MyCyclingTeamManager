using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Services
{
    public class NotificationSender : INotificationSender
    {
        private readonly TeamManagerDbContext dbContext;

        private readonly IEmailSender emailSender;

        private readonly ILogger<NotificationSender> logger;

        public NotificationSender(TeamManagerDbContext db, IEmailSender emailService, ILogger<NotificationSender> log)
        {
            dbContext = db;
            emailSender = emailService;
            logger = log;
        }

        public async Task SendEntryDeadlineNotificationsAsync()
        {
            try
            {
                logger.LogDebug("Sending entry deadline notifications...");
                int notificationBeforeEntryDeadlineInDays = 7;
                DateTime deadline = DateTime.UtcNow.AddDays(notificationBeforeEntryDeadlineInDays);
                IEnumerable<string> races = dbContext.Races.Where(x => x.EntryDeadline.HasValue && (x.EntryDeadline.Value.Date == deadline.Date)).Select(x => x.Name).AsEnumerable();
                if(races == null || races.Count() == 0)
                {
                    logger.LogInformation("No races found for sending entry deadline notification.");
                    return;
                }

                logger.LogDebug($"{races.Count()} found for sending entry deadline notification.");
                await emailSender.SendRaceEntryDeadlineIsComingAsync("bringafan-se@googlegroups.com", "Versenyzők", notificationBeforeEntryDeadlineInDays, races.ToArray());
                logger.LogInformation($"Notifications are sent about the entry deadline of the following races.", races);
                logger.LogDebug("Send entry deadline notifications finished.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to send notification about entry deadlines.");
                throw;
            }
        }
    }
}
