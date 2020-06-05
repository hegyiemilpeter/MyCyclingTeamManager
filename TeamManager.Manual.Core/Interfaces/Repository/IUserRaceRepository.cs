using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces.Repository
{
    public interface IUserRaceRepository : IBaseRepository<UserRace>
    {
        IList<UserRace> ListUserRacesByUserId(int userId);
    }
}
