using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;
using System.IO;
using System.Runtime.Serialization;  // dodac w projekcie referencje System.Runtime.Serialization i System.XML

namespace ASD
{
    [Serializable]
    public class ProperResult
    {
        public int[] ExpectedGraphColoring;
        public int ExpectedColorCount;
    }

    abstract class BasicGraphColoringTestCase : TestCase
    {
        protected int colorCount;
        protected int[] graphColoring;
        protected ProperResult properResult;
        protected Graph g;
        protected Graph gCopy;

        public BasicGraphColoringTestCase(double timeLimit, Graph g, ProperResult properResult) : base(timeLimit, null)
        {
            this.properResult = properResult;
            this.g = g;
        }

        public override void VerifyTestCase(out Result resultCode, out string message)
        {
            if(!g.IsEqual(gCopy))
            {
                resultCode = Result.BadResult;
                message = string.Format("Graph was changed");
                return;
            }

            if (graphColoring == null)
            {
                resultCode = Result.BadResult;
                message = string.Format("Graph is not colored at all (result is null).");
                return;
            }

            int n = g.VerticesCount;
            bool[] used = new bool[n + 1];

            for (int i = 0; i < n; ++i)
                if (graphColoring[i] == 0)
                {
                    resultCode = Result.BadResult;
                    message = string.Format("Vertex {0} is not colored (value in array is 0).", i);
                    return;
                }

            for (int i = 0; i < n; ++i)
                if (graphColoring[i] < 0 || graphColoring[i] > n)
                {
                    resultCode = Result.BadResult;
                    message = string.Format("Vertex {0} is colored with invalid color {1}.", i, graphColoring[i]);
                    return;
                }
                else
                {
                    used[graphColoring[i]] = true;
                }

            if (graphColoring.Max() != used.Count(x => x))
            {
                resultCode = Result.BadResult;
                message = string.Format("Coloring in invalid (some color(s) is not used).");
                return;
            }

            for (int i = 0; i < n; ++i)
                foreach (var e in g.OutEdges(i))
                {
                    if (graphColoring[i] == graphColoring[e.To])
                    {
                        resultCode = Result.BadResult;
                        message = string.Format("Conflict on edge ({0},{1}).", i, e.To);
                    }
                }

            if (colorCount != properResult.ExpectedColorCount)
            {
                resultCode = Result.BadResult;
                message = string.Format("incorrect color count: {0} (expected: {1})", colorCount, properResult.ExpectedColorCount);
                return;
            }

            for(int i = 0; i < properResult.ExpectedGraphColoring.GetLength(0); i++)
            {
                if(properResult.ExpectedGraphColoring[i] != graphColoring[i])
                {
                    resultCode = Result.BadResult;
                    message = string.Format("incorrect graph coloring on index {0}: {1} (expected: {2})", i, graphColoring[i], properResult.ExpectedGraphColoring[i]);
                    return;
                }
            }

            resultCode = Result.Success;
            message = "OK";
        }
    }

    class GreedyColoringGraphColoringTestCase : BasicGraphColoringTestCase
    {
        public GreedyColoringGraphColoringTestCase(double timeLimit, Graph g, ProperResult properResult) : base(timeLimit, g, properResult)
        {
        }

        public override void PerformTestCase()
        {
            gCopy = g.Clone();
            graphColoring = g.GreedyColoring();
            if (graphColoring != null)
                colorCount = graphColoring.Max();
        }
    }

    class BFSGraphColoringTestCase : BasicGraphColoringTestCase
    {
        public BFSGraphColoringTestCase(double timeLimit, Graph g, ProperResult properResult) : base(timeLimit, g, properResult)
        {
        }

        public override void PerformTestCase()
        {
            gCopy = g.Clone();
            graphColoring = g.BFSColoring();
            if (graphColoring != null)
                colorCount = graphColoring.Max();
        }
    }

    class LargestBackDegreeGraphColoringTestCase : BasicGraphColoringTestCase
    {
        public LargestBackDegreeGraphColoringTestCase(double timeLimit, Graph g, ProperResult properResult) : base(timeLimit, g, properResult)
        {
        }

