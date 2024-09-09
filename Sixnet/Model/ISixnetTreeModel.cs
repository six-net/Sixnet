using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Model
{
    public interface ISixnetTreeModel<T> where T : ISixnetTreeModel<T>
    {
        public List<T> Children { get; set; }
    }
}
