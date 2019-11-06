using FWLog.Data.Models.FilterCtx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExtensionMethods
{
    public static class LinqExtension
    {
        /// <summary>
        /// Aplica a condição de filtragem caso a condição seja verdadeira.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Fonte de dados.</param>
        /// <param name="condition">Condição que determinará se a filtragem será aplicada.</param>
        /// <param name="predicate">Expressão de filtragem.</param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Aplica a condição de filtragem caso a condição seja verdadeira.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Fonte de dados.</param>
        /// <param name="condition">Condição que determinará se a filtragem será aplicada.</param>
        /// <param name="predicate">Expressão de filtragem.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Monta a lista de dados paginada de acordo com a configuração da página.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Fonte de dados.</param>
        /// <param name="pageConfiguration">Configuração da página requerente.</param>
        /// <returns></returns>
        public static List<TSource> PaginationResult<TSource>(this IEnumerable<TSource> source, DataTableFilter pageConfiguration)
        {
            return source
                    .OrderBy(pageConfiguration.OrderByColumn, pageConfiguration.OrderByDirection)
                    .Skip(pageConfiguration.Start)
                    .Take(pageConfiguration.Length)
                    .ToList();
        }
    }
}
