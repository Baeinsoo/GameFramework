using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface ICrudRepository<TEntity, TId> : IRepository<TEntity, TId>
    {
        //  Create & Update
        TEntity Save(TEntity entity);
        IEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities);

        //  Read
        long Count();
        bool ExistsById(TId id);
        TEntity FindById(TId id);
        IEnumerable<TEntity> FindAll();
        IEnumerable<TEntity> FindAllById(IEnumerable<TId> ids);

        //  Delete
        void Delete(TEntity entity);
        void DeleteById(TId id);
        void DeleteAll();
        void DeleteAll(IEnumerable<TEntity> entities);
        void DeleteAllById(IEnumerable<TId> ids);
    }
}
