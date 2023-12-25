using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class CrudRepository<TEntity, TId> : ICrudRepository<TEntity, TId>
    {
        public ICrudDao<TEntity, TId> dao { get; set; }

        public CrudRepository(ICrudDao<TEntity, TId> dao)
        {
            this.dao = dao;
        }

        public TEntity Save(TEntity entity)
        {
            return dao.Save(entity);
        }

        public IEnumerable<TEntity> SaveAll(IEnumerable<TEntity> entities)
        {
            return dao.SaveAll(entities);
        }

        public long Count()
        {
            return dao.Count();
        }

        public bool ExistsById(TId id)
        {
            return dao.ExistsById(id);
        }

        public TEntity FindById(TId id)
        {
            return dao.FindById(id);
        }

        public IEnumerable<TEntity> FindAll()
        {
            return dao.FindAll();
        }

        public IEnumerable<TEntity> FindAllById(IEnumerable<TId> ids)
        {
            return dao.FindAllById(ids);
        }

        public void Delete(TEntity entity)
        {
            dao.Delete(entity);
        }

        public void DeleteById(TId id)
        {
            dao.DeleteById(id);
        }

        public void DeleteAll()
        {
            dao.DeleteAll();
        }

        public void DeleteAll(IEnumerable<TEntity> entities)
        {
            dao.DeleteAll(entities);
        }

        public void DeleteAllById(IEnumerable<TId> ids)
        {
            dao.DeleteAllById(ids);
        }
    }
}
