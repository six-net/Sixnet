// "Company © 2025. All rights reserved."

namespace Sixnet.Model
{
    public interface ISixnetTreeModel<T> where T : ISixnetTreeModel<T>
    {
        public List<T> Children { get; set; }
    }
}
