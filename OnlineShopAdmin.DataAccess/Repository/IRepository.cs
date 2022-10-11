﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetListAsync(string[] includeProperties = null, CancellationToken cancellationToken = default);
        Task<(IEnumerable<T> list, Pager pageDetails)> GetListAsync(int pg, int pageSize, string[] includeProperties = null, CancellationToken cancellationToken = default);
        IEnumerable<T> GetList();
        Task<T> GetByIdAsync(int id, string[] includeProperties = null, CancellationToken cancellationToken = default);
        Task InseretAsynch(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsynch(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsynch(int id, CancellationToken cancellationToken = default);
        Task DeleteAsynch(T entity, CancellationToken cancellationToken = default);
        Task SaveAsynch(CancellationToken cancellationToken = default);
        Task<bool> CustomExists(int id);
    }
}
