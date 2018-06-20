using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slowchop
{
    public class Graph<T>
    {
        List<T> nodes = new List<T>();
        internal Dictionary<T, float> costs = new Dictionary<T, float>();
        internal Dictionary<T, List<T>> links = new Dictionary<T, List<T>>();

        public override string ToString()
        {
            return "Graph N:" + nodes.Count + " L:" + links.Count;
        }

        public void AddNode(T node, float cost=0)
        {
            if (nodes.Contains(node))
            {
                throw new Exception("Node already exists!");
            }
            nodes.Add(node);
            costs[node] = cost;
        }

        public void AddLink(T from, T to)
        {
            if (!nodes.Contains(from))
            {
                return;
            }
            
            if (!nodes.Contains(to))
            {
                return;
            }

            List<T> toList;
            if (!links.TryGetValue(from, out toList))
            {
                toList = new List<T>();
            }

            toList.Add(to);
            links[from] = toList;
        }

        public void Clear()
        {
            links.Clear();
            nodes.Clear();
        }

        public Dictionary<T, List<T>> GetLinks()
        {
            return links;
        }
    }
}