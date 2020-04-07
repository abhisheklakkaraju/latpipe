using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MvcTemplate.Data.Logging;
using MvcTemplate.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTemplate.Data.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext Context { get; }
        private IAuditLogger? Logger { get; }

        public UnitOfWork(DbContext context, IAuditLogger? logger = null)
        {
            Context = context;
            Logger = logger;
        }

        public TDestination? GetAs<TModel, TDestination>(Int64? id) where TModel : BaseModel where TDestination : class
        {
            return id == null
                ? default
                : Context.Set<TModel>().Where(model => model.Id == id).ProjectTo<TDestination>().FirstOrDefault();
        }
        public TModel? Get<TModel>(Int64? id) where TModel : BaseModel
        {
            return id == null ? null : Context.Find<TModel>(id);
        }
        public TDestination To<TDestination>(Object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public IQuery<TModel> Select<TModel>() where TModel : BaseModel
        {
            return new Query<TModel>(Context.Set<TModel>());
        }

        public void InsertRange<TModel>(IEnumerable<TModel> models) where TModel : BaseModel
        {
            foreach (TModel model in models)
                model.Id = 0;

            Context.AddRange(models);
        }
        public void Insert<TModel>(TModel model) where TModel : BaseModel
        {
            model.Id = 0;

            Context.Add(model);
        }
        public void Update<TModel>(TModel model) where TModel : BaseModel
        {
            EntityEntry<TModel> entry = Context.Entry(model);
            if (entry.State != EntityState.Modified && entry.State != EntityState.Unchanged)
                entry.State = EntityState.Modified;

            entry.Property(property => property.CreationDate).IsModified = false;
        }

        public void DeleteRange<TModel>(IEnumerable<TModel> models) where TModel : BaseModel
        {
            Context.RemoveRange(models);
        }
        public void Delete<TModel>(TModel model) where TModel : BaseModel
        {
            Context.Remove(model);
        }
        public void Delete<TModel>(Int64 id) where TModel : BaseModel
        {
            Delete(Context.Find<TModel>(id));
        }

        public void Commit()
        {
            Logger?.Log(Context.ChangeTracker.Entries<BaseModel>());

            Context.SaveChanges();

            Logger?.Save();
        }

        public void Dispose()
        {
            Logger?.Dispose();
            Context.Dispose();
        }
    }
}
