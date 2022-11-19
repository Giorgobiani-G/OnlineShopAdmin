using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetListAsync(string[] includeProperties = null, CancellationToken cancellationToken = default);
        Task<(IEnumerable<T> list, Pager pageDetails)> GetListAsync(int pg, int pageSize, string search=null, string[] includeProperties = null, CancellationToken cancellationToken = default);
        IEnumerable<T> GetList();
        Task<T> GetByIdAsync(int id, string[] includeProperties = null, CancellationToken cancellationToken = default);
        Task InsertAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task SaveAsync(CancellationToken cancellationToken = default);
        Task<bool> CustomExists(int id);
    }
}
