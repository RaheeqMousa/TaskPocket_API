using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPocket.DAL.Models;
using TaskPocket.DAL.Repository.Interfaces;

namespace TaskPocket.DAL.Repository.Classes
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        public T Add(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll(bool withTracking = false)
        {
            throw new NotImplementedException();
        }

        public T? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int Remove(T Entity)
        {
            throw new NotImplementedException();
        }

        public bool ToggleStatus(T entity)
        {
            throw new NotImplementedException();
        }

        public int Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
