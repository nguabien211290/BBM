using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BBM.Business.Models.View;
using BBM.Business.Model.Module;

namespace BBM.Business.Repository
{
    public interface IRepository<T> : IDisposable where T : class
    {
        //Method to get all rows in a table
        IEnumerable<T> DataSet { get; }

        //Method to add row to the table
        void Add(T entity);

        //Method to fetch row from the table
        T GetById(object id);

        //Method to update a row in the table
        void Update(T entity, params Expression<Func<T, object>>[] updatedProperties);

        //Method to delete a row from the table
        void Delete(T entity);

        void Delete(object id);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        IEnumerable<T> Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "");

        List<T> SearchBy(PagingInfo pageinfo, out int count, out int min, out double totalMoney, int BranchesId = 0);

        //List<T> SearchBy1(PagingInfo pageinfo, out int count, out int min, out Dictionary<string, object> values, UserCurrent User);
    }
}
