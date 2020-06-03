using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamManager.Manual.Core.Interfaces.Repository
{
    public interface IBaseRepository<TEntity> : IDisposable
        where TEntity : class
    {
        Task<int> CreateAsync(TEntity entity);

        Task<IEnumerable<TEntity>> ListAsync();

        Task UpdateAsync(TEntity entity);

        Task<TEntity> GetByIDAsync(int id);
    }
}
