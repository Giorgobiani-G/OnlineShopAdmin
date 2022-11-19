﻿using Microsoft.EntityFrameworkCore;
using OnlineShopAdmin.DataAccess.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public async Task<T> GetByIdAsync(int id, string[] includeProperties, CancellationToken cancellationToken = default)
        {
            if (includeProperties is not null)
            {
                Type type = typeof(T);
                string pkName = type.GetProperties().ElementAt(0).Name;

                ParameterExpression pe = Expression.Parameter(type, "x");
                MemberExpression me = Expression.Property(pe, pkName);
                ConstantExpression idPar = Expression.Constant(id);
                BinaryExpression body = Expression.Equal(me, idPar);
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, new[] { pe });

                IQueryable<T> query = _table;

                foreach (string includeProperty in includeProperties)
                    query = query.Include(includeProperty);

                return await query.FirstOrDefaultAsync(lambda, cancellationToken);
            }

            return await _table.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<(IEnumerable<T> list, Pager pageDetails)> GetListAsync(int pg = 1, int pageSize = 20, string search = null, string[] includeProperties = null, CancellationToken cancellationToken = default)
        {
            var isnullOrEmpty = !string.IsNullOrEmpty(search);

            IQueryable<T> data = _table;

            if (isnullOrEmpty)
            {
                var type = typeof(T);
        
                PropertyInfo[] properties;

                var prm = Expression.Parameter(type, "x");

                var searchValue = Expression.Constant(search);
               
                var isDecimal = decimal.TryParse(search, out var decimalValue);
                var isInt = int.TryParse(search, out _);
                var isDAteTime = DateTime.TryParse(search, out var resDateTime);

                if (isDecimal || isInt)
                {
                    properties = type.GetProperties().Where(x => x.PropertyType == typeof(decimal) /*|| x.PropertyType == typeof(byte)
                    || x.PropertyType == typeof(short) || x.PropertyType == typeof(int)*/).ToArray();
                    
                    var memberExpressions = properties.Select(prp => Expression.Property(prm, prp));

                    var searchDecimalValue = Expression.Constant(decimalValue);
                    
                    var equalsMethod = typeof(decimal).GetMethod("Equals", new[] { typeof(decimal) });
                    
                    IEnumerable<Expression> callExpressions = memberExpressions.Select(mem => Expression.Call(mem, equalsMethod!, searchDecimalValue));

                    var body = callExpressions.Aggregate((prev, current) => Expression.Or(prev, current));

                    var lambda = Expression.Lambda<Func<T, bool>>(body, prm);

                    data = _table.Where(lambda);
                }
                else if (isDAteTime)
                {
                    var searchValueDateTime = Expression.Constant(resDateTime);

                    var nullableProperties = type.GetProperties().Where(x=>x.PropertyType == typeof(DateTime?)).ToArray();

                    var nullableMembers = nullableProperties.Select(prp => Expression.Property(prm, prp));

                    var equalsMethodNull = typeof(DateTime?).GetMethod("Equals", new[] { typeof(object) });
                    
                    Expression converted= Expression.Convert(searchValueDateTime,typeof(object));

                    IEnumerable<Expression> callExpressionsNullable = nullableMembers.Select(mem => Expression.Call(mem, equalsMethodNull!, converted));

                    properties = type.GetProperties().Where(x => x.PropertyType == typeof(DateTime)).ToArray();

                    var memberExpressions = properties.Select(prp => Expression.Property(prm, prp));

                    var equalsMethod = typeof(DateTime).GetMethod("Equals", new[] { typeof(DateTime) });

                    IEnumerable<Expression> callExpressions = memberExpressions.Select(mem => Expression.Call(mem, equalsMethod!, searchValueDateTime));

                    var combined = callExpressions.Concat(callExpressionsNullable);

                    var body = combined.Aggregate((prev, current) => Expression.Or(prev, current));

                    var lambda = Expression.Lambda<Func<T, bool>>(body, prm);

                    data = _table.Where(lambda);
                }
                else
                {
                    properties = type.GetProperties().Where(x => x.PropertyType == typeof(string)).ToArray();

                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    IEnumerable<Expression> expressions = properties.Select(prp => Expression.Call(Expression.Property(prm, prp), containsMethod!, searchValue));

                    var body = expressions.Aggregate((prev, current) => Expression.Or(prev, current));

                    var lambda = Expression.Lambda<Func<T, bool>>(body, prm);

                    data = _table.Where(lambda);
                }
            }

            var items = data.Count();

            if (pg < 1)
                pg = 1;

            if (pageSize < 1)
                pageSize = 20;

            var pager = new Pager(items, pg, pageSize);

            var skip = (pg - 1) * pageSize;

            if (includeProperties is not null)
            {
                //IQueryable<T> query = _table;

                foreach (var includeProperty in includeProperties)
                {
                    data = data.Include(includeProperty);
                }

                var res = await data.Skip(skip).Take(pager.PageSize).ToListAsync(cancellationToken);

                return (list: res, pageDetails: pager);
            }
            var result = await data.Skip(skip).Take(pager.PageSize).ToListAsync(cancellationToken);

            return (list: result, pageDetails: pager);
        }


        public async Task<IEnumerable<T>> GetListAsync(string[] includeProperties, CancellationToken cancellationToken = default)
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

        public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _table.AddAsync(entity, cancellationToken);
            await SaveAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _table.Update(entity);
            await SaveAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            T typeToDeleted = _table.Find(id);
            _table.Remove(typeToDeleted);
            await SaveAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _table.Remove(entity);
            await SaveAsync(cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CustomExists(int id)
        {
            var entities = _table.AsAsyncEnumerable();
            await foreach (var entity in entities)
            {
                int pkValue = (int)entity.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(entity)).Values.ElementAt(0);

                if (pkValue == id)
                    return true;
            }
            return false;
        }

        public IEnumerable<T> GetList()
        {
            return _table.ToList();
        }
    }

    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager() { }

        public Pager(int totalItems, int page, int pageSize = 5)
        {
            int totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);
            int currentPage = page;
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
