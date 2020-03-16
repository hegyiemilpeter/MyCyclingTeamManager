using System.Threading.Tasks;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface INotificationSender
    {
        Task SendEntryDeadlineNotificationsAsync();
    }
}
