using Csv_Enumerable;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Csv_To_Database
{
    public interface IAsyncRepository<T> : IDisposable where T : EntityBase
    {
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> range);

        Task DeleteAsync(T entity);

        Task EditAsync(T entity);
    }
}
