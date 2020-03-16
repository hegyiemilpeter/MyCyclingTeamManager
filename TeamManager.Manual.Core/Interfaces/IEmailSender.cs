using System;
using System.Threading.Tasks;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IEmailSender
    {
        Task SendForgotPasswordEmailAsync(string to, string lastName, string token, string userId, string baseUrl);

        Task SendContactEmailAsync(string to, string message, string replyTo);

        Task SendAdminVerifiedEmailAsync(string to, string firstName, string loginAddress);

        Task SendBillDeletedEmailAsync(string to, string firstName, int amount, DateTime purchaseDate, string url);

        Task SendRaceEntryDeadlineIsComingAsync(string to, string name, int days, params string[] race);
    }
}
