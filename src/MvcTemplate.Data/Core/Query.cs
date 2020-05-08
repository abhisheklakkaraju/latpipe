using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MvcTemplate.Data
{
    public class Query<TModel> : IQuery<TModel> where TModel : class
    {
        public Type ElementType => Set.ElementType;
        public Expression Expression => Set.Expression;
        public IQueryProvider Provider => Set.Provider;

        private IQueryable<TModel> Set { get; }

        public Query(IQueryable<TModel> set)
        {
            Set = set;
        }

        public IQuery<TResult> Select<TResult>(Expression<Func<TModel, TResult>> selector) where TResult : class
        {
            return new Query<TResult>(Set.Select(selector));
        }
        public IQuery<TModel> Where(Expression<Func<TModel, Boolean>> predicate)
        {
            return new Query<TModel>(Set.Where(predicate));
        }

        public IQueryable<TView> To<TView>()
        {
            return Set.AsNoTracking().ProjectTo<TView>();
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return Set.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
