using System;
using System.Collections.Generic;

namespace Slowchop
{
    public class Graph<T>
    {
        private readonly List<T> _nodes = new List<T>();
        internal readonly Dictionary<T, float> Costs = new Dictionary<T, float>();
        internal readonly Dictionary<T, List<T>> Links = new Dictionary<T, List<T>>();

        public override string ToString()
        {
            return "Graph N:" + _nodes.Count + " L:" + Links.Count;
        }

        public void AddNode(T node, float cost = 0)
        {
            if (_nodes.Contains(node))
            {
                throw new Exception("Node already exists!");
            }

            _nodes.Add(node);
            Costs[node] = cost;
        }

        public void AddLink(T from, T to)
        {
            if (!_nodes.Contains(from))
            {
                return;
            }

            if (!_nodes.Contains(to))
            {
                return;
            }

            if (!Links.TryGetValue(from, out var toList))
            {
                toList = new List<T>();
            }

            toList.Add(to);
            Links[from] = toList;
        }

        public void Clear()
        {
            Links.Clear();
            _nodes.Clear();
        }

        public Dictionary<T, List<T>> GetLinks()
        {
            return Links;
        }
    }
}