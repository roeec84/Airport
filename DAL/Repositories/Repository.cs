using Common.API;
using Common.Models;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        private readonly AirportDbContext db;
        private readonly object lockObj = new object();
        private DbSet<T> entities;

        public Repository(AirportDbContext db)
        {
            this.db = db;
            entities = db.Set<T>();
        }

        public async Task<bool> Add(T entity)
        {
            bool flag;
            try
            {
                entities.Add(entity);
                await db.SaveChangesAsync();
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public IEnumerable<T> GetAll()
        {
            return entities;
        }

        public async Task<T> GetById(int id)
        {
            return await entities.FirstOrDefaultAsync(e => e.Id == id);
        }

        public bool Save()
        {
            lock(lockObj)
            {
                bool flag;
                try
                {
                    db.SaveChangesAsync();
                    flag = true;
                }
                catch (Exception ex)
                {
                    flag = false;
                }
                return flag;
            }
        }
    }
}
