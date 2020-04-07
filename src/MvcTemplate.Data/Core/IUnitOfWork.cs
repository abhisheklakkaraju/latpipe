using MvcTemplate.Objects;
using System;
using System.Collections.Generic;

namespace MvcTemplate.Data.Core
{
    public interface IUnitOfWork : IDisposable
    {
        TDestination? GetAs<TModel, TDestination>(Int64? id) where TModel : BaseModel where TDestination : class;
        TModel? Get<TModel>(Int64? id) where TModel : BaseModel;
        TDestination To<TDestination>(Object source);

        IQuery<TModel> Select<TModel>() where TModel : BaseModel;

        void InsertRange<TModel>(IEnumerable<TModel> models) where TModel : BaseModel;
        void Insert<TModel>(TModel model) where TModel : BaseModel;

        void Update<TModel>(TModel model) where TModel : BaseModel;

        void DeleteRange<TModel>(IEnumerable<TModel> models) where TModel : BaseModel;
        void Delete<TModel>(TModel model) where TModel : BaseModel;
        void Delete<TModel>(Int64 id) where TModel : BaseModel;

        void Commit();
    }
}
