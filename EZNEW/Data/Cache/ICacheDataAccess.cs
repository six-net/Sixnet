using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.DataAccess;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Defines data cache data access
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public interface ICacheDataAccess<T> : IDataAccess<T>
    {
    }
}
