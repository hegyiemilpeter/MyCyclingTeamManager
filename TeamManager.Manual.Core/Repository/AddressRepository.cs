using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private TeamManagerDbContext DbContext { get; }

        public AddressRepository(TeamManagerDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<int> CreateAsync(Address entity)
        {
            await DbContext.AddAsync(entity);
            return entity.Id;
        }

        #region IDisposableSupport

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

        public void Dispose()
        {
            Dispose(true);
        }

        public Task<IEnumerable<Address>> ListAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Address entity)
        {
            DbContext.Addresses.Update(entity);

            if (DbContext.Entry(entity).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
            {
                DbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }

            return Task.CompletedTask;
        }

        public async Task<Address> GetByIDAsync(int id)
        {
            return await DbContext.Addresses.FindAsync(id);
        }

        #endregion
    }
}
