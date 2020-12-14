using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.API
{
    public interface IRepository<T> where T : BaseModel
    {
        IEnumerable<T> GetAll();
        Task<T> GetById(int id);
        Task<bool> Add(T entity);
        bool Save();
    }
}
