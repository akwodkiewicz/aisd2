using System;
using System.Linq;
using ASD;
using ASD.Graphs;

namespace Lab9
{
    class MuseumTestCase : TestCase
    {
        public static bool NumberOnly;

        private readonly Graph g;
        private readonly int[] level;
        private readonly int[] entrances;
        private readonly int[] exits;
        private readonly int expectedResult;

        private MuseumRoutes? routes = null;


        public MuseumTestCase(double timeLimit, Graph g, int[] level, int[] entrances, int[] exits, int expectedResult) : base(timeLimit, null)
        {
            this.g = g;
            this.level = level;
            this.entrances = entrances;
            this.exits = exits;
            this.expectedResult = expectedResult;
        }


        public override void PerformTestCase()
        {
            routes = Muzeum.FindRoutes(g.Clone(), level, entrances, exits);
        }

        public override void VerifyTestCase(out Result resultCode, out string message)
        {
            if (routes.Value.liczba == expectedResult)
            {
                if (NumberOnly)
                {
                    message = "OK";
                    resultCode = Result.Success;
                    return;
                }
                if (routes.Value.trasy == null || routes.Value.trasy.Length != routes.Value.liczba)
                {
                    message = "Zły rozmiar tablicy tras lub tablica tras równa null";
                    resultCode = Result.BadResult;
                }
                else
                    if (routes.Value.trasy.Any(x => (x == null || x.Length == 0)))
                {
                    message = "Tablica tras zawiera nulle lub puste tablice";
                    resultCode = Result.BadResult;
                }
                else
                {
                    switch (CheckPaths(routes.Value.trasy))
                    {
                        case PathQualityResult.OK:
                            message = "OK";
                            resultCode = Result.Success;
                            break;
                        case PathQualityResult.BAD_END:
                            message = "Początek lub koniec trasy to nie drzwi!";
                            resultCode = Result.BadResult;
                            break;
                        case PathQualityResult.INDEX_OUT_OF_RANGE:
                            message = "Trasy zawierają wierzchołki spoza grafu";
                            resultCode = Result.BadResult;
                            break;
                        case PathQualityResult.NONEXISTENT_EDGE:
                            message = "Trasy zawierają krawędzi spoza grafu";
                            resultCode = Result.BadResult;
                            break;
                        case PathQualityResult.OVER_LEVEL:
                            message = "Wierzchołek odwiedzony częściej niż poziom ciekawości";
                            resultCode = Result.BadResult;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            else
            {
                resultCode = Result.BadResult;
                message = String.Format("Oczekiwano tras: {0}, program policzył: {1}", expectedResult, routes.Value.liczba);
            }
        }

        enum PathQualityResult
        {
            OK, INDEX_OUT_OF_RANGE, NONEXISTENT_EDGE, OVER_LEVEL, BAD_END
        }

        public static bool HasEdge(Graph g, int from, int to)
        {
            return !Double.IsNaN(g.GetEdgeWeight(from, to));
        }

        PathQualityResult CheckPaths(int[][] paths)
        {
            PathQualityResult res = PathQualityResult.OK;
            int[] realLevel = new int[g.VerticesCount];
            for (int i = 0; i < paths.Length && res == PathQualityResult.OK; i++)
            {
                if (!entrances.Contains(paths[i].First()) || !exits.Contains(paths[i].Last()))
                {
                    res = PathQualityResult.BAD_END;
                }

                for (int j = 0; j < paths[i].Length && res == PathQualityResult.OK; j++)
                {
                    if (paths[i][j] < 0 || paths[i][j] >= g.VerticesCount)
                    {
                        res = PathQualityResult.INDEX_OUT_OF_RANGE;
                        //                        Console.Out.WriteLine(paths[i][j]);
                    }
                    else
                        if (j > 0 && !HasEdge(g, paths[i][j - 1], paths[i][j]))
                    {
                        res = PathQualityResult.NONEXISTENT_EDGE;
                        //                            Console.Out.WriteLine("{0}--{1}", paths[i][j-1], paths[i][j]);
                    }
                    else
                            if (level[paths[i][j]] == realLevel[paths[i][j]])
                    {
                        res = PathQualityResult.OVER_LEVEL;
                    }
                    realLevel[paths[i][j]]++;
                }
            }

            return res;
        }


        public static MuseumTestCase RandomTest(int size, int seed, double time, int solution)
        {
            Random random = new Random(seed);
            RandomGraphGenerator rgg = new RandomGraphGenerator(seed);

            Graph g = rgg.UndirectedGraph(typeof(AdjacencyListsGraph<SimpleAdjacencyList>), size, .3, 1, 1);

            int[] cLevel = Enumerable.Range(0, size).Select(x => random.Next(20)).ToArray();
            int[] entrances = Enumerable.Range(0, random.Next(size / 2)).Select(x => random.Next(size)).ToArray();
            int[] exits = Enumerable.Range(0, random.Next(size / 2)).Select(x => random.Next(size)).ToArray();
            return new MuseumTestCase(time, g, cLevel, entrances, exits, solution);
        }
    }

    class MainClass
    {


        public static void Main(string[] args)
        {

            if (GraphLibraryVersion.ToString() != "7.0.2")
            {
                Console.WriteLine("Pobierz nową wersję biblioteki");
                return;
            }

            Graph triplePath = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 5);
            triplePath.AddEdge(0, 1);
            triplePath.AddEdge(0, 2);
            triplePath.AddEdge(0, 3);
            triplePath.AddEdge(4, 1);
            triplePath.AddEdge(4, 2);
            triplePath.AddEdge(4, 3);

            Graph grid5 = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 25);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    grid5.AddEdge(5 * i + j, 5 * i + j + 1);
                    grid5.AddEdge(5 * j + i, 5 * (j + 1) + i);
                }
            }

