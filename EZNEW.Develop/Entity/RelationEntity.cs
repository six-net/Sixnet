using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// 关联实体
    /// </summary>
    public class RelationEntity
    {
        #region Propertys

        /// <summary>
        /// relation entity type
        /// </summary>
        public string RelationEntityTypeFullName
        {
            get;set;
        }

        /// <summary>
        /// relation key
        /// </summary>
        public string RelationKey
        {
            get;set;
        }

        #endregion
    }
}
