using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Repository
{
    public class UserRepository : IUserRepository
    {
        private TeamManagerDbContext DbContext { get; }

        public UserRepository(TeamManagerDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Task<int> CreateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await DbContext.Users.ToListAsync();
        }

        public Task UpdateAsync(User entity)
        {
            DbContext.Users.Update(entity);

            if (DbContext.Entry(entity).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
            {
                DbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }

            return Task.CompletedTask;
        }

        public async void VerifyUser(int userId)
        {
            User user = DbContext.Users.Find(userId);
            user.VerifiedByAdmin = true;

            await UpdateAsync(user);
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

        public Task<User> GetByIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
