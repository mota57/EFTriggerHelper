using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EFTriggerHelper
{
    #region create

    public interface IBeforeCreate<TEntity> 
    {
        void BeforeCreate(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IBeforeCreateAsync<TEntity>
    {
        Task BeforeCreateAsync(DbContext context,  IEnumerable<TEntity> entities);
    }



    public interface IAfterCreate<TEntity>
    {
        void AfterCreate(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IAfterCreateAsync<TEntity>
    {
        Task AfterCreateAsync(DbContext context,  IEnumerable<TEntity> entities);
    }
    #endregion






    #region update


    public interface IBeforeUpdate<TEntity>
    {
        void BeforeUpdate(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IBeforeUpdateAsync<TEntity>
    {
        Task BeforeUpdateAsync(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IAfterUpdate<TEntity>
    {
        void AfterUpdate(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IAfterUpdateAsync<TEntity>
    {
        Task AfterUpdateAsync(DbContext context,  IEnumerable<TEntity> entities);
    }

    #endregion


    #region deleted 


    public interface IBeforeDelete<TEntity>
    {
        void BeforeDelete(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IBeforeDeleteAsync<TEntity>
    {
        void BeforeDeleteAsync(DbContext context,  IEnumerable<TEntity> entities);
    }


    public interface IAfterDelete<TEntity>
    {
        void AfterDelete(DbContext context,  IEnumerable<TEntity> entities);
    }

    public interface IAfterDeleteAsync<TEntity>
    {
        Task AfterDeleteAsync(DbContext context,  IEnumerable<TEntity> entities);
    }
    



    #endregion



  

}

