using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Core.Models;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IBillManager
    {
        Task CreateBillAsync(User user, int amount, DateTime createdAt, IFormFile image);

        Task<BillModel> ListBillsByUserAsync(int userId);

        Task<IList<Bill>> ListBillsAsync();

        Task<Bill> GetBillByIdAsync(int billId);

        Task DeleteBillAsync(int billId);
    }
}
