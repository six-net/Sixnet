// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Model;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Sixnet temp table info
    /// </summary>
    public class SixnetTempTable : ISixnetCloneable<SixnetTempTable>
    {
        public string Name { get; set; }

        public SixnetTempTable Clone()
        {
            return new SixnetTempTable()
            {
                Name = Name
            };
        }
    }
}