            Graph crossing = new AdjacencyListsGraph<SimpleAdjacencyList>(false, 5);
            crossing.AddEdge(0, 4);
            crossing.AddEdge(1, 4);
            crossing.AddEdge(2, 4);
            crossing.AddEdge(3, 4);


            MuseumTestCase triplePathCase = new MuseumTestCase(10, triplePath, new int[] { 3, 1, 1, 1, 3 }, new int[] { 0 }, new int[] { 4 }, 3);
            MuseumTestCase doublePathCase = new MuseumTestCase(10, triplePath, new int[] { 1, 1, 1, 1, 1 }, new int[] { 0, 4 }, new int[] { 0, 4 }, 2);
            MuseumTestCase singleVertexCase1 = new MuseumTestCase(10, triplePath, new int[] { 1, 1, 1, 1, 1 }, new int[] { 0 }, new int[] { 0 }, 1);
            MuseumTestCase singleVertexCase2 = new MuseumTestCase(10, triplePath, new int[] { 2, 1, 1, 1, 2 }, new int[] { 0 }, new int[] { 4 }, 2);
            MuseumTestCase doubledPathCase = new MuseumTestCase(10, triplePath, new int[] { 6, 2, 2, 2, 6 }, new int[] { 0 }, new int[] { 4 }, 6);
            MuseumTestCase almostDoubledPathCase = new MuseumTestCase(10, triplePath, new int[] { 6, 2, 2, 2, 5 }, new int[] { 0 }, new int[] { 4 }, 5);
            MuseumTestCase almostDoubledPathCase2 = new MuseumTestCase(10, triplePath, new int[] { 5, 2, 2, 2, 6 }, new int[] { 0 }, new int[] { 4 }, 5);
            MuseumTestCase middleVertexLimit = new MuseumTestCase(10, crossing, new int[] { 1, 1, 1, 1, 1 }, new int[] { 0, 1 }, new int[] { 2, 3 }, 1);

            MuseumTestCase centerToCornersGrid = new MuseumTestCase(10, grid5, new int[]
                {
                1,3,3,3,2,
                0,3,0,0,0,
                0,4,6,1,1,
                0,1,0,0,1,
                1,1,0,0,1
                }, new int[] { 12 }, new int[] { 0, 4, 20, 24 }, 5);

            MuseumTestCase cornersToCenterGrid = new MuseumTestCase(10, grid5, new int[]
                {
                1,3,3,3,2,
                0,3,0,0,0,
                0,4,6,1,1,
                0,1,0,0,1,
                1,1,0,0,1
                }, new int[] { 0, 4, 20, 24 }, new int[] { 12 }, 5);


            TestSet set = new TestSet();
            set.TestCases.Add(triplePathCase);
            set.TestCases.Add(doublePathCase);
            set.TestCases.Add(singleVertexCase1);
            set.TestCases.Add(singleVertexCase2);
            set.TestCases.Add(doubledPathCase);
            set.TestCases.Add(almostDoubledPathCase);
            set.TestCases.Add(almostDoubledPathCase2);
            set.TestCases.Add(middleVertexLimit);
            set.TestCases.Add(centerToCornersGrid);
            set.TestCases.Add(cornersToCenterGrid);
            set.TestCases.Add(MuseumTestCase.RandomTest(100, 1337, 10, 204));
            set.TestCases.Add(MuseumTestCase.RandomTest(100, 1410, 10, 181));
            set.TestCases.Add(MuseumTestCase.RandomTest(100, 240044, 10, 128));

            MuseumTestCase.NumberOnly = true;
            Console.WriteLine("\nCzesc I\n");
            set.PreformTests(verbose: true, checkTimeLimit: false);
            MuseumTestCase.NumberOnly = false;
            Console.WriteLine("\nCzesc II\n");
            set.PreformTests(verbose: true, checkTimeLimit: false);



            // Custom tests
            Console.WriteLine("\nPerforming boilerplate task");
            long boilerplateTaskTime = PerformBoilerplateTask();

            Console.WriteLine("Custom tests\n");
            TestSet customSet = new TestSet();
            customSet.TestCases.Add(MuseumTestCase.RandomTest(100, 69, 10, 116));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(150, 70, 10, 91));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(150, 71, 10, 170));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(150, 72, 10, 95));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(150, 73, 10, 99));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(150, 74, 10, 307));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(200, 75, 10, 485));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(200, 76, 10, 219));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(200, 77, 10, 0));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(200, 78, 10, 536));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(250, 79, 10, 424));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(250, 80, 10, 58));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(250, 81, 10, 387));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(250, 82, 10, 635));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(300, 83, 10, 657));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(300, 84, 10, 551));

            customSet.TestCases.Add(MuseumTestCase.RandomTest(500, 85, 10, 289));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(500, 86, 10, 969));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(500, 87, 10, 1238));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(750, 88, 10, 1722));
            customSet.TestCases.Add(MuseumTestCase.RandomTest(1000, 89, 10, 2257));
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            customSet.PreformTests(verbose: true, checkTimeLimit: false);
            stopwatch.Stop();
            long customTestsTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Custom tests: {0,5} ms          ({1:F3} times the boilerplate time)", customTestsTime, (double)customTestsTime / boilerplateTaskTime);
        }

        static long PerformBoilerplateTask()
        {
            RandomGraphGenerator rgg = new RandomGraphGenerator();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Graph boilerplateGraph = rgg.UndirectedGraph(typeof(AdjacencyMatrixGraph), 100, 0.9);
            int cc;
            for (int i = 0; i < 500; i++)
                boilerplateGraph.GeneralSearchAll<EdgesStack>(null, null, null, out cc);

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}

