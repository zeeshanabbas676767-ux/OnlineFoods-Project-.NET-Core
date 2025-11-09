using Microsoft.EntityFrameworkCore;
using NewCoreProject.Data;
using System.Collections.Generic;
using System.Linq;

namespace NewCoreProject.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _table;

        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }

        public IEnumerable<T> GetAll() => _table.ToList();

        public T GetById(int id) => _table.Find(id);

        public void Add(T entity) => _table.Add(entity);

        public void Update(T entity) => _table.Update(entity);

        public void Delete(int id)
        {
            var entity = _table.Find(id);
            if (entity != null)
                _table.Remove(entity);
        }

        public void Save() => _db.SaveChanges();
    }
}
