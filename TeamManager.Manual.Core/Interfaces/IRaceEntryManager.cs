using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IRaceEntryManager
    {
        Task AddEntryAsync(User user, Race race);

        Task RemoveEntryAsync(User user, Race race);

        Task<IList<string>> ListEntriedUsersAsync(int id);
    }
}
