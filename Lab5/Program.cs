using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD
{
    class CyclePartitionTestCase : TestCase
    {
        Graph G;
        Edge[][] result;
        public CyclePartitionTestCase(double timeLimit,Exception expectedException,Graph G) : base(timeLimit,expectedException)
        {
            this.G = G;
        }

        public override void PerformTestCase()
        {
            result = G.Clone().cyclePartition();
        }

        public override void VerifyTestCase(out Result resultCode,out string message)
        {
            Graph GCopy = G.Clone();
            if (result == null)
            {
                resultCode = Result.BadResult;
                message = "incorrect result: null";
                return;
            }
            foreach (Edge[] cycle in result)
            {
                for (int i = 0; i < cycle.GetLength(0); i++)
                {
                    if (cycle[i].To != cycle[(i + 1) % cycle.GetLength(0)].From)
                    {
                        resultCode = Result.BadResult;
                        message = "incorrect result: edges " + cycle[i].ToString() + " and " + cycle[(i + 1) % cycle.GetLength(0)].ToString() + " consecutive on a cycle";
                        return;
                    }
                    if (G.GetEdgeWeight(cycle[i].From,cycle[i].To).IsNaN())
                    {
                        resultCode = Result.BadResult;
                        message = "incorrect result: a cycle contains nonexisting edge " + cycle[i].ToString();
                        return;
                    }
                    if (!GCopy.DelEdge(cycle[i]))
                    {
                        resultCode = Result.BadResult;
                        message = "incorrect result: edge " + cycle[i].ToString() + " contained in more than one cycle";
                        return;
                    }
                }
            }
            if (GCopy.EdgesCount > 0)
            {
                resultCode = Result.BadResult;
                message = "incorrect result: " + GCopy.EdgesCount.ToString() + "edges are not contained in any cycle";
                return;
            }
            resultCode = Result.Success;
            message = "OK";
        }
    }
    class PerfectMatchingTestCase : TestCase
    {
        Graph G;
        Graph GCopy;
        Graph result;

        public PerfectMatchingTestCase(double timeLimit,Exception expectedException,Graph G) : base(timeLimit,expectedException)
        {
            this.G = G;
            GCopy = G.Clone();
        }

        public override void PerformTestCase()
        {
            result = GCopy.perfectMatching();
        }

        public override void VerifyTestCase(out Result resultCode,out string message)
        {
            if (result == null)
            {
                resultCode = Result.BadResult;
                message = "incorrect result: null";
                return;
            }
            for (int i = 0; i < G.VerticesCount; i++)
                if (result.OutDegree(i) != 1)
                {
                    resultCode = Result.BadResult;
                    message = "incorrect result: vertex " + i.ToString() + " has degree " + result.OutDegree(i);
                    return;
                }
            for (int i = 0; i < G.VerticesCount; i++)
            {
                foreach (Edge e in G.OutEdges(i))
                    if (G.GetEdgeWeight(e.From,e.To).IsNaN())
                    {
                        resultCode = Result.BadResult;
                        message = "incorrect result: returning nonexistant edge {" + e.From.ToString() + ", " + e.To.ToString() + "}";
                        return;
                    }
            }
            resultCode = Result.Success;
            message = "OK";
        }
    }

    class Program
    {

        static Graph randomRegularBipartite(int vertices,int degree,int seed)
        {
            Random rand = new Random(seed);
            Graph ret = new AdjacencyListsGraph<HashTableAdjacencyList>(false,2 * vertices);
            for (int j = 0; j < degree; j++)
            {
                for (int i = 0; i < vertices; i++)
                {
                    int curi = i;
                    int v2 = -1;
                    while (true)
                    {
                        v2 = 2 * rand.Next(vertices) + 1;
                        //while (ret.OutDegree(v2) != j)
                        //    v2 = 2 * rand.Next(vertices) + 1;
                        while (!ret.GetEdgeWeight(2 * curi,v2).IsNaN())
                            v2 = 2 * rand.Next(vertices) + 1;
                        if (ret.OutDegree(v2) != j)
                        {
                            int tmpi = ret.OutEdges(v2).ToArray()[rand.Next(ret.OutDegree(v2))].To / 2;
                            while (ret.OutDegree(2 * tmpi) != j + 1)
                                tmpi = ret.OutEdges(v2).ToArray()[rand.Next(ret.OutDegree(v2))].To / 2;
                            ret.DelEdge(2 * tmpi,v2);
                            ret.AddEdge(2 * curi,v2);
                            curi = tmpi;
                            continue;
                        }
                        break;
                    }
                    ret.AddEdge(2 * curi,v2);
                }
            }
            return ret;
        }


        static void Main(string[] args)
        {
            TestSet cyclePartitionTests = new TestSet();
            TestSet matchingTests = new TestSet();

            Graph C4 = new AdjacencyMatrixGraph(false,4);
            C4.AddEdge(0,1);
            C4.AddEdge(0,3);
            C4.AddEdge(2,1);
            C4.AddEdge(2,3);
            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,C4));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,C4));

            Graph DoubleC4 = new AdjacencyMatrixGraph(false,8);
            DoubleC4.AddEdge(0,1);
            DoubleC4.AddEdge(0,3);
            DoubleC4.AddEdge(2,1);
            DoubleC4.AddEdge(2,3);

            DoubleC4.AddEdge(4,5);
            DoubleC4.AddEdge(4,7);
            DoubleC4.AddEdge(6,5);
            DoubleC4.AddEdge(6,7);

            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,DoubleC4));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,DoubleC4));

            Graph matching = new AdjacencyListsGraph<SimpleAdjacencyList>(false,20);
            for (int i = 0; i < 10; i++)
                matching.AddEdge((3 * i) % 20,(3 * i + 10) % 20);

            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,matching));

            Graph tripleC6 = new AdjacencyListsGraph<HashTableAdjacencyList>(false,18);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 6; j++)
                    tripleC6.AddEdge(6 * i + j,6 * i + (j + 1) % 6);

            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,tripleC6));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,tripleC6));

            Graph C4free = new AdjacencyListsGraph<HashTableAdjacencyList>(false,54);
            for (int c = 0; c < 3; c++)
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 6; j++)
                        C4free.AddEdge(18 * c + 6 * i + j,18 * c + 6 * i + (j + 1) % 6);
            for (int c = 0; c < 3; c++)
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 6; j += 2)
                        C4free.AddEdge(18 * c + 6 * i + j,18 * c + 6 * ((i + 1) % 3) + (j + 1) % 6);
            for (int c = 0; c < 3; c++)
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 6; j += 2)
                        C4free.AddEdge(18 * c + 6 * i + j,18 * ((c + 1) % 3) + 6 * ((i + 1) % 3) + (j + 1) % 6);

            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,C4free));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,C4free));

            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,randomRegularBipartite(100,10,13)));
            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,randomRegularBipartite(100,40,14)));
            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,randomRegularBipartite(200,8,15)));
            cyclePartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,randomRegularBipartite(100,16,16)));


            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,randomRegularBipartite(300,4,13)));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,randomRegularBipartite(200,8,14)));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,randomRegularBipartite(200,16,15)));
            matchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,randomRegularBipartite(100,32,16)));

            Console.WriteLine("***************************   Cycle partition tests  ***************************");
            cyclePartitionTests.PreformTests(true,false);

            Console.WriteLine("***************************  Perfect matching tests  ***************************");
            matchingTests.PreformTests(true,false);



            Console.WriteLine("**************************   Custom tests *************************");
            Console.WriteLine("Timing a boilerplate task");
            long boilerplateTaskTimeElapsed = PerformBoilerplateTask();
            Console.WriteLine("Boilerplate task done. Results will be shown below");



            int baseSeed = 15;  // will be incremented for every test
            int customTestSetSize = 15;
            TestSet customPartitionTests = new TestSet();
            TestSet customMatchingTests = new TestSet();
            Console.WriteLine("Generating test cases, this may take a while");
            for (int i = 0; i < customTestSetSize; i++)
            {
                Random r = new Random(baseSeed);
                int vertices = r.Next(100,400);
                int degree = 2 * r.Next(2,40);
                customPartitionTests.TestCases.Add(new CyclePartitionTestCase(5,null,randomRegularBipartite(vertices,degree,baseSeed++)));
            }

            for (int i = 0; i < customTestSetSize; i++)
            {
                Random r = new Random(baseSeed);
                int vertices = r.Next(100,400);
                int degree = (int)Math.Pow(2,r.Next(2,7));
                customMatchingTests.TestCases.Add(new PerfectMatchingTestCase(5,null,randomRegularBipartite(vertices,degree,baseSeed++)));
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // Preform tests... (Why do they have a typo in the test library? Even after a month)
            customPartitionTests.PreformTests(true,false);
            stopwatch.Stop();

            long partitionTestsTimeElapsed = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            customMatchingTests.PreformTests(true,false);
            stopwatch.Stop();
            long matchingTestsTimeElapsed = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("Custom tests performance metrics");
            Console.WriteLine("Boilerplate task: {0,5} ms",boilerplateTaskTimeElapsed);
            Console.WriteLine("Partition tests: {0,5} ms        ({1:F3} times the boilerplate time)",partitionTestsTimeElapsed,(double)partitionTestsTimeElapsed / boilerplateTaskTimeElapsed);
            Console.WriteLine("Matching tests:  {0,5} ms        ({1:F3} times the boilerplate time)",matchingTestsTimeElapsed,(double)matchingTestsTimeElapsed / boilerplateTaskTimeElapsed);
        }

        static long PerformBoilerplateTask()
        {
            RandomGraphGenerator rgg = new RandomGraphGenerator();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Graph boilerplateGraph = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph),100,0.9);
            int cc;
            for (int i = 0; i < 500; i++)
                boilerplateGraph.GeneralSearchAll<EdgesStack>(null,null,null,out cc);

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
