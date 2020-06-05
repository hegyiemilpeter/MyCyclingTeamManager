using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Repository
{
    public class RaceRepository : IRaceRepository
    {
        public TeamManagerDbContext DbContext { get; }

        public RaceRepository(TeamManagerDbContext teamManagerDbContext)
        {
            DbContext = teamManagerDbContext;
        }

        public async Task<int> CreateAsync(Race entity)
        {
            var race = await DbContext.Races.AddAsync(entity);
            return race.Entity.Id;
        }

        public Task<Race> GetByIDAsync(int id)
        {
            return Task.FromResult(DbContext.Races.Find(id));
        }

        public Task<IEnumerable<Race>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Race entity)
        {
            throw new NotImplementedException();
        }

        private bool disposedResources = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedResources)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }

                disposedResources = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
