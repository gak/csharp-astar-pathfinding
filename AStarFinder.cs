using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slowchop
{
    internal class AStarFinder<T> : Finder<T>
    {
        private readonly Dictionary<T, NodeCosts> _costs = new Dictionary<T, NodeCosts>();
        public List<T> ClosedList = new List<T>();
        private T _dst;
        private T _focusedNode;
        private Graph<T> _graph;
        public List<T> OpenList = new List<T>();
        private NodeCosts _parentCost;

        private States _state;

        internal override List<T> Search(Graph<T> graph, T src, T dst)
        {
            // Quick check to see if destination has neighbours
            List<T> neighbours;
            if (!graph.links.TryGetValue(dst, out neighbours) || !neighbours.Any())
            {
                _state = States.NoSolution;
                return SearchResults();
            }

            SearchStart(graph, src, dst);

            while (SearchStep())
            {
            }

            return SearchResults();
        }

        public List<T> SearchResults()
        {
            if (_state == States.Found)
            {
                return OpenListToPath();
            }

            return null;
        }

        public bool SearchStep()
        {
            OpenList.Remove(_focusedNode);
            ClosedList.Add(_focusedNode);

            if (_focusedNode.Equals(_dst))
            {
                _state = States.Found;
                return false;
            }

            List<T> neighbours;
            if (!_graph.links.TryGetValue(_focusedNode, out neighbours))
            {
                neighbours = new List<T>();
            }

            // Add costs dst each neighbour that is not closed
            CalculateNeighborCosts(neighbours);

            if (!OpenList.Any())
            {
                _state = States.NoSolution;
                return false;
            }

            _focusedNode = GetBestOpenNode();
            return true;
        }

        private void CalculateNeighborCosts(List<T> neighbours)
        {
            foreach (T node in neighbours)
            {
                if (ClosedList.Contains(node))
                    continue;

                NodeCosts cost = null;
                float g = _parentCost.g + Callback.ApproximateDistance(_focusedNode, node) + _graph.costs[node];
                
                if (OpenList.Contains(node))
                {
                    cost = _costs[node];
                    if (g < cost.g)
                    {
                        cost.g = g;
                    }
                }
                else
                {
                    cost = new NodeCosts
                    {
                        g = g,
                        h = Callback.ApproximateDistance(node, _dst)
                    };
                    _costs[node] = cost;

                    OpenList.Add(node);
                }

                cost.parent = _focusedNode;
                cost.f = cost.g + cost.h;
            }
        }

        private T GetBestOpenNode()
        {
            T bestNode = OpenList[0];
            NodeCosts bestCost = _costs[bestNode];

            foreach (T node in OpenList)
            {
                NodeCosts cost = _costs[node];

                if (cost.f >= bestCost.f)
                    continue;

                bestNode = node;
                bestCost = cost;
            }
            return bestNode;
        }

        public void SearchStart(Graph<T> graph, T src, T dst)
        {
            if (Callback == null)
            {
                throw new Exception("callback not defined");
            }

            this._graph = graph;
            this._dst = dst;

            // Clear out vars
            _state = States.Searching;
            OpenList.Clear();
            ClosedList.Clear();
            _costs.Clear();

            // Add starting node to open list and add costs
            OpenList.Add(src);
            _parentCost = new NodeCosts();
            _parentCost.g = 0;
            _parentCost.h = Callback.ApproximateDistance(src, dst);
            _parentCost.f = _parentCost.h;
            _costs[src] = _parentCost;
            _focusedNode = src;
        }

        private List<T> OpenListToPath()
        {
            var path = new List<T>();

            T node = _focusedNode;
            NodeCosts cost = _costs[node];
            path.Add(node);

            while (cost.parent != null)
            {
                node = cost.parent;
                path.Add(node);
                cost = _costs[node];
            }

            path.Reverse();

            return path;
        }


        internal class NodeCosts
        {
            /**
             * Sum
             */
            public float f;

            /**
             * The movement cost to move from the starting point A to a given square on the grid, following the path
             * generated to get there.
             */
            public float g;

            /**
             * The estimated movement cost to move from that given square on the grid to the final destination.
            */
            public float h;

            /**
             * Parent Node
             */
            public T parent;
        }
    }
}