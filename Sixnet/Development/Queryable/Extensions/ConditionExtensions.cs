using System;
using static Sixnet.Development.Data.Dapper.SqlMapper;
using System.Collections.Generic;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using System.Linq;

namespace Sixnet.Development.Queryable
{
    public static class ConditionExtensions
    {
        #region Clone

        /// <summary>
        /// Clone a new condition
        /// </summary>
        /// <param name="originalCondition">Original condition</param>
        /// <returns>Return a new condition</returns>
        public static ISixnetCondition Clone(this ISixnetCondition originalCondition)
        {
            if (originalCondition is Criterion criterion)
            {
                return criterion.Clone();
            }
            if (originalCondition is ISixnetQueryable queryable)
            {
                return queryable?.Clone();
            }
            throw new NotSupportedException($"Cloning operations are not supported for {originalCondition?.GetType()}");
        }

        #endregion

        #region Append entity

        /// <summary>
        /// Include entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="originalQueryable"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static ISixnetQueryable IncludeEntities<TEntity>(this ISixnetQueryable originalQueryable, IEnumerable<TEntity> entities) where TEntity : class, ISixnetEntity<TEntity>
        {
            return AppendEntityIdentityCore(entities, originalQueryable, false);
        }

        /// <summary>
        /// Include entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="originalQueryable"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ISixnetQueryable IncludeEntity<TEntity>(this ISixnetQueryable originalQueryable, TEntity entity) where TEntity : class, ISixnetEntity<TEntity>
        {
            return AppendEntityIdentityCore(new TEntity[1] { entity }, originalQueryable, false);
        }

        /// <summary>
        /// Exclude entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="originalQueryable"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static ISixnetQueryable ExcludeEntities<TEntity>(this ISixnetQueryable originalQueryable, IEnumerable<TEntity> entities) where TEntity : class, ISixnetEntity<TEntity>
        {
            return AppendEntityIdentityCore(entities, originalQueryable, true);
        }

        /// <summary>
        /// Exclude entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="originalQueryable"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ISixnetQueryable ExcludeEntity<TEntity>(this ISixnetQueryable originalQueryable, TEntity entity) where TEntity : class, ISixnetEntity<TEntity>
        {
            return AppendEntityIdentityCore(new TEntity[1] { entity }, originalQueryable, true);
        }

        /// <summary>
        /// Append entity identity condition to original queryable object
        /// it will create new queryable if the original is null
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="entities">Datas</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="exclude">Exclude</param>
        /// <returns></returns>
        static ISixnetQueryable AppendEntityIdentityCore<TEntity>(IEnumerable<TEntity> entities, ISixnetQueryable originalQueryable = null, bool exclude = false) where TEntity : class, ISixnetEntity<TEntity>
        {
            if (entities.IsNullOrEmpty())
            {
                return originalQueryable;
            }
            var entityType = typeof(TEntity);
            originalQueryable ??= SixnetQuerier.Create().SetModelType(entityType);

            var primaryKeys = SixnetEntityManager.GetPrimaryKeyNames(entityType);
            SixnetDirectThrower.ThrowSixnetExceptionIf(primaryKeys.IsNullOrEmpty(), string.Format("Type:{0} isn't set primary keys", entityType.FullName));

            var primaryKeyValues = new List<dynamic>();
            var primaryKeyCount = primaryKeys.GetCount();
            var isSinglePrimaryKey = primaryKeyCount == 1;
            var entityGroupQueryable = SixnetQuerier.Create();
            foreach (var entity in entities)
            {
                if (isSinglePrimaryKey)
                {
                    primaryKeyValues.Add(entity.GetValue(primaryKeys.ElementAt(0)));
                }
                else
                {
                    var entityQueryable = SixnetQuerier.Create();
                    foreach (var key in primaryKeys)
                    {
                        entityQueryable = entityQueryable.Where(Criterion.Create(exclude ? CriterionOperator.NotEqual : CriterionOperator.Equal, DataField.Create(key, entityType), ConstantField.Create(entity.GetValue(key))));
                    }
                    entityQueryable.Connector = CriterionConnector.Or;
                    entityGroupQueryable.Where(entityQueryable);
                }
            }
            if (isSinglePrimaryKey)
            {
                var primaryKeyValueCount = primaryKeyValues.Count;
                originalQueryable = exclude
                    ? primaryKeyValueCount > 1
                        ? originalQueryable.Where(Criterion.Create(CriterionOperator.NotIn, DataField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues)))
                        : originalQueryable.Where(Criterion.Create(CriterionOperator.NotEqual, DataField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues[0])))
                    : primaryKeyValueCount > 1
                        ? originalQueryable.Where(Criterion.Create(CriterionOperator.In, DataField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues)))
                        : originalQueryable.Where(Criterion.Create(CriterionOperator.Equal, DataField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues[0])));
            }
            else
            {
                originalQueryable = originalQueryable.Where(entityGroupQueryable);
            }
            return originalQueryable;
        }

        #endregion
    }
}
