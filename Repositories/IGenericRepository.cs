using System.Collections.Generic;
using System.Linq.Expressions;

namespace NewCoreProject.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Save();
    }
}
