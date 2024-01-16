using System;
using System.IO;
using System.Reflection;

namespace Sixnet.Localization
{
    internal class SixnetAssemblyWrapper
    {
        public SixnetAssemblyWrapper(Assembly assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public Assembly Assembly { get; }

        public virtual string FullName => Assembly.FullName!;

        public virtual Stream GetManifestResourceStream(string name) => Assembly.GetManifestResourceStream(name);
    }
}
