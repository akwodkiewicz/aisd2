using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using ASD;
using ASD.Graphs;

namespace lab10
{

class MatchingTestCase : TestCase
    {
        private readonly Graph g;
        private readonly int allowedCollisions;
        private readonly int expectedResult;
        private readonly double expectedWeight;
        private Nullable<AlmostMatchingSolution> solution=null;

        public MatchingTestCase(double timeLimit, Graph g, int allowedCollisions, int expectedResult, double expectedWeight) : base(timeLimit, null)
            {
            this.g = g;
            this.allowedCollisions=allowedCollisions;
            this.expectedResult = expectedResult;
            this.expectedWeight = expectedWeight;
            }

        public override void PerformTestCase()
            {
            solution = AlmostMatching.LargestS(g.Clone(), allowedCollisions);
            }

        public static bool HasEdge(Graph g, int from, int to, double weight)
            {
            return g.GetEdgeWeight(from, to) == weight;
            }


        private Tuple<Result, string> VerifyEdges(List<Edge> edges)
            {
            int[] collisions = new int[g.VerticesCount];
            if(edges == null)
                {
                return Tuple.Create(Result.BadResult, String.Format("Zwrócono edges==null"));
                }
            else
                {
                foreach(Edge e in edges)
                    {
                    collisions[e.From]++;
                    collisions[e.To]++;
                    }

                int collSum = collisions.Select(x => Math.Max(0,x-1)).Aggregate((x,y) => x+y);

                if(expectedResult != edges.Count)
                    {
                    return Tuple.Create(Result.BadResult, String.Format("Zła liczba krawędzi w liście: {0}, zadeklarowano {1}", edges.Count, expectedResult));
                    }
                else
                    if(edges.Distinct().Count() != edges.Count)
                        {
                        return Tuple.Create(Result.BadResult, "Wynik zawiera powtórzenia krawędzi!!");
                        }
                    else
                        if(edges.Any(e => !HasEdge(g, e.From, e.To, e.Weight)))
                            {
                            return Tuple.Create(Result.BadResult, "Wynik zawiera krawędzie spoza grafu");
                            }
                        else
                            if(collSum > allowedCollisions)
                                {
                                return Tuple.Create(Result.BadResult, "Zbyt dużo kolizji krawędzi w liście");
                                }
                            else
                                if(edges.Count > 0 && edges.Select(e=> e.Weight).Aggregate((a,b) => a+b) != expectedWeight)
                                    {
                                    return Tuple.Create(Result.BadResult, String.Format("Zbyt duża suma wag krawędzi, otrzymano {0}, oczekiwane {1}", edges.Select(e=> e.Weight).Aggregate((a,b) => a+b), expectedWeight));
                                    }
                                else
                                    {
                                    return Tuple.Create(Result.Success, "OK");
                                    }
                }
            }

        public override void VerifyTestCase(out Result result, out string message)
            {
            if(!solution.HasValue)
                {
                throw new InvalidOperationException("Weryfikacja testu bez wykonania -- błąd krytyczny w bibliotece testów");
                }

            AlmostMatchingSolution sol = solution.Value;

            if(sol.edgesCount != expectedResult)
                {
                message = String.Format("Zła liczność rozwiązania: {0}, oczekiwano {1}", sol.edgesCount, expectedResult);
                result = Result.BadResult;
                }
            else
                {
                var er = VerifyEdges(sol.solution);
                if(er.Item1 == Result.Success)
                    {
                    result = Result.Success;
                    message = "OK";
                    }
                else
                    {
                    result = er.Item1;
                    message = "Zadeklarowana liczność OK, "+er.Item2;
                    }
                }
            }
    }

static class MainClass
    {
        public static void UnweightedTest()
            {
            Console.Out.WriteLine("Grafy nieważone");
            Graph grid3 = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
            for(int i=0; i<9; i+=3)
                {
                grid3.AddEdge(i,i+1);
                grid3.AddEdge(i+1,i+2);
                }

            for(int i=0; i<3; i++)
                {
                grid3.AddEdge(i+0,i+3);
                grid3.AddEdge(i+3,i+6);
                }

            Graph path20 = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 20);
            for(int i=1; i < path20.VerticesCount; i++)
                {
                path20.AddEdge(i-1, i);
                }

            Graph shiftedPath20 = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 20);
            shiftedPath20.AddEdge(18,19);
            shiftedPath20.AddEdge(0,1);
            shiftedPath20.AddEdge(19,1);
            for(int i=2; i < shiftedPath20.VerticesCount; i++)
                {
                shiftedPath20.AddEdge(i-1, i);
                }

