using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public static class GraphColoring
    {
        /// <summary>
        /// Ogólna metoda - kolorowanie zachłanne na podstawie ustalonego ciągu wierzchołków.
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <param name="order">Porządek wierzchołków, w jakim mają być one przetwarzane. W przypadku null użyj pierwotnego numerowania wierzchołków</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] GreedyColoring(this Graph g, int[] order = null)
        {
            var colorTable = new int[g.VerticesCount];

            for (int i = 0; i < g.VerticesCount; i++)
            {
                var v = (order == null) ? i : order[i];
                colorTable[v] = FindLowestColor(g, v, colorTable);
            }
            return colorTable;
        }
        private static int FindLowestColor(Graph g, int v, int[] colorTable)
        {
            var c = 1;
            while (true)
            {
                var isFound = true;
                foreach (var e in g.OutEdges(v))
                    if (colorTable[e.To] == c)
                    {
                        isFound = false;
                        c++;
                        break;
                    }
                if (isFound)
                    return c;
            }
        }
        /// <summary>
        /// Przybliżone kolorowanie metodą BFS
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] BFSColoring(this Graph g)
        {
            int cc, i = 0;
            int[] order = new int[g.VerticesCount];
            HashSet<int> set = new HashSet<int>();
            g.GeneralSearchAll<EdgesQueue>((v) =>
            {
                order[i++] = v;
                return true;
            }, null, null, out cc);
            var result = g.GreedyColoring(order);
            return result;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą LargestBackDegree
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] LargestBackDegree(this Graph g)
        {
            var backDegrees = new Dictionary<int, int>(g.VerticesCount);
            for (int v = 0; v < g.VerticesCount; v++)
            {
                if(!backDegrees.ContainsKey(v))
                    backDegrees.Add(v,0);
                foreach (var e in g.OutEdges(v))
                    if (e.To < v)
                    {
                        if (!backDegrees.ContainsKey(v))
                            backDegrees.Add(v, 1);
                        backDegrees[v]++;
                    }
            }
            var sortedBackDegrees = from entry in backDegrees
                                    orderby entry.Key ascending
                                    orderby entry.Value descending
                                    select entry.Key;
            var order = sortedBackDegrees.ToArray();
            var result = g.GreedyColoring(order);
            return result;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą ColorDegreeOrdering
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] ColorDegreeOrdering(this Graph g)
        {
            var neighborColorDegreeTable = new int[g.VerticesCount];
            var isUsed = new bool[g.VerticesCount];
            var order = new int[g.VerticesCount];
            var iteration = 0;
            while (iteration != g.VerticesCount)
            {
                var v = FindHighestDegree(neighborColorDegreeTable, isUsed);
                foreach (var e in g.OutEdges(v))
                    neighborColorDegreeTable[e.To]++;
                isUsed[v] = true;
                order[iteration++] = v;
            }
            var result = g.GreedyColoring(order);
            return result;
        }
        private static int FindHighestDegree(int[] neighborColorDegreeTable, bool[] isUsed)
        {
            int max = -1;
            int index = -1;
            for (int v = 0; v < neighborColorDegreeTable.Length; v++)
                if (!isUsed[v] && neighborColorDegreeTable[v] > max)
                {
                    index = v;
                    max = neighborColorDegreeTable[v];
                }
            return index;
        }
        /// <summary>
        /// Przybliżone kolorowanie metodą Incremental
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] Incremental(this Graph g)
        {
            var isColored = new bool[g.VerticesCount];
            var numOfColored = 0;
            var c = 1;
            var colorTable = new int[g.VerticesCount];
            var set = new HashSet<int>();

            while (numOfColored != g.VerticesCount)
            {
                set.Clear();
                for (int v = 0; v < g.VerticesCount; v++)
                {
                    if (isColored[v])
                        continue;
                    if (NotANeighbor(v, g, set))
                    {
                        set.Add(v);
                        numOfColored++;
                    }
                }
                foreach (var v in set)
                {
                    isColored[v] = true;
                    colorTable[v] = c;
                }
                c++;
            }
            return colorTable;
        }
        private static bool NotANeighbor(int v, Graph g, HashSet<int> set)
        {
            foreach (var e in g.OutEdges(v))
                if (set.Contains(e.To))
                    return false;
            return true;
        }
    }
}
