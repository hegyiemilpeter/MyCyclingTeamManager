using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IEmailSender
    {
        Task SendForgotPasswordEmailAsync(string to, string lastName, string token, string userId, string baseUrl);

        Task SendContactEmailAsync(string to, string message, string replyTo);

        Task SendAdminVerifiedEmailAsync(string to, string firstName, string loginAddress);

        Task SendBillDeletedEmailAsync(string to, string firstName, int amount, DateTime purchaseDate, string url);
    }
}
