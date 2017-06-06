using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    public static class Lab06GraphExtender
    {

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki,
        /// algorytm zwróci jedną z nich).
        /// Dopuszczamy, aby na ścieżce powtarzały się wierzchołki/krawędzie.
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność do O(D), gdzie D jest złożonością implementacji alogorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g"></param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako ciąg krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość znalezionej ścieżki</returns>
        public static double? FindSecondShortestPath(this Graph g, int a, int b, out Edge[] path)
        {
            path = null;
            PathsInfo[] reversePathInfo;
            List<Edge> bestPath = new List<Edge>();
            if (g.Directed)
            {
                Graph gReversed = g.Reverse();
                if (!gReversed.DijkstraShortestPaths(b, out reversePathInfo))
                    return null;
            }
            else
                if (!g.DijkstraShortestPaths(b, out reversePathInfo))
                return null;
            int v = a;
            while (reversePathInfo[v].Last != null)
            {
                var last = (Edge)reversePathInfo[v].Last;
                bestPath.Add(new Edge(last.To, last.From, last.Weight));
                v = last.From;
            }
            double currentDistance = 0.0;
            double secondDistance = double.PositiveInfinity;
            Edge extended = new Edge(-1, -1, double.PositiveInfinity);
            foreach (var currentEdge in bestPath)
            {
                foreach (var e in g.OutEdges(currentEdge.From))
                {
                    if (e == currentEdge)
                        continue;
                    if (currentDistance + e.Weight + reversePathInfo[e.To].Dist <= secondDistance)
                    {
                        extended = e;
                        secondDistance = currentDistance + e.Weight + reversePathInfo[e.To].Dist;
                    }
                }
                currentDistance += currentEdge.Weight;
            }
            List<Edge> pathList = new List<Edge>();
            if (extended.Weight != double.PositiveInfinity)
            {
                foreach (var e in bestPath)
                {
                    if (e.From == extended.From)
                        break;
                    else
                        pathList.Add(e);
                }
                pathList.Add(extended);
                v = extended.To;
                while (reversePathInfo[v].Last != null)
                {
                    var last = (Edge)reversePathInfo[v].Last;
                    pathList.Add(new Edge(last.To, last.From, last.Weight));
                    v = last.From;
                }
                path = pathList.ToArray();
            }
            if (path == null)
                return null;
            else
                return secondDistance;
        }

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki,
        /// algorytm zwróci jedną z nich).
        /// Wymagamy, aby na ścieżce nie było powtórzeń wierzchołków ani krawędzi.  
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność to O(nD), gdzie D jest złożonością implementacji algorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g"></param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako ciąg krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość tej ścieżki</returns>
        public static double? FindSecondSimpleShortestPath(this Graph g, int a, int b, out Edge[] path)
        {
            path = null;
            PathsInfo[] bestPathInfo, tempPathInfo, secondBestPathInfo = null;
            double secondBest = double.PositiveInfinity;
            EdgesStack stack = new EdgesStack();
            if (!g.DijkstraShortestPaths(a, out bestPathInfo))
                return null;
            int v = b;
            int pathCount = 0;
            while (bestPathInfo[v].Last != null)
            {
                stack.Put((Edge)bestPathInfo[v].Last);
                v = stack.Peek().From;
                pathCount++;
            }
            Edge deleted;
            while (!stack.Empty)
            {
                deleted = stack.Get();
                g.DelEdge(deleted);
                g.DijkstraShortestPaths(a, out tempPathInfo);
                if (tempPathInfo[b].Dist < secondBest)
                {
                    secondBestPathInfo = tempPathInfo;
                    secondBest = tempPathInfo[b].Dist;
                }
                g.AddEdge(deleted);
            }
            List<Edge> pathList = new List<Edge>();
            v = b;
            if (!double.IsPositiveInfinity(secondBest))
            {
                while (secondBestPathInfo[v].Last != null)
                {
                    pathList.Add((Edge)secondBestPathInfo[v].Last);
                    v = pathList.Last().From;
                }
                pathList.Reverse();
                path = pathList.ToArray();
            }
            if (path == null)
                return null;
            else
                return pathList.Sum(edge => edge.Weight);
        }
    }
}
