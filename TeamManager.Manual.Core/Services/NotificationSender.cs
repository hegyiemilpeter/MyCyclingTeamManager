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
                int notificationBeforeEntryDeadlineInDays = 7;
                DateTime deadline = DateTime.UtcNow.AddDays(notificationBeforeEntryDeadlineInDays);
                IEnumerable<Race> races = dbContext.Races.Where(x => x.EntryDeadline.HasValue && (x.EntryDeadline.Value.Date == deadline.Date)).AsEnumerable();
                if(races.Count() > 0)
                {
                    await emailSender.SendRaceEntryDeadlineIsComingAsync("bringafan-se@googlegroups.com", "Versenyzők", notificationBeforeEntryDeadlineInDays, races.Select(x => x.Name).ToArray());
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to send notification about entry deadlines.");
                throw;
            }
        }
    }
}
