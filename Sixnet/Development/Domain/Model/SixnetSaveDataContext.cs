using Sixnet.Development.Entity;

namespace Sixnet.Development.Domain.Model
{
    public class SixnetSaveDataContext<T> where T : SixnetBaseEntity<T>, new()
    {
        internal Action<SixnetUpdateEntityOptions> UpdateConfigure;

        /// <summary>
        /// Gets or sets the save info
        /// </summary>
        public SixnetSaveDataInfo<T> SaveInfo { get; set; }

        /// <summary>
        /// Gets or sets the new data
        /// </summary>
        public T NewData { get; set; }

        /// <summary>
        /// Gets or sets the old data
        /// </summary>
        public T OldData { get; internal set; }

        /// <summary>
        /// Whether is update
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// Whether allow updating archived data
        /// </summary>
        public bool IncludeArchived { get; set; }

        /// <summary>
        /// Configure update
        /// </summary>
        /// <param name="configure"></param>
        public void ConfigureUpdate(Action<SixnetUpdateEntityOptions> configure)
        {
            if (configure != null)
            {
                if (UpdateConfigure == null)
                {
                    UpdateConfigure = configure;
                }
                else
                {
                    UpdateConfigure += configure;
                }
            }
        }
    }
}
