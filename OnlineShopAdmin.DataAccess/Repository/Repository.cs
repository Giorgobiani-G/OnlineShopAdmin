using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AdventureWorksLT2019Context _context;
        private readonly DbSet<T> _table;

        public Repository(AdventureWorksLT2019Context context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken,string[] includeProperties)
        {
            if (includeProperties is not null)
            {
                Type type = typeof(T);
                string pkName = type.GetProperties().ElementAt(0).Name;

                ParameterExpression pe = Expression.Parameter(type, "x");
                MemberExpression me = Expression.Property(pe, pkName);
                ConstantExpression idpar = Expression.Constant(id);
                BinaryExpression body = Expression.Equal(me, idpar);
                Expression<Func<T, bool>> almb = Expression.Lambda<Func<T, bool>>(body, new [] { pe });

                IQueryable<T> query = _table;

                foreach (string includeProperty in includeProperties)
                  query =  query.Include(includeProperty);

                return await query.FirstOrDefaultAsync(almb,cancellationToken);
            }

            return await _table.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetListAsync(CancellationToken cancellationToken, string[] includeProperties)
        {
            if (includeProperties is not null)
            {
                IQueryable<T> query = _table;
            
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
                return await query.ToListAsync(cancellationToken);
            }
            return await _table.ToListAsync(cancellationToken);
        }

        public async Task InseretAsynch(T entity, CancellationToken cancellationToken)
        {
            await _table.AddAsync(entity, cancellationToken);
            await SaveAsynch(cancellationToken);
        }

        public async Task UpdateAsynch(T entity, CancellationToken cancellationToken)
        {
                _table.Update(entity);
                await SaveAsynch(cancellationToken);
        }

        public async Task DeleteAsynch(int id, CancellationToken cancellationToken)
        {
            T TypeTobeDeleted = _table.Find(id);
            _table.Remove(TypeTobeDeleted);
            await SaveAsynch(cancellationToken);
        }

        public async Task DeleteAsynch(T entity, CancellationToken cancellationToken)
        {
            _table.Remove(entity);
            await SaveAsynch(cancellationToken);
        }

        public async Task SaveAsynch(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CustomExists(int id)
        {
            var entities =  _table.AsAsyncEnumerable();
            await foreach (var etntity in entities)
            {
                int pkvalue = (int)etntity.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(etntity)).Values.ElementAt(0);

                if (pkvalue == id)
                    return true;
            }
            return false;
        }

        public IEnumerable<T> GetList()
        {
            return _table.ToList();
        }
    }
}
