using System.Collections.Generic;

namespace Slowchop
{
    public abstract class Finder<T>
    {
        protected ICallback<T> Callback;

        public void SetCallback(ICallback<T> callback)
        {
            Callback = callback;
        }

        public abstract IEnumerable<T> Search(Graph<T> graph, T from, T to);
    }
}