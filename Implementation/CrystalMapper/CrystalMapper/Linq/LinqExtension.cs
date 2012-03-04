﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalMapper.Generic;
using CrystalMapper.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace CrystalMapper.Linq
{
    public static class LinqExtension
    {
        public static IQueryable<T> Query<T>(this DataContext dataContext) where T : Entity<T>, new()
        {
            return new Query<T>(dataContext);
        }

        public static TSource ForUpdate<TSource>(this IQueryable<TSource> source)
        {
            var forUpdate = (MethodInfo)MethodInfo.GetCurrentMethod();
            var expression = Expression.Call(source.Expression, forUpdate.MakeGenericMethod(typeof(TSource)), source.Expression);
            return source.Provider.Execute<TSource>(expression);
        }

        public static TSource ForUpdate<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var forUpdate = (MethodInfo)MethodInfo.GetCurrentMethod();
            var expression = Expression.Call(source.Expression, forUpdate.MakeGenericMethod(typeof(TSource)), source.Expression, predicate);
            return source.Provider.Execute<TSource>(expression);
        }

        public static TSource[] ForUpdateAll<TSource>(this IQueryable<TSource> source)
        {
            var forUpdate = (MethodInfo)MethodInfo.GetCurrentMethod();
            var expression = Expression.Call(source.Expression, forUpdate.MakeGenericMethod(typeof(TSource)), source.Expression);
            return source.Provider.Execute<TSource[]>(expression);
        }

        public static List<TSource> ForUpdateAll<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var forUpdate = (MethodInfo)MethodInfo.GetCurrentMethod();
            var expression = Expression.Call(source.Expression, forUpdate.MakeGenericMethod(typeof(TSource)), source.Expression, predicate);
            return source.Provider.Execute<List<TSource>>(expression);
        }
    }
}
