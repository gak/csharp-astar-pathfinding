using System;
using System.Collections.Generic;
using System.Linq;

namespace Slowchop
{
    public class AStarFinder<T> : Finder<T>
    {
        private readonly Dictionary<T, NodeCosts> _costs = new Dictionary<T, NodeCosts>();
        private readonly List<T> _closedList = new List<T>();
        private T _dst;
        private T _focusedNode;
        private Graph<T> _graph;
        private readonly List<T> _openList = new List<T>();
        private NodeCosts _parentCost;

        private States _state;

        public override IEnumerable<T> Search(Graph<T> graph, T src, T dst)
        {
            // Quick check to see if destination has neighbours
            if (!graph.Links.TryGetValue(dst, out var neighbours) || !neighbours.Any())
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

        private IEnumerable<T> SearchResults()
        {
            return _state == States.Found ? OpenListToPath() : null;
        }

        private bool SearchStep()
        {
            _openList.Remove(_focusedNode);
            _closedList.Add(_focusedNode);

            if (_focusedNode.Equals(_dst))
            {
                _state = States.Found;
                return false;
            }

            if (!_graph.Links.TryGetValue(_focusedNode, out var neighbours))
            {
                neighbours = new List<T>();
            }

            // Add costs dst each neighbour that is not closed
            CalculateNeighborCosts(neighbours);

            if (!_openList.Any())
            {
                _state = States.NoSolution;
                return false;
            }

            _focusedNode = GetBestOpenNode();
            return true;
        }

        private void CalculateNeighborCosts(IEnumerable<T> neighbours)
        {
            foreach (var node in neighbours)
            {
                if (_closedList.Contains(node))
                    continue;

                NodeCosts cost;
                var g = _parentCost.G + Callback.ApproximateDistance(_focusedNode, node) + _graph.Costs[node];

                if (_openList.Contains(node))
                {
                    cost = _costs[node];
                    if (g < cost.G)
                    {
                        cost.G = g;
                    }
                }
                else
                {
                    cost = new NodeCosts
                    {
                        G = g,
                        H = Callback.ApproximateDistance(node, _dst)
                    };
                    _costs[node] = cost;

                    _openList.Add(node);
                }

                cost.Parent = _focusedNode;
                cost.F = cost.G + cost.H;
            }
        }

        private T GetBestOpenNode()
        {
            var bestNode = _openList[0];
            var bestCost = _costs[bestNode];

            foreach (var node in _openList)
            {
                var cost = _costs[node];

                if (cost.F >= bestCost.F)
                    continue;

                bestNode = node;
                bestCost = cost;
            }

            return bestNode;
        }

        private void SearchStart(Graph<T> graph, T src, T dst)
        {
            if (Callback == null)
            {
                throw new Exception("callback not defined");
            }

            _graph = graph;
            _dst = dst;

            // Clear out vars
            _state = States.Searching;
            _openList.Clear();
            _closedList.Clear();
            _costs.Clear();

            // Add starting node to open list and add costs
            _openList.Add(src);
            _parentCost = new NodeCosts();
            _parentCost.G = 0;
            _parentCost.H = Callback.ApproximateDistance(src, dst);
            _parentCost.F = _parentCost.H;
            _costs[src] = _parentCost;
            _focusedNode = src;
        }

        private List<T> OpenListToPath()
        {
            var path = new List<T>();

            var node = _focusedNode;
            var cost = _costs[node];
            path.Add(node);

            while (cost.Parent != null)
            {
                node = cost.Parent;
                path.Add(node);
                cost = _costs[node];
            }

            path.Reverse();

            return path;
        }

        private class NodeCosts
        {
            /**
             * Sum
             */
            public float F;

            /**
             * The movement cost to move from the starting point A to a given square on the grid, following the path
             * generated to get there.
             */
            public float G;

            /**
             * The estimated movement cost to move from that given square on the grid to the final destination.
            */
            public float H;

            /**
             * Parent Node
             */
            public T Parent;
        }
    }
}