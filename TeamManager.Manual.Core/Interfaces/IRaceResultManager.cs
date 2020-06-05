using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IRaceResultManager
    {
        Task AddResultAsync(int userId, int raceId, int? absoluteResult, int? categoryResult, bool? staff, MemoryStream image, string contentTypeForImage);

        IEnumerable<ResultModel> ListRaceResultsByUserId(int userId);

        Task ChangeValidatedStatus(int userRaceId);
    }
}
