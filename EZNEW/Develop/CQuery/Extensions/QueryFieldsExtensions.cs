//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using EZNEW.Develop.Entity;

//namespace EZNEW.Develop.CQuery
//{
//    public static class QueryFieldsExtensions
//    {
//        /// <summary>
//        /// Get actually query fields
//        /// </summary>
//        /// <typeparam name="TEntity">Entity type</typeparam>
//        /// <param name="sourceQuery">Source query</param>
//        /// <param name="forceMustFields">Whether return the must query fields</param>
//        /// <returns>Return the newest IQuery object</returns>
//        public static IEnumerable<string> GetActuallyQueryFields<TEntity>(this IQuery sourceQuery, bool forceMustFields)
//        {
//            return GetActuallyQueryFields(sourceQuery, typeof(TEntity), forceMustFields);
//        }

//        /// <summary>
//        /// Get actually query fields
//        /// </summary>
//        /// <param name="entityType">Entity type</param>
//        /// <param name="sourceQuery">Source query</param>
//        /// <param name="forceMustFields">Whether return the must query fields</param>
//        /// <returns>Return the newest IQuery object</returns>
//        public static IEnumerable<string> GetActuallyQueryFields(this IQuery sourceQuery, Type entityType, bool forceMustFields)
//        {
//            return GetActuallyQueryFieldsWithSign(sourceQuery, entityType, forceMustFields).Item2;
//        }

//        /// <summary>
//        /// Get actually query fields
//        /// Item1: whether return entity full query fields
//        /// </summary>
//        /// <param name="sourceQuery">Source query</param>
//        /// <param name="entityType">Entity type</param>
//        /// <param name="forceMustFields">Whether return the must query fields</param>
//        /// <returns>Return actually query fields</returns>
//        internal static Tuple<bool, IEnumerable<string>> GetActuallyQueryFieldsWithSign(this IQuery sourceQuery, Type entityType, bool forceMustFields)
//        {
//            bool fullQuery = true;
//            var allowQueryFields = sourceQuery.QueryFields;
//            if (!allowQueryFields.IsNullOrEmpty())
//            {
//                fullQuery = false;
//                var mustQueryFields = EntityManager.GetMustQueryFields(entityType);
//                if (forceMustFields && !mustQueryFields.IsNullOrEmpty())
//                {
//                    allowQueryFields = mustQueryFields.Union(allowQueryFields);
//                }
//                return new Tuple<bool, IEnumerable<string>>(fullQuery, allowQueryFields);
//            }
//            IEnumerable<string> allFields = EntityManager.GetQueryFields(entityType);
//            var notQueryFields = sourceQuery.NotQueryFields;
//            if (!notQueryFields.IsNullOrEmpty())
//            {
//                fullQuery = false;
//                allFields = allFields.Except(notQueryFields);
//                var mustQueryFields = EntityManager.GetMustQueryFields(entityType);
//                if (forceMustFields && !mustQueryFields.IsNullOrEmpty())
//                {
//                    allFields = mustQueryFields.Union(allFields);
//                }
//            }
//            return new Tuple<bool, IEnumerable<string>>(fullQuery, allFields);
//        }
//    }
//}
