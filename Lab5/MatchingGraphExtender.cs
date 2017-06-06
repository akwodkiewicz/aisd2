using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD
{
    public static class MatchingGraphExtender
    {
        /// <summary>
        /// Podział grafu na cykle. Zakładamy, że dostajemy graf nieskierowany i wszystkie wierzchołki grafu mają parzyste stopnie
        /// (nie trzeba sprawdzać poprawności danych).
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Tablica cykli; krawędzie każdego cyklu powinny być uporządkowane zgodnie z kolejnością na cyklu, zaczynając od dowolnej</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m)
        /// </remarks>
        public static Edge[][] cyclePartition(this Graph G)
        {
            Graph gr = G.Clone();
            Edge[][] eTable = new Edge[G.EdgesCount][];
            EdgesStack eStack = new EdgesStack();
            bool[] visited = new bool[gr.VerticesCount];
            int numOfCycles = 0;
            DeepSearch(ref gr,ref eStack,ref visited,ref eTable,ref numOfCycles);

            int notNull = 0;
            foreach (var x in eTable)
            {
                if (x == null)
                    break;
                else
                    notNull++;
            }
            Edge[][] resultTable = new Edge[notNull][];
            int i = 0;
            while (i < notNull)
            {
                resultTable[i] = eTable[i];
                i++;
            }
            return resultTable;
        }
        public static void DeepSearch(ref Graph G,ref EdgesStack eStack,ref bool[] visited,ref Edge[][] eTable,ref int numOfCycles)
        {
            Stack<int> vStack;
            int lastv = -1;
            for (int s = 0; s < G.VerticesCount; s++)
            {
                vStack = new Stack<int>();
                while (G.OutDegree(s) > 0)
                {
                    if (vStack.Count == 0)
                        vStack.Push(s);
                    int v = vStack.Peek();
                    if (G.OutDegree(v) == 0)
                        break;
                    if (visited[v])
                    {
                        int i = 0; Edge temp; EdgesStack tempStack = new EdgesStack();
                        if (eStack.Count == 1)
                        {
                            temp = eStack.Get();
                            vStack.Pop();
                            G.DelEdge(temp);
                            tempStack.Put(temp);
                            i++;
                        }
                        else
                        {
                            do
                            {
                                temp = eStack.Get();
                                vStack.Pop();
                                G.DelEdge(temp);
                                visited[temp.From] = false;
                                tempStack.Put(temp);
                                i++;
                            } while (temp.From != v);
                            if (eStack.Count != 0)
                                lastv = eStack.Peek().From;
                        }
                        eTable[numOfCycles] = new Edge[i];
                        i = 0;
                        while (!tempStack.Empty)
                            eTable[numOfCycles][i++] = tempStack.Get();
                        numOfCycles++;
                        continue;
                    }
                    else
                    {
                        visited[v] = true;
                        foreach (var e in G.OutEdges(v))
                            if (e.To != lastv)
                            {
                                vStack.Push(e.To);
                                eStack.Put(e);
                                break;
                            }
                        lastv = v;
                    }
                }
            }
        }
        ///WERSJA LISTOWA -- WOLNIEJSZA O OK 3%
        /*
        public static Edge[][] cyclePartition2(this Graph G)
        {
            Graph gr = G.Clone();
            List<List<Edge>> cycleLists = new List<List<Edge>>();
            EdgesStack eStack = new EdgesStack();
            bool[] visited = new bool[gr.VerticesCount];
            DeepSearch2(ref gr,ref eStack,ref visited,ref cycleLists);
            int numOfCycles = cycleLists.Count();

            int i = 0;
            Edge[][] resultTable = new Edge[numOfCycles][];
            foreach (var list in cycleLists)
            {
                list.Reverse();
                resultTable[i++] = list.ToArray();
            }

            return resultTable;
        }
        public static void DeepSearch2(ref Graph G,ref EdgesStack eStack,ref bool[] visited,ref List<List<Edge>> eTable)
        {
            Stack<int> vStack;
            int lastv = -1;
            for (int s = 0; s < G.VerticesCount; s++)
            {
                vStack = new Stack<int>();
                while (G.OutDegree(s) > 0)
                {
                    if (vStack.Count == 0)
                        vStack.Push(s);
                    int v = vStack.Peek();
                    if (G.OutDegree(v) == 0)
                        break;
                    if (visited[v])
                    {
                        Edge temp; List<Edge> list = new List<Edge>();
                        if (eStack.Count == 1)
                        {
                            temp = eStack.Get();
                            vStack.Pop();
                            G.DelEdge(temp);
                            list.Add(temp);
                        }
                        else
                        {
                            do
                            {
                                temp = eStack.Get();
                                vStack.Pop();
                                G.DelEdge(temp);
                                visited[temp.From] = false;
                                list.Add(temp);
                            } while (temp.From != v);
                            if (eStack.Count != 0)
                                lastv = eStack.Peek().From;
                        }
                        eTable.Add(list);
                        continue;
                    }
                    else
                    {
                        visited[v] = true;
                        foreach (var e in G.OutEdges(v))
                            if (e.To != lastv)
                            {
                                vStack.Push(e.To);
                                eStack.Put(e);
                                break;
                            }
                        lastv = v;
                    }
                }
            }
        } 
        */

        /// <summary>
        /// Szukanie skojarzenia doskonałego w grafie nieskierowanym o którym zakładamy, że jest dwudzielny i 2^r-regularny
        /// (nie trzeba sprawdzać poprawności danych)
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Skojarzenie doskonałe w G</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m), gdzie m jest liczbą krawędzi grafu G
        /// </remarks>
        public static Graph perfectMatching(this Graph G)
        {
            Edge[][] tables;
            Graph X;
            int vCount = G.VerticesCount;
            bool[] visited = new bool[vCount];
            tables = G.cyclePartition();
            X = G.IsolatedVerticesGraph();
            while (true)
            {
                for (int q = 0; q < tables.Length; q++)
                {
                    int len = tables[q].Length;
                    for (int i = 0; i < len; i += 2)
                        X.AddEdge(tables[q][i]);
                }
                if (X.EdgesCount == X.VerticesCount / 2)
                    break;
                tables = X.cyclePartition();
                X = G.IsolatedVerticesGraph();
            }
            return X;
        }
    }
}
