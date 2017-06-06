using System.Collections.Generic;
using System;
using static ASD.Geometry;

namespace ASD
{
    public static class SutherlandHodgman
    {
        /// <summary>
        /// Oblicza pole wielokata przy pomocy formuly Gaussa
        /// </summary>
        /// <param name="polygon">Kolejne wierzcholki wielokata</param>
        /// <returns>Pole wielokata</returns>
        public static double PolygonArea(this Point[] polygon)
        {
            var n = polygon.Length;
            if (n < 3)
                return 0.0;

            var sum = 0.0;
            for (int i = 2; i < n; i++)
                    sum += (
                    polygon[i - 1].x * polygon[i].y
                    - polygon[i - 1].x * polygon[i - 2].y
                    );
            sum += (
                polygon[0].x * polygon[1].y
                - polygon[0].x * polygon[n - 1].y
                );
            sum += (
                polygon[n-1].x * polygon[0].y
                - polygon[n-1].x * polygon[n - 2].y
                );
            return Math.Abs(sum * 0.5);
        }

        /// <summary>
        /// Sprawdza, czy dwa punkty leza po tej samej stronie prostej wyznaczonej przez odcinek s
        /// </summary>
        /// <param name="p1">Pierwszy punkt</param>
        /// <param name="p2">Drugi punkt</param>
        /// <param name="s">Odcinek wyznaczajacy prosta</param>
        /// <returns>
        /// True, jesli punkty leza po tej samej stronie prostej wyznaczonej przez odcinek 
        /// (lub co najmniej jeden z punktow lezy na prostej). W przeciwnym przypadku zwraca false.
        /// </returns>
        public static bool IsSameSide(Point p1, Point p2, Segment s)
        {
            var d1 = ((s.pe.x - s.ps.x) * (p1.y - s.ps.y) - (s.pe.y - s.ps.y) * (p1.x - s.ps.x));
            var d2 = ((s.pe.x - s.ps.x) * (p2.y - s.ps.y) - (s.pe.y - s.ps.y) * (p2.x - s.ps.x));

            return d1 * d2 >= 0;
        }

        /// <summary>Oblicza czesc wspolna dwoch wielokatow przy pomocy algorytmu Sutherlanda–Hodgmana</summary>
        /// <param name="subjectPolygon">Wielokat obcinany (wklesly lub wypukly)</param>
        /// <param name="clipPolygon">Wielokat obcinajacy (musi byc wypukly i zakladamy, ze taki jest)</param>
        /// <returns>Czesc wspolna wielokatow</returns>
        /// <remarks>
        /// - mozna zalozyc, ze 3 kolejne punkty w kazdym z wejsciowych wielokatow nie sa wspolliniowe
        /// - wynikiem dzialania funkcji moze byc tak naprawde wiele wielokatow (sytuacja taka moze wystapic,
        ///   jesli wielokat obcinany jest wklesly)
        /// - jesli wielokat obcinany i obcinajacy zawieraja wierzcholki o tych samych wspolrzednych,
        ///   w wynikowym wielokacie moge one byc zduplikowane
        /// - wierzcholki wielokata obcinanego, przez ktore przechodza krawedzie wielokata obcinajacego
        ///   zostaja zduplikowane w wielokacie wyjsciowym
        /// </remarks>
        public static Point[] GetIntersectedPolygon(Point[] subjectPolygon, Point[] clipPolygon)
        {
            var output = new List<Point>(subjectPolygon);
            for(int i = 0; i<clipPolygon.Length; i++)
            {
                Segment s = new Segment(clipPolygon[i % clipPolygon.Length], clipPolygon[(i + 1) % clipPolygon.Length]);
                Point pNext = clipPolygon[(i+2) % clipPolygon.Length];

                var input = new List<Point>(output);
                output.Clear();
                var pp = input[input.Count - 1];
                foreach(var p in input)
                {
                    if (IsSameSide(p, pNext, s))
                    {
                        if (!(IsSameSide(pp, pNext, s)))
                            output.Add(GetIntersectionPoint(new Segment(pp, p), s));
                        output.Add(p);
                    }
                    else if (IsSameSide(pp, pNext, s))
                        output.Add(GetIntersectionPoint(new Segment(pp, p), s));
                    pp = p;
                }
            }

            for(int i=0; i<output.Count-1; i++)
            {
                if (output[i] == output[i + 1])
                {
                    output.RemoveAt(i + 1);
                    i--;
                }
            }
            return output.ToArray();
        }


        /// <summary>
        /// Zwraca punkt przeciecia dwoch prostych wyznaczonych przez odcinki
        /// </summary>
        /// <param name="seg1">Odcinek pierwszy</param>
        /// <param name="seg2">Odcinek drugi</param>
        /// <returns>Punkt przeciecia prostych wyznaczonych przez odcinki</returns>
        public static Point GetIntersectionPoint(Segment seg1, Segment seg2)
        {
            Point direction1 = new Point(seg1.pe.x - seg1.ps.x, seg1.pe.y - seg1.ps.y);
            Point direction2 = new Point(seg2.pe.x - seg2.ps.x, seg2.pe.y - seg2.ps.y);
            double dotPerp = (direction1.x * direction2.y) - (direction1.y * direction2.x);

            Point c = new Point(seg2.ps.x - seg1.ps.x, seg2.ps.y - seg1.ps.y);
            double t = (c.x * direction2.y - c.y * direction2.x) / dotPerp;

            return new Point(seg1.ps.x + (t * direction1.x), seg1.ps.y + (t * direction1.y));
        }
    }
}
