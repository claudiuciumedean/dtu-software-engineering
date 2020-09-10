using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.Repository
{

    //We are going to keep all the database logic inside the repository classes.Controllers will be responsible for handling requests, 
    //    model validation and returning responses to the frontend part of the application.

    //We should inject this interface into our API Controller and API will be communicating with the data context using this interface.
    public interface IDataRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();
        //  TEntity Get(long id);
        TEntity Get(Guid id);

        TEntity Get(Guid id1, Guid id2);

        TEntity GetByName(string name);

        TEntity GetLastItemId();

        void Add(TEntity entity);
        void Update(TEntity dbEntity, TEntity entity);
        void Delete(TEntity entity);
    }
}
