﻿using EZNEW.Development.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines model permanent record entity
    /// </summary>
    public class ModelPermanentRecordEntity<TEntity>
        : ModelRecordEntity<TEntity>, IObsoleteEntity
        where TEntity : BaseEntity<TEntity>, IModel<TEntity>, new()
    {
        /// <summary>
        /// Indecates whether is obsolete
        /// </summary>
        [EntityField(Description = "Obsolete", Role = FieldRole.ObsoleteTag)]
        public bool IsObsolete { get; set; }
    }
}