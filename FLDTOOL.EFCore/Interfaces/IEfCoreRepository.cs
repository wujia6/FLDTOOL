using System.Linq.Expressions;
using FLDTOOL.EFCore.Implements;
using Microsoft.EntityFrameworkCore;

namespace FLDTOOL.EFCore.Interfaces
{
    public interface IEfCoreRepository<TDbContext, TEntity>
        where TDbContext: DbContext
        where TEntity: class
    {
        /// <summary>
        /// 
        /// </summary>
        IEfCoreDbContext<TDbContext> AppDbContext { get; }
        /// <summary>
        /// 可查询实体集合
        /// </summary>
        IQueryable<TEntity> Entities { get; }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Insert(TEntity entity);
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Insert(IEnumerable<TEntity> entities);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete(TEntity entity);
        /// <summary>
        /// 删除集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Delete(IEnumerable<TEntity> entities);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        bool Update(TEntity entity, Expression<Func<TEntity, object>>[]? fields = null);
        /// <summary>
        /// 更新集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Update(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>>[]? fields = null);
    }
}
