using BBM.Business.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BBM.Business.Infractstructure
{
    public class CRUD
    {
        public admin_softbbmEntities DbContext = new admin_softbbmEntities();

        public virtual async Task<List<T>> GetAll<T>() where T : class
        {
            var query = await DbContext.Set<T>().ToListAsync<T>();
            return query;
        }

        public virtual async Task<T> FindBy<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var query = await DbContext.Set<T>().Where(predicate).FirstOrDefaultAsync<T>();
            return query;
        }

        public virtual async Task SaveChangesAsync(bool recreateContext = false)
        {
            await DbContext.SaveChangesAsync();
            //if (recreateContext)
            //{
            //    DbContext.Dispose();
            //    DbContext = new admin_softbbmEntities();
            //    //DbContext.Configuration.AutoDetectChangesEnabled = false;
            //}
        }

        public virtual void Update<T>(T entity, params Expression<Func<T, object>>[] updatedProperties) where T : class
        {
            var dbEntityEntry = DbContext.Entry(entity);

            DbContext.Set<T>().Attach(entity);

            if (updatedProperties.Any())
            {
                foreach (var property in updatedProperties)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                DbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
        }
        public virtual void Add<T>(T entity) where T : class
        {
            DbContext.Set<T>().Add(entity);
        }
        public virtual void SaveChanges()
        {
            DbContext.SaveChanges();
        }
    }
}