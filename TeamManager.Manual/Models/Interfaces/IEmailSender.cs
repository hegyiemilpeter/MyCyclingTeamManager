using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IEmailSender
    {
        Task SendForgotPasswordEmailAsync(string to, string lastName, string token, string userId, string baseUrl);
    }
}
