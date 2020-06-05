using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Repository
{
    public class UserRaceRepository : IUserRaceRepository
    {
        private TeamManagerDbContext DbContext { get; }

        public UserRaceRepository(TeamManagerDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<int> CreateAsync(UserRace entity)
        {
            var userRaceEntity = await DbContext.UserRaces.AddAsync(entity);
            return userRaceEntity.Entity.Id;
        }

        public Task<UserRace> GetByIDAsync(int id)
        {
            return Task.FromResult(DbContext.UserRaces.Find(id));
        }

        public Task<IEnumerable<UserRace>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserRace entity)
        {
            DbContext.UserRaces.Update(entity);
            return Task.CompletedTask;
        }

        public IList<UserRace> ListUserRacesByUserId(int userId)
        {
            return DbContext.UserRaces
                .Include(x => x.Race)
                .Where(x => x.UserId == userId && (x.AbsoluteResult.HasValue || x.CategoryResult.HasValue || (x.IsTakePartAsStaff.HasValue && x.IsTakePartAsStaff.Value)))
                .ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
