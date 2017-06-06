using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public partial struct Point
    {

        /// <summary>
        /// Metoda zwraca odległość w przestrzeni Euklidesowej między dwoma punktami w przestrzeni dwuwymiarowej
        /// </summary>
        /// <param name="p1">Pierwszy punkt w przestrzeni dwuwymiarowej</param>
        /// <param name="p2">Drugi punkt w przestrzeni dwuwymiarowej</param>
        /// <returns>Odległość w przestrzeni Euklidesowej między dwoma punktami w przestrzeni dwuwymiarowej</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(1)
        /// </remarks>
        public static double Distance(Point p1, Point p2)
        {
            var dx = (p1.x - p2.x);
            var dy = (p1.y - p2.y);
            return Math.Sqrt(dx * dx + dy * dy);
        }

    }

    public class ByY : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            if (p1.y.CompareTo(p2.y) == 0)
                return p1.x.CompareTo(p2.x);
            return p1.y.CompareTo(p2.y);

        }
    }
    public class ByX : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            return p1.x.CompareTo(p2.x);
        }
    }

    class SweepClosestPair
    {

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(n^2), gdzie n to liczba punktów w chmurze
        /// </remarks>
        public static Tuple<Point, Point> FindClosestPointsBrute(List<Point> points, out double minDistance)
        {
            minDistance = int.MaxValue;
            Point a = new Point();
            Point b = new Point();
            for (int p1 = 0; p1 < points.Count; p1++)
                for (int p2 = p1 + 1; p2 < points.Count; p2++)
                    if (Point.Distance(points[p1], points[p2]) < minDistance)
                    {
                        a = points[p1];
                        b = points[p2];
                        minDistance = Point.Distance(a, b);
                    }

            return new Tuple<Point, Point>(a, b);
        }

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność n*logn, gdzie n to liczba punktów w chmurze
        /// </remarks>
        public static Tuple<Point, Point> FindClosestPoints(List<Point> points, out double minDistance)
        {
            var minDist = double.PositiveInfinity;
            int lastDeleted = 0;
            Point a = new Point();
            Point b = new Point();
            SortedSet<Point> tree = new SortedSet<Point>(new ByY());

            var pointsArray = points.ToArray();
            Array.Sort(pointsArray, new ByX());

            for (int i = 0; i < points.Count; i++)
            {
                var p = pointsArray[i];
                while (p.x - pointsArray[lastDeleted].x > minDist)
                    tree.Remove(pointsArray[lastDeleted++]);
     
                var result = tree.GetViewBetween(new Point(p.x, p.y - minDist), new Point(p.x, p.y + minDist));
                foreach (var q in result)
                    if (Point.Distance(p, q) < minDist)
                    {
                        a = p;
                        b = q;
                        minDist = Point.Distance(p, q);
                    }

                tree.Add(p);
            }

            minDistance = minDist;
            return new Tuple<Point, Point>(a, b);
        }

    }

}
