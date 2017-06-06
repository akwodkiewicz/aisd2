using System;
using ASD.Graphs;

namespace ASD
{


    public static class SmurfphoneFactory
    {
        /// <summary>
        /// Metoda zwraca największą możliwą do wyprodukowania liczbę smerfonów
        /// </summary>
        /// <param name="providers">Dostawcy</param>
        /// <param name="factories">Fabryki</param>
        /// <param name="distanceCostMultiplier">współczynnik kosztu przewozu</param>
        /// <param name="productionCost">Łączny koszt produkcji wszystkich smerfonów</param>
        /// <param name="transport">Tablica opisująca ilości transportowanych surowców miedzy poszczególnymi dostawcami i fabrykami</param>
        /// <param name="maximumProduction">Maksymalny rozmiar produkcji</param>
        public static double CalculateFlow(Provider[] providers, Factory[] factories, double distanceCostMultiplier, out double productionCost, out int[,] transport, int maximumProduction = int.MaxValue)
        {
            /*
            dostawcy wejściowi:   [0; providers.Length-1]
            fabryki główne:       [providers.Length;  providers.Length+factories.Length-1]
            fabryki nadgodzinne:  [providers.Length+factories.Length;  providers.Length+2*factories.Length-1]
            */
            int source = providers.Length + 2 * factories.Length;
            int postsource = source + 1;
            int sink = source + 2;
            Graph graph = new AdjacencyMatrixGraph(true, providers.Length + 2 * factories.Length + 3);
            Graph costs = new AdjacencyMatrixGraph(true, providers.Length + 2 * factories.Length + 3);

            //Dodawanie krawędzi od 3 wierzchołków: źródła, post-źródła i ujścia
            graph.Add(new Edge(source, postsource, maximumProduction));
            costs.Add(new Edge(source, postsource, 0.0));
            for (int p = 0; p < providers.Length; p++)
            {
                graph.Add(new Edge(postsource, p, providers[p].Capacity));
                costs.Add(new Edge(postsource, p, providers[p].Cost));
            }
            for (int f = 0; f < factories.Length; f++)
            {
                graph.Add(new Edge(providers.Length + f, sink, factories[f].Limit));
                graph.Add(new Edge(providers.Length + factories.Length + f, sink, int.MaxValue));
                costs.Add(new Edge(providers.Length + f, sink, factories[f].LowerCost));
                costs.Add(new Edge(providers.Length + factories.Length + f, sink, factories[f].HigherCost));
            }

            //Proste łączenie dostawców fabrykami głownymi
            for (int p = 0; p < providers.Length; p++)
                for (int f = 0; f < factories.Length; f++)
                {
                    graph.Add(new Edge(p, providers.Length + f, int.MaxValue));
                    costs.Add(new Edge(p, providers.Length + f, Math.Ceiling(distanceCostMultiplier * Distance(providers[p], factories[f]))));
                }

            //Łączenie głównych fabryk z fabrykami nadgodzinnymi
            for (int f = 0; f < factories.Length; f++)
            {
                graph.Add(new Edge(providers.Length + f, providers.Length + factories.Length + f, int.MaxValue));
                costs.Add(new Edge(providers.Length + f, providers.Length + factories.Length + f, 0.0));
            }


            Graph flows;
            var result = MinCostFlowGraphExtender.MinCostFlow(graph, costs, source, sink, out productionCost, out flows);

            transport = new int[providers.Length, factories.Length];
            for (int i = 0; i < providers.Length; i++)
            {
                foreach (var e in flows.OutEdges(i))
                    if (e.Weight != 0)
                        transport[i, e.To - providers.Length] = (int)e.Weight;
            }

            return result;
        }
        public static double Distance(Provider p, Factory f)
        {
            return Math.Sqrt(Math.Pow((p.Position.X - f.Position.X), 2) + Math.Pow((p.Position.Y - f.Position.Y), 2));
        }
    }
}
