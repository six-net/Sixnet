using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Domain.Parameter
{
    public class SaveDataAfterContext<T>
    {
        /// <summary>
        /// Gets or sets the save parameter
        /// </summary>
        public SaveDataParameter<T> SaveParameter {  get; set; }

        /// <summary>
        /// Gets or sets the old data
        /// </summary>
        public T OldData {  get; set; }
    }
}
