using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slowchop
{
    abstract class Finder<T>
    {
        protected ICallback<T> Callback;

        public void SetCallback(ICallback<T> callback)
        {
            Callback = callback;
        }

        internal abstract List<T> Search(Graph<T> graph, T from, T to);

    }
}