            RandomGraphGenerator rgg = new RandomGraphGenerator(240044);
            Graph eCycle24 = rgg.UndirectedCycle(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), 24, 1, 1, true);
            Graph oCycle23 = rgg.UndirectedCycle(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), 23, 1, 1, true);
            Graph iso = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 1024);

            TestSet set1 = new TestSet();

            set1.TestCases.Add(new MatchingTestCase(10, path20, 0, 10, 10));
            set1.TestCases.Add(new MatchingTestCase(10, shiftedPath20, 0, 10, 10));
            set1.TestCases.Add(new MatchingTestCase(10, eCycle24, 0, 12, 12));
            set1.TestCases.Add(new MatchingTestCase(10, oCycle23, 0, 11, 11));
            set1.TestCases.Add(new MatchingTestCase(10, grid3, 0, 4, 4));
            set1.TestCases.Add(new MatchingTestCase(10, grid3, 1, 5, 5));
            set1.TestCases.Add(new MatchingTestCase(10, grid3, 2*grid3.EdgesCount - grid3.VerticesCount, grid3.EdgesCount, grid3.EdgesCount));
            set1.TestCases.Add(new MatchingTestCase(10, path20, 0, 10, 10));
            set1.TestCases.Add(new MatchingTestCase(10, shiftedPath20, 0, 10, 10));
            set1.TestCases.Add(new MatchingTestCase(10, path20, 1, 10, 10));
            set1.TestCases.Add(new MatchingTestCase(10, shiftedPath20, 1, 10, 10));
            set1.TestCases.Add(new MatchingTestCase(10, eCycle24, 0, 12, 12));
            set1.TestCases.Add(new MatchingTestCase(10, oCycle23, 0, 11, 11));
            set1.TestCases.Add(new MatchingTestCase(10, eCycle24, 1, 12, 12));
            set1.TestCases.Add(new MatchingTestCase(10, oCycle23, 1, 12, 12));
            set1.TestCases.Add(new MatchingTestCase(10, iso, 0, 0, 0));
            set1.TestCases.Add(new MatchingTestCase(10, iso, 10000, 0, 0));

            set1.PreformTests(true, false);
            }


        public static void WeightedTest()
            {
            Console.Out.WriteLine("Grafy ważone");

            Graph grid3 = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 9);
            for(int i=0; i<9; i+=3)
                {
                grid3.AddEdge(i,i+1, 3);
                grid3.AddEdge(i+1,i+2, 2);
                }

            for(int i=0; i<3; i++)
                {
                grid3.AddEdge(i+0,i+3, 2);
                grid3.AddEdge(i+3,i+6, 1);
                }

            RandomGraphGenerator rgg = new RandomGraphGenerator(240044);
            Graph eCycle24 = rgg.UndirectedCycle(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), 24, 1, 5, true);
            Graph oCycle23 = rgg.UndirectedCycle(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), 23, 1, 5, true);
            Graph g3 = rgg.UndirectedGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), 20, 0.2, 1, 11, true);


            TestSet set2 = new TestSet();

            set2.TestCases.Add(new MatchingTestCase(10, grid3, 0, 4, 5));
            set2.TestCases.Add(new MatchingTestCase(10, grid3, 1, 5, 7));
            set2.TestCases.Add(new MatchingTestCase(10, grid3, 2*grid3.EdgesCount - grid3.VerticesCount, grid3.EdgesCount, 3+6+6+9));
            set2.TestCases.Add(new MatchingTestCase(10, grid3, 2*grid3.EdgesCount - grid3.VerticesCount - 1, grid3.EdgesCount-1, 3+6+6+6));
            set2.TestCases.Add(new MatchingTestCase(10, eCycle24, 0, 12, 33));
            set2.TestCases.Add(new MatchingTestCase(10, oCycle23, 0, 11, 30));
            set2.TestCases.Add(new MatchingTestCase(10, eCycle24, 1, 12, 24));
            set2.TestCases.Add(new MatchingTestCase(10, oCycle23, 1, 12, 32));
            set2.TestCases.Add(new MatchingTestCase(10, g3, 0, 10, 45));
            set2.TestCases.Add(new MatchingTestCase(10, g3, 2, 11, 43));
            set2.TestCases.Add(new MatchingTestCase(10, g3, 3, 11, 35));

            set2.PreformTests(true, false);
            }

        public static void Main (string[] args)
            {
            Stopwatch w = new Stopwatch();

            w.Start();
            UnweightedTest();
            WeightedTest();
            w.Stop();
            Console.WriteLine("Time elapsed: {0}ms", w.ElapsedMilliseconds);
            }
    }
}


