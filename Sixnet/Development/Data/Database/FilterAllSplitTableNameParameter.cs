using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Data.Database
{
    public class FilterAllSplitTableNameParameter
    {
        public List<string> AllTableNames { get; set;}

        public string RootTableName {  get; set;}

        public SplitTableBehavior Behavior { get; set;}
    }
}
