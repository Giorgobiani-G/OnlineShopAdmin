using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetListAsync(CancellationToken cancellationToken, string[] includeProperties = null);
        IEnumerable<T> GetList();
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken, string[] includeProperties = null);
        Task InseretAsynch(T entity, CancellationToken cancellationToken);
        Task UpdateAsynch(T entity, CancellationToken cancellationToken);
        Task DeleteAsynch(int id, CancellationToken cancellationToken);
        Task DeleteAsynch(T entity, CancellationToken cancellationToken);
        Task SaveAsynch(CancellationToken cancellationToken);
        Task<bool> CustomExists(int id);
    }
}
