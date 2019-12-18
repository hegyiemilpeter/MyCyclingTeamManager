using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IBillManager
    {
        Task CreateBillAsync(User user, int amount, DateTime createdAt, IFormFile image);
    }
}
