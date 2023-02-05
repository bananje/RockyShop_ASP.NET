using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        T Find(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null, // null если условие where не определено
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // порядок вывода перечисляемых объектов
            string includeProperties = null, // включить свойства
            bool isTracking = true // отслеживание запроса (для редактирвоания)
            ); 

        T FirstOrDefault(
            Expression<Func<T, bool>> filter = null, // null если условие where не определено
            string includeProperties = null, // включить свойства
            bool isTracking = true
            );

        void Add(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        void Save();
        
    }
}
