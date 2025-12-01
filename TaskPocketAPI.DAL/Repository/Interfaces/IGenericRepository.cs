using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPocket.DAL.Models;

namespace TaskPocket.DAL.Repository.Interfaces
{
    public interface IGenericRepository<T> where T: BaseModel
    {
        T Add(T entity);
        IEnumerable<T> GetAll(bool withTracking = false);
        T? GetById(int id);
        int Update(T entity);
        int Remove(T Entity);
        bool ToggleStatus(T entity);

    }
}