        public override void PerformTestCase()
        {
            gCopy = g.Clone();
            graphColoring = g.LargestBackDegree();
            if (graphColoring != null)
                colorCount = graphColoring.Max();
        }
    }

    class ColorDegreeOrderingGraphColoringTestCase : BasicGraphColoringTestCase
    {
        public ColorDegreeOrderingGraphColoringTestCase(double timeLimit, Graph g, ProperResult properResult) : base(timeLimit, g, properResult)
        {
        }

        public override void PerformTestCase()
        {
            gCopy = g.Clone();
            graphColoring = g.ColorDegreeOrdering();
            if (graphColoring != null)
                colorCount = graphColoring.Max();
        }
    }

    class IncrementalGraphColoringTestCase : BasicGraphColoringTestCase
    {
        public IncrementalGraphColoringTestCase(double timeLimit, Graph g, ProperResult properResult) : base(timeLimit, g, properResult)
        {
        }

        public override void PerformTestCase()
        {
            gCopy = g.Clone();
            graphColoring = g.Incremental();
            if (graphColoring != null)
                colorCount = graphColoring.Max();
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            Graph g1, g2, g3, g4, g5, g6;

            // test 1 -- graf pełny
            g1 = new AdjacencyMatrixGraph(false, 20);
            for (int i = 0; i < 20; ++i)
                for (int j = i + 1; j < 20; ++j)
                    g1.AddEdge(i, j);

            //test 2 -- graf pusty
            g2 = new AdjacencyMatrixGraph(false, 20);

            // test 3
            g3 = new AdjacencyMatrixGraph(false, 8);

            for (int i = 1; i < 6; ++i)
                g3.AddEdge(i - 1, i);
            g3.AddEdge(6, 1);
            g3.AddEdge(7, 4);

            // test 4 -- K_n,n - skojarzenie doskonałe, nieparzyste i parzyste w osobnych klasach dwudzielności

            g4 = new AdjacencyMatrixGraph(false, 20);
            for (int i = 0; i < 10; ++i)
                for (int j = 0; j < 10; ++j)
                    if (i != j)
                        g4.AddEdge(2 * i, 2 * j + 1);

            // test 5 -- prismoid - przypadek dla SL

            g5 = new AdjacencyMatrixGraph(false, 8);

            g5.AddEdge(0, 1);
            g5.AddEdge(0, 2);
            g5.AddEdge(0, 4);
            g5.AddEdge(0, 6);
            g5.AddEdge(1, 2);
            g5.AddEdge(1, 3);
            g5.AddEdge(1, 7);
            g5.AddEdge(2, 3);
            g5.AddEdge(2, 4);
            g5.AddEdge(3, 5);
            g5.AddEdge(3, 7);
            g5.AddEdge(4, 5);
            g5.AddEdge(4, 6);
            g5.AddEdge(5, 6);
            g5.AddEdge(5, 7);
            g5.AddEdge(6, 7);

            //http://cs.stackexchange.com/questions/42973/counter-example-to-graph-coloring-heuristic-using-bfs
            g6 = new AdjacencyMatrixGraph(false, 6);

            g6.AddEdge(0, 1);
            g6.AddEdge(0, 2);
            g6.AddEdge(1, 3);
            g6.AddEdge(3, 4);
            g6.AddEdge(3, 5);
            g6.AddEdge(2, 4);
            g6.AddEdge(2, 5);
            g6.AddEdge(4, 5);

            RandomGraphGenerator rgg = new RandomGraphGenerator();
            rgg.SetSeed(111);
            Graph g7 = rgg.UndirectedCycle(typeof(AdjacencyMatrixGraph), 100);
            rgg.SetSeed(222);
            Graph g8 = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 100, 0.3);
            rgg.SetSeed(333);
            Graph g9 = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 100, 0.5);
            rgg.SetSeed(444);
            Graph g10 = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 100, 0.7);
            rgg.SetSeed(666);
            Graph g11 = rgg.UndirectedGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), 17000, 0.001);

            Graph[] graphs = { g1,g2,g3,g4,g5,g6,g7,g8,g9,g10,g11};

            DataContractSerializer dcs = new DataContractSerializer(typeof(ProperResult[]));
            FileStream greedy = new FileStream("greedy.dat",FileMode.Open);
            ProperResult[] greedyProperResults = (ProperResult[])dcs.ReadObject(greedy);
            greedy.Close();
            FileStream bfs = new FileStream("bfs.dat",FileMode.Open);
            ProperResult[] bfsProperResults = (ProperResult[])dcs.ReadObject(bfs);
            bfs.Close();
            FileStream back = new FileStream("back.dat",FileMode.Open);
            ProperResult[] backProperResults = (ProperResult[])dcs.ReadObject(back);
            back.Close();
            FileStream color = new FileStream("color.dat",FileMode.Open);
            ProperResult[] colorProperResults = (ProperResult[])dcs.ReadObject(color);
            color.Close();
            FileStream incremental = new FileStream("incremental.dat",FileMode.Open);
            ProperResult[] incrementalProperResults = (ProperResult[])dcs.ReadObject(incremental);
            incremental.Close();

            TestSet setSG = new TestSet();
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g1, greedyProperResults[0]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g2, greedyProperResults[1]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g3, greedyProperResults[2]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g4, greedyProperResults[3]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g5, greedyProperResults[4]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g6, greedyProperResults[5]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g7, greedyProperResults[6]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g8, greedyProperResults[7]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g9, greedyProperResults[8]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g10, greedyProperResults[9]));
            setSG.TestCases.Add(new GreedyColoringGraphColoringTestCase(5, g11, greedyProperResults[10]));
            Console.WriteLine("\nGreedy Coloring");
            setSG.PreformTests(verbose: true, checkTimeLimit: false);

            TestSet setBFS = new TestSet();
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g1, bfsProperResults[0]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g2, bfsProperResults[1]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g3, bfsProperResults[2]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g4, bfsProperResults[3]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g5, bfsProperResults[4]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g6, bfsProperResults[5]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g7, bfsProperResults[6]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g8, bfsProperResults[7]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g9, bfsProperResults[8]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g10, bfsProperResults[9]));
            setBFS.TestCases.Add(new BFSGraphColoringTestCase(5, g11, bfsProperResults[10]));
            Console.WriteLine("\nBFS Coloring");
            setBFS.PreformTests(verbose: true, checkTimeLimit: false);

            TestSet setLBD = new TestSet();
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g1, backProperResults[0]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g2, backProperResults[1]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g3, backProperResults[2]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g4, backProperResults[3]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g5, backProperResults[4]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g6, backProperResults[5]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g7, backProperResults[6]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g8, backProperResults[7]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g9, backProperResults[8]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g10, backProperResults[9]));
            setLBD.TestCases.Add(new LargestBackDegreeGraphColoringTestCase(5, g11, backProperResults[10]));
            Console.WriteLine("\nLargest Back Degree");
            setLBD.PreformTests(verbose: true, checkTimeLimit: false);

            TestSet setDS = new TestSet();
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g1, colorProperResults[0]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g2, colorProperResults[1]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g3, colorProperResults[2]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g4, colorProperResults[3]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g5, colorProperResults[4]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g6, colorProperResults[5]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g7, colorProperResults[6]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g8, colorProperResults[7]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g9, colorProperResults[8]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g10, colorProperResults[9]));
            setDS.TestCases.Add(new ColorDegreeOrderingGraphColoringTestCase(5, g11, colorProperResults[10]));
            Console.WriteLine("\nColor Degree Ordering");
            setDS.PreformTests(verbose: true, checkTimeLimit: false);

            TestSet setInc = new TestSet();
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g1, incrementalProperResults[0]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g2, incrementalProperResults[1]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g3, incrementalProperResults[2]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g4, incrementalProperResults[3]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g5, incrementalProperResults[4]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g6, incrementalProperResults[5]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g7, incrementalProperResults[6]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g8, incrementalProperResults[7]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g9, incrementalProperResults[8]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g10, incrementalProperResults[9]));
            setInc.TestCases.Add(new IncrementalGraphColoringTestCase(5, g11, incrementalProperResults[10]));
            Console.WriteLine("\nIncremental");
            setInc.PreformTests(verbose: true, checkTimeLimit: false);
        }
    }
}

