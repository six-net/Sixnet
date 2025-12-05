// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    internal class SixnetEmptyDatabaseUpdateRecord : SixnetBaseDatabaseUpdateRecord<SixnetEmptyDatabaseUpdateRecord>
    {
        public override Version Version { get; set; }
        public override string Note { get; set; }
        public override long Id { get; set; }

        protected override Task ExecuteRollbackAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task ExecuteUpdateAsync()
        {
            throw new NotImplementedException();
        }

        internal static SixnetEmptyDatabaseUpdateRecord Create(long id)
        {
            return new SixnetEmptyDatabaseUpdateRecord()
            {
                Id = id
            };
        }
    }
}
