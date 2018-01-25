using BBM.Business.Model.Entity;
using BBM.Business.Models.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace BBM.Business.Repository
{
    public class Repository<T> : IRepository<T>, IDisposable where T : class
    {
        protected admin_softbbmEntities dbContext;

        //DbSet usable with any of our entity types
        protected DbSet<T> dbSet;

        //constructor taking the database context and getting the appropriately typed data set from it
        public Repository(admin_softbbmEntities context)
        {
            dbContext = context;
            dbSet = context.Set<T>();
        }
        public virtual IQueryable<T> GetAll()
        {

            IQueryable<T> query = dbContext.Set<T>();
            return query;
        }

        //Implementation of IRepository methods
        public virtual IEnumerable<T> DataSet { get { return dbSet; } }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {

            IQueryable<T> query = dbContext.Set<T>().Where(predicate);
            return query;
        }

        public virtual IEnumerable<T> Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual T GetById(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            var dbEntityEntry = dbContext.Entry(entity);

            dbContext.Set<T>().Attach(entity);

            if (updatedProperties.Any())
            {
                foreach (var property in updatedProperties)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }
        }


        public virtual void Delete(T entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }


        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual List<T> SearchBy(PagingInfo pageinfo, int BranchesId, out int count, out int min)
        {
            count = 0;
            min = 0;
            return new List<T>();
        }

        //IDisposable implementation
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
