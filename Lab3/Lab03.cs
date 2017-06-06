
using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace ASD
{

    public static class Lab03GraphExtender
    {

        /// <summary>
        /// Wyszukiwanie cykli w grafie
        /// </summary>
        /// <param name="g">Badany graf</param>
        /// <param name="cycle">Znaleziony cykl</param>
        /// <returns>Informacja czy graf jest acykliczny</returns>
        /// <remarks>
        /// 1) Algorytm powinien dzia�a� zar�wno dla graf�w skierowanych, jak i nieskierowanych
        /// 2) Grafu nie wolno zmienia�
        /// 3) Je�li graf zawiera cykl to parametr cycle powinien by� tablic� kraw�dzi tworz�cych dowolny z cykli.
        ///    Kraw�dzie musz� by� umieszczone we w�a�ciwej kolejno�ci (tak jak w cyklu, mo�na rozpocz�� od dowolnej)
        /// 4) Je�li w grafie nie ma cyklu to parametr cycle ma warto�� null.
        /// </remarks>
        public static bool FindCycle(this Graph g,out Edge[] cycle)
        {
            int verticesCount = g.VerticesCount;
            int cc = 0;
            Stack<int> lastFrom = new Stack<int>();
            List<Edge> cycleList = new List<Edge>();         
            bool[] visited = new bool[verticesCount];

            Predicate<int> preVisit = id =>
            {
                visited[id] = true;
                return true;
            };
            Predicate<int> postVisit = id =>
            {
                visited[id] = false;
                if(lastFrom.Count!=0)
                    lastFrom.Pop();
                if (cycleList.Count!=0)
                    cycleList.RemoveAt(cycleList.Count-1);
                return true;
            };
            Predicate<Edge> isTree = e =>
            {
                if (!g.Directed && lastFrom.Count!=0 && e.To == lastFrom.Peek())
                    return true;
                cycleList.Add(e);
                if (visited[e.To])
                    return false;
                lastFrom.Push(e.From);
                return true;
            };
            if (!g.GeneralSearchAll<EdgesStack>(preVisit,postVisit,isTree,out cc))
            {
                int i = 0;
                int listCount = cycleList.Count;
                int startVertex = cycleList[listCount - 1].To;
                for (; i < listCount; i++)
                    if (cycleList[i].From == startVertex)
                        break;
                cycleList.RemoveRange(0,i);
                cycle = cycleList.ToArray();
                return true;
            }

            cycle = null;
            return false;
        }

        /// <summary>
        /// Wyznaczanie centrum drzewa
        /// </summary>
        /// <param name="g">Badany graf</param>
        /// <param name="center">Znalezione centrum</param>
        /// <returns>Informacja czy badany graf jest drzewem</returns>
        /// <remarks>
        /// 1) Dla graf�w skierowanych metoda powinna zg�asza� wyj�tek ArgumentException
        /// 2) Grafu nie wolno zmienia�
        /// 3) Parametr center to 1-elementowa lub 2-elementowa tablica zawieraj�ca numery wierzcho�k�w stanowi�cych centrum.
        ///    (w przypadku 2 wierzcho�k�w ich kolejno�� jest dowolna)
        /// </remarks>
        public static bool TreeCenter(this Graph g,out int[] center)
        {
            if (g.Directed)
                throw new ArgumentException();
            if(g == null)
            {
                center = null;
                return false;
            }
            if (g.VerticesCount == 1)
            {
                center = new int[1];
                center[0] = 0;
                return true;
            }

            //Sprawdzenie ilo�ci sk�adowych
            int cc = 0;
            g.GeneralSearchAll<EdgesStack>(null,null,null,out cc);
            if (cc > 1)
            {
                center = null;
                return false;
            }

            //G��wny algorytm 'wyrzucania' wierzcho�k�w
            int verticesCount = g.VerticesCount;
            int numOfLeftVertices = verticesCount;
            bool[] isLeaf = new bool[verticesCount];
            bool addedSomething = false;
            int deg;
            List<int> leaves = new List<int>();
            while (numOfLeftVertices > 2)
            {
                addedSomething = false;

                for (int i = 0; i < verticesCount; i++)
                {
                    deg = 0;
                    if (!isLeaf[i])
                    {
                        foreach (var x in g.OutEdges(i))
                            if (!isLeaf[x.To])
                                deg++;
                    }
                    if (deg == 1)
                    {
                        leaves.Add(i);
                        addedSomething = true;
                    }
                }
                numOfLeftVertices -= leaves.Count;
                foreach (var x in leaves)
                    isLeaf[x] = true;
                leaves.Clear();
                //Znaleziono cykl
                if (!addedSomething)
                { center = null; return false; }

            }

            int? a = null; int? b = null;
            for (int i = 0; i < verticesCount; i++)
            {
                if (!isLeaf[i] && a == null)
                    a = i;
                else if (!isLeaf[i])
                    b = i;
            }
            if (b != null)
            {
                center = new int[2];
                center[0] = (int)a; center[1] = (int)b;
            }
            else
            {
                center = new int[1];
                center[0] = (int)a;
            }

            return true;
        }

    }

}
