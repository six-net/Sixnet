﻿using System;
using System.Data;

namespace Sixnet.Development.Data.Dapper
{
    public static partial class SqlMapper
    {
        private struct DeserializerState
        {
            public readonly int Hash;
            public readonly Func<IDataReader, object> Func;

            public DeserializerState(int hash, Func<IDataReader, object> func)
            {
                Hash = hash;
                Func = func;
            }
        }
    }
}
