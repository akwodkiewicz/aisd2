
using System.Collections.Generic;
using ASD.Graphs;

namespace lab10
{

    public struct AlmostMatchingSolution
    {
        public AlmostMatchingSolution(int edgesCount, List<Edge> solution)
        {
            this.edgesCount = edgesCount;
            this.solution = solution;
        }

        public readonly int edgesCount;
        public readonly List<Edge> solution;
    }



    public class AlmostMatching
    {
        /// <summary>
        /// Zwraca najliczniejszy możliwy zbiór krawędzi, którego poziom
        /// ryzyka nie przekracza limitu. W ostatnim etapie zwracać
        /// zbiór o najmniejszej sumie wag ze wszystkich najliczniejszych.
        /// </summary>
        /// <returns>Liczba i lista linek (krawędzi)</returns>
        /// <param name="g">Graf linek</param>
        /// <param name="allowedCollisions">Limit ryzyka</param>
        public static AlmostMatchingSolution LargestS(Graph g, int allowedCollisions)
        {
            //Reset statycznej 'najlepszej listy krawędzi'
            bestList = new List<Edge>();

            //Deklaracja zmiennych
            int maxEdgeCount = 0;
            double minWeight = double.PositiveInfinity;
            List<Edge> currentList = new List<Edge>();
            Graph gModified = g.IsolatedVerticesGraph();

            //Tworzenie listy wszystkich krawędzi
            List<Edge> edgeList = new List<Edge>();
            for (int i = 0; i < g.VerticesCount; i++)
            {
                foreach (var e in g.OutEdges(i))
                {
                    if (e.From > e.To)
                        continue;
                    edgeList.Add(e);
                }
            }
            AlmostRecursive(ref gModified, edgeList, 0, 0, allowedCollisions, currentList, ref maxEdgeCount, 0.0, ref minWeight);

            return new AlmostMatchingSolution(maxEdgeCount, bestList);
        }

        public static List<Edge> bestList;
        public static void AlmostRecursive(ref Graph gModified, List<Edge> edgeList, int index, 
                                            int numOfCollisions, int allowedCollisions,
                                            List<Edge> currentList, ref int maxEdgeCount,
                                            double currentWeight, ref double minWeight)
        {
            //Skończyły się nam krawędzie
            if (index == edgeList.Count)
            {
                if (currentList.Count == maxEdgeCount && currentWeight > minWeight)
                    return;
                else if (currentList.Count >= maxEdgeCount)
                {
                    minWeight = currentWeight;
                    maxEdgeCount = currentList.Count;
                    bestList = new List<Edge>(currentList);
                }
                return;
            }

            //Czy warto kontynuować rekursję?
            if (edgeList.Count - index + currentList.Count < maxEdgeCount)
                return;

            //Pobranie kolejnej krawędzi
            Edge temp = edgeList[index];

            //Rekurencja bez dodawania:
            AlmostRecursive(ref gModified, edgeList, index + 1, numOfCollisions, allowedCollisions, currentList, ref maxEdgeCount, currentWeight, ref minWeight);

            //Czy możemy dodać krawędź do rozwiązania
            var collisions = (gModified.OutDegree(temp.From) > 0) ? ((gModified.OutDegree(temp.To) > 0) ? 2 : 1) : (gModified.OutDegree(temp.To) > 0) ? 1 : 0;
            if (numOfCollisions + collisions <= allowedCollisions)
            {
                currentList.Add(temp);
                gModified.AddEdge(temp);

                //Odpalenie rekurencji z dodawaniem
                AlmostRecursive(ref gModified, edgeList, index + 1, numOfCollisions + collisions, allowedCollisions, currentList, ref maxEdgeCount, currentWeight + temp.Weight, ref minWeight);

                currentList.Remove(temp);
                gModified.DelEdge(temp);
            }

            return;
        }
    }

}


