using System.Linq.Expressions;
using FLDTOOL.EFCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FLDTOOL.EFCore.Implements
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="dbContext"></param>
    public class EfCoreRepository<TDbContext, TEntity>(IEfCoreDbContext<TDbContext> dbContext) : IEfCoreRepository<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        public IEfCoreDbContext<TDbContext> AppDbContext { get; } = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public IQueryable<TEntity> Entities { get => AppDbContext.Set<TEntity>(); }

        public bool Delete(TEntity entity)
        {
            try
            {
                AppDbContext.Set<TEntity>().AttachRange(entity);
                AppDbContext.Entry(entity).State = EntityState.Deleted;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(IEnumerable<TEntity> entities)
        {
            try
            {
                if (!entities.Any())
                    return false;
                foreach (var entity in entities)
                {
                    if (!Delete(entity))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Insert(TEntity entity)
        {
            try
            {
                AppDbContext.Set<TEntity>().AttachRange(entity);
                AppDbContext.Entry(entity).State = EntityState.Added;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Insert(IEnumerable<TEntity> entities)
        {
            try
            {
                if (!entities.Any())
                    return false;
                foreach (var entity in entities)
                {
                    if (!Insert(entity))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(TEntity entity, Expression<Func<TEntity, object>>[]? fields = null)
        {
            try
            {
                AppDbContext.Set<TEntity>().AttachRange(entity);
                if (fields != null && fields!.Length != 0)
                {
                    foreach (var expression in fields!)
                    {
                        AppDbContext.Entry(entity).Property(expression).IsModified = true;
                    }
                }
                else
                    AppDbContext.Entry(entity).State = EntityState.Modified;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>>[]? fields = null)
        {
            try
            {
                if (!entities.Any())
                    return false;
                foreach (var entity in entities)
                {
                    if (!Update(entity, fields))
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
