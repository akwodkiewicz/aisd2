using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab9
{
    public struct MuseumRoutes
    {
        public MuseumRoutes(int count, int[][] routes)
        {
            this.liczba = count;
            this.trasy = routes;
        }

        public readonly int liczba;
        public readonly int[][] trasy;
    }


    static class Muzeum
    {
        /// <summary>
        /// Znajduje najliczniejszy multizbiór tras
        /// </summary>
        /// <returns>Znaleziony multizbiór</returns>
        /// <param name="g">Graf opisujący muzeum</param>
        /// <param name="cLevel">Tablica o długości równej liczbie wierzchołków w grafie -- poziomy ciekawości wystaw</param>
        /// <param name="entrances">Wejścia</param>
        /// <param name="exits">Wyjścia</param>
        public static MuseumRoutes FindRoutes(Graph g, int[] cLevel, int[] entrances, int[] exits)
        {
            //Zamienić każdy wierzchołek o numerze k na wierzchołki k, n+k
            //przepustowość krawędzi <k, n+k>=ciekawość sali
            //przepustowość krawędzi <k, k+1>=int.MaxValue
            //podłączyć wierzchołki sal wejściowych ze źródłem 2n
            //podłączyć wierzchołki sal wyjściowych z ujściem 2n+1 
            //przejść 'result' razy po grafie flow DFS-em i dekrementowac wagi krawędzi

            int n = g.VerticesCount;
            Graph doubleGraph = g.IsolatedVerticesGraph(true, 2*n+2);
            Graph flow;


            for (int v = 0; v < n; v++)
                doubleGraph.AddEdge(v, n + v, cLevel[v]);

            for (int v = 0; v<n; v++)
                foreach (var e in g.OutEdges(v))
                    doubleGraph.AddEdge(n+v, e.To, int.MaxValue);

            foreach (var v in entrances)
                doubleGraph.AddEdge(2 * n, v, int.MaxValue);

            foreach (var v in exits)
                doubleGraph.AddEdge(n + v, 2 * n + 1, int.MaxValue);

            var result = (int)doubleGraph.FordFulkersonDinicMaxFlow(2 * n, 2 * n + 1, out flow, MaxFlowGraphExtender.OriginalDinicBlockingFlow);

            List<List<int>> list = new List<List<int>>();
            int counter = 0;
            while(counter < result)
            {
                List<int> elem = new List<int>();
                int v = 2 * n;
                Edge edge = new Edge(0,0,-1);
                bool parityflag = true;
                while(v!=2*n+1)
                {
                    parityflag = !parityflag;
                    foreach (var e in flow.OutEdges(v))
                        if (e.Weight > 0)
                        {
                            edge = e;
                            break;
                        }
                    var weight = edge.Weight;
                    flow.DelEdge(edge);
                    flow.AddEdge(edge.From, edge.To, weight - 1);
                    if(parityflag)
                        elem.Add(v);
                    v = edge.To;
                }
                list.Add(elem);
                counter++;
            }

            int[][] resultarray = new int[list.Count][];
            int i = 0;
            foreach(var elem in list)
                resultarray[i++] = elem.ToArray();


            return new MuseumRoutes(result, resultarray);
        }
    }
}

