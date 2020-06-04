using System;
using TeamManager.Manual.Core.Interfaces.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Repository
{
    public class UnitOfWork : IDisposable
    {
        private TeamManagerDbContext DbContext { get; }

        public IUserRepository UserRepository { get; }

        public IAddressRepository AddressRepository { get; }

        public UnitOfWork(TeamManagerDbContext dbContext)
        {
            DbContext = dbContext;
            AddressRepository = new AddressRepository(dbContext);
            UserRepository = new UserRepository(dbContext);
        }

        public virtual void Save()
        {
            DbContext.SaveChanges();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    AddressRepository.Dispose();
                    UserRepository.Dispose();
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
