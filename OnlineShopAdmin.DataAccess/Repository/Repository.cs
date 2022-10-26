using Microsoft.EntityFrameworkCore;
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
                ConstantExpression idpar = Expression.Constant(id);
                BinaryExpression body = Expression.Equal(me, idpar);
                Expression<Func<T, bool>> almb = Expression.Lambda<Func<T, bool>>(body, new[] { pe });

                IQueryable<T> query = _table;

                foreach (string includeProperty in includeProperties)
                    query = query.Include(includeProperty);

                return await query.FirstOrDefaultAsync(almb, cancellationToken);
            }

            return await _table.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<(IEnumerable<T> list, Pager pageDetails)> GetListAsync(int pg = 1, int pageSize = 20, string search = null, string[] includeProperties = null, CancellationToken cancellationToken = default)
        {
            bool isnullOrEmpty = !string.IsNullOrEmpty(search);

            IQueryable<T> data = _table;

            if (isnullOrEmpty)
            {
                Type type = typeof(T);

                PropertyInfo[] properties;

                ParameterExpression prm = Expression.Parameter(type, "x");

                ConstantExpression searchValue = Expression.Constant(search);

                bool isDecimal = decimal.TryParse(search, out decimal resDecimal);
                bool isInt = int.TryParse(search, out int resInt);
                bool isDAteTime = DateTime.TryParse(search, out DateTime resDAteTime);

                if (isDecimal || isInt)
                {
                    properties = isInt switch
                    {
                        false => type.GetProperties().Where(x => x.PropertyType == typeof(decimal)).ToArray(),
                        _ => type.GetProperties().Where(x => x.PropertyType == typeof(int) || x.PropertyType == typeof(byte) || x.PropertyType == typeof(short)).ToArray()
                    };

                    IEnumerable<MemberExpression> memberExpressions = properties.Select(prp => Expression.Property(prm, prp));

                    ConstantExpression searchValueDecimal = isInt switch
                    {
                        false => searchValueDecimal = Expression.Constant(resDecimal),
                        _ => searchValueDecimal = Expression.Constant(resInt)
                    };

                    List<Expression> expressions = new();

                    foreach (var item in memberExpressions)
                    {
                        var expression = Expression.Equal(item, searchValueDecimal);
                        expressions.Add(expression);
                    }

                    Expression body = expressions.Aggregate((prev, current) => Expression.Or(prev, current));

                    Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, prm);

                    data = _table.Where(lambda);
                }
                else if (isDAteTime)
                {
                    properties = type.GetProperties().Where(x => x.PropertyType == typeof(DateTime)).ToArray();

                    IEnumerable<MemberExpression> memberExpressions = properties.Select(prp => Expression.Property(prm, prp));

                    ConstantExpression searchValueDecimal = Expression.Constant(resDAteTime);

                    List<Expression> expressions = new();

                    foreach (var item in memberExpressions)
                    {
                        var expression = Expression.Equal(item, searchValueDecimal);
                        expressions.Add(expression);
                    }

                    Expression body = expressions.Aggregate((prev, current) => Expression.Or(prev, current));

                    Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, prm);

                    data = _table.Where(lambda);
                }
                else
                {
                    properties = type.GetProperties().Where(x => x.PropertyType == typeof(string)).ToArray();

                    MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    IEnumerable<Expression> expressions = properties.Select(prp => Expression.Call(Expression.Property(prm, prp), containsMethod, searchValue));

                    Expression body = expressions.Aggregate((prev, current) => Expression.Or(prev, current));

                    Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(body, prm);

                    data = _table.Where(lambda);
                }
            }

            var benefCount = data.Count()/* _table.AsQueryable().Count()*/;

            if (pg < 1)
                pg = 1;

            if (pageSize < 1)
                pageSize = 20;

            var pager = new Pager(benefCount, pg, pageSize);

            int benfSkip = (pg - 1) * pageSize;

            if (includeProperties is not null)
            {
                //IQueryable<T> query = _table;

                foreach (var includeProperty in includeProperties)
                {
                    data = data.Include(includeProperty);
                }

                var res = await data.Skip(benfSkip).Take(pager.PageSize).ToListAsync(cancellationToken);

                return (list: res, pageDetails: pager);
            }
            var result = await data.Skip(benfSkip).Take(pager.PageSize).ToListAsync(cancellationToken);

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

        public async Task InseretAsynch(T entity, CancellationToken cancellationToken = default)
        {
            await _table.AddAsync(entity, cancellationToken);
            await SaveAsynch(cancellationToken);
        }

        public async Task UpdateAsynch(T entity, CancellationToken cancellationToken = default)
        {
            _table.Update(entity);
            await SaveAsynch(cancellationToken);
        }

        public async Task DeleteAsynch(int id, CancellationToken cancellationToken = default)
        {
            T TypeTobeDeleted = _table.Find(id);
            _table.Remove(TypeTobeDeleted);
            await SaveAsynch(cancellationToken);
        }

        public async Task DeleteAsynch(T entity, CancellationToken cancellationToken = default)
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
            var entities = _table.AsAsyncEnumerable();
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
            int totalpages = (int)Math.Ceiling(totalItems / (decimal)pageSize);
            int currentpage = page;
            int startpage = currentpage - 5;
            int endpage = currentpage + 4;

            if (startpage <= 0)
            {
                endpage = endpage - (startpage - 1);
                startpage = 1;
            }

            if (endpage > totalpages)
            {
                endpage = totalpages;
                if (endpage > 10)
                {
                    startpage = endpage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentpage;
            PageSize = pageSize;
            TotalPages = totalpages;
            StartPage = startpage;
            EndPage = endpage;
        }
    }
}
