// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Provider.Memory.Abstractions
{
    internal class PostEvictionCallbackRegistration
    {
        public PostEvictionDelegate EvictionCallback { get; set; }

        public object State { get; set; }
    }
}
