using System;
using System.Collections.Generic;
using System.Linq;
using Slowchop;

namespace Example
{
    class MapExample : ICallback<Pos>
    {
        private List<Pos> _positions;
        private Graph<Pos> _graph;
        private Random _rng;
        private const int Bounds = 20;

        public void Run()
        {
            _graph = new Graph<Pos>();
            _graph.Clear();
            _positions = new List<Pos>();
            _rng = new Random();

            CreateNodes();
            CreateLinks();

            Console.WriteLine(_graph);

            var finder = new AStarFinder<Pos>();

            finder.SetCallback(this);
            var path = finder.Search(_graph, new Pos(0, Bounds / 2), new Pos(Bounds - 1, Bounds / 2));
            if (path == null)
            {
                Console.WriteLine("No solution! Try again...");
                return;
            }

            DrawMap(path.ToList());
        }

        private void DrawMap(List<Pos> path)
        {
            for (var x = 0; x < Bounds; x++)
            {
                for (var y = 0; y < Bounds; y++)
                {
                    var pos = new Pos(x, y);
                    if (!_positions.Contains(pos))
                    {
                        Console.Write("O");
                        continue;
                    }

                    if (path.Contains(pos))
                    {
                        Console.Write(".");
                        continue;
                    }

                    Console.Write(" ");
                }

                Console.WriteLine("");
            }
        }

        private void CreateLinks()
        {
            foreach (var position in _positions)
            {
                foreach (var neighbour in position.Neighbours(Bounds))
                {
                    _graph.AddLink(position, neighbour);
                }
            }
        }

        private void CreateNodes()
        {
            for (var x = 0; x < Bounds; x++)
            {
                for (var y = 0; y < Bounds; y++)
                {
                    var position = new Pos(x, y);

                    if (_rng.NextDouble() < 0.2)
                    {
                        continue;
                    }

                    _positions.Add(position);
                    _graph.AddNode(position);
                }
            }
        }

        public float ApproximateDistance(Pos src, Pos dst)
        {
            return src.ApproxDistance(dst);
        }
    }
}