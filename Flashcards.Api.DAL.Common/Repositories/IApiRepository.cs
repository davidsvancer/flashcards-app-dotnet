using Flashcards.Api.DAL.Common.Entities.Interfaces;
using System.Linq.Expressions;

namespace Flashcards.Api.DAL.Common.Repositories
{
    public interface IApiRepository<TEntity> where TEntity : IEntity
    {
        IList<TEntity> GetAll();
        TEntity? GetById(Guid id);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        Guid Insert(TEntity entity);
        Guid? Update(TEntity entity);
        void Remove(Guid id);
        bool Exists(Guid id);
    }
}
