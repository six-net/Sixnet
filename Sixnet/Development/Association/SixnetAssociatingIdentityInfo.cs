// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Association
{
    public interface ISixnetAssociatingIdentityInfo
    {
        string SourcePropertyName { get; set; }

        string AssociatedPropertyName { get; set; }

        string AssociatedFieldName { get; set; }

        object GetSourceDataValue(object data);

        object GetAssociatedDataValue(object data);
    }

    public class SixnetAssociatingIdentityInfo<TSource, TAssociated> : ISixnetAssociatingIdentityInfo
    {
        public string SourcePropertyName { get; set; }

        public Func<TSource, object> GetSourceDataValueFunc { get; set; }

        public string AssociatedPropertyName { get; set; }

        public Func<TAssociated, object> GetAssociatedDataValueFunc { get; set; }

        public string AssociatedFieldName { get; set; }

        internal void SetGettingSourceDataValueFunc(Expression<Func<TSource, object>> expression)
        {
            if (expression != null)
            {
                GetSourceDataValueFunc = expression.Compile();
            }
        }

        internal void SetGettingAssociatedDataValueFunc(Expression<Func<TAssociated, object>> expression)
        {
            if (expression != null)
            {
                GetAssociatedDataValueFunc = expression.Compile();
            }
        }

        public object GetSourceDataValue(object data)
        {
            if (data is TSource sourceData)
            {
                return GetSourceDataValueFunc(sourceData);
            }
            return null;
        }

        public object GetAssociatedDataValue(object data)
        {
            if (data is TAssociated associationData)
            {
                return GetAssociatedDataValueFunc(associationData);
            }
            return null;
        }
    }
}
