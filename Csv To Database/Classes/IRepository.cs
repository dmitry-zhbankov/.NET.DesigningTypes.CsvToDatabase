using Csv_Enumerable;
using System;
using System.Collections.Generic;

namespace Csv_To_Database
{
    public interface IRepository<T> : IDisposable where T : EntityBase
    {
        T GetById(int id);

        IEnumerable<T> GetAll();

        void Add(T entity);

        void AddRange(IEnumerable<T> range);

        void Delete(T entity);

        void Edit(T entity);
    }
}
