using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class BridgeCrossing
    {

        /// <summary>
        /// Metoda rozwiązuje zadanie optymalnego przechodzenia przez most.
        /// </summary>
        /// <param name="_times">Tablica z czasami przejścia poszczególnych osób</param>
        /// <param name="strategy">Strategia przekraczania mostu: lista list identyfikatorów kolejnych osób,
        /// które przekraczają most (na miejscach parzystych przejścia par przez most,
        /// na miejscach nieparzystych powroty jednej osoby z latarką). Jeśli istnieje więcej niż jedna strategia
        /// realizująca przejście w optymalnym czasie wystarczy zwrócić dowolną z nich.</param>
        /// <returns>minimalny czas, w jakim wszyscy turyści mogą pokonać most</returns>
        public static int CrossBridge(int[] times, out List<List<int>> strategy)
        {
            strategy = new List<List<int>>();
            bestStrategy = new List<List<int>>();
            var minimumTime = int.MaxValue;
            var rightSide = new bool[times.Length];
            CrossBridgeLeftToRight(times, ref strategy, rightSide, 0, ref minimumTime);
            strategy = bestStrategy;
            return minimumTime;
        }

        private static List<List<int>> bestStrategy;
        private static void CrossBridgeLeftToRight(int[] times, ref List<List<int>> strategy, bool[] rightSide, int currentTime, ref int minimumTime)
        {
            if (times.Length == 2)
            {
                bestStrategy.Add(new List<int>() { 0, 1 });
                minimumTime = Math.Max(times[0], times[1]);
                return;
            }
            else if (times.Length == 1)
            {
                bestStrategy.Add(new List<int>() { 0 });
                minimumTime = times[0];
                return;
            }
            for (int firstPerson = 0; firstPerson < times.Length - 1; firstPerson++)
            {
                if (rightSide[firstPerson])
                    continue;
                for (int secondPerson = firstPerson + 1; secondPerson < times.Length; secondPerson++)
                {
                    if (rightSide[secondPerson] || currentTime + Math.Max(times[firstPerson], times[secondPerson]) > minimumTime)
                        continue;

                    currentTime += Math.Max(times[firstPerson], times[secondPerson]);
                    rightSide[firstPerson] = rightSide[secondPerson] = true;
                    var temp = new List<int>() { firstPerson, secondPerson };
                    strategy.Add(temp);
                    CrossBridgeRightToLeft(times, ref strategy, rightSide, currentTime, ref minimumTime);

                    //powrót do przeszłości
                    rightSide[firstPerson] = rightSide[secondPerson] = false;
                    strategy.Remove(temp);
                    currentTime -= Math.Max(times[firstPerson], times[secondPerson]);
                }
            }
            return;
        }

        private static void CrossBridgeRightToLeft(int[] times, ref List<List<int>> strategy, bool[] rightSide, int currentTime, ref int minimumTime)
        {
            //Wszystkie osoby przeszły na prawą stronę
            if (rightSide.Count(b => b) == times.Length)
            {
                bestStrategy = strategy.ToList();
                minimumTime = currentTime;
                return;
            }

            //ROZWIĄZANIE EXTRA TESTÓW -- WYBIERAMY ZAWSZE NAJSZYBSZĄ OSOBĘ DO POWROTU
            var firstPerson = -1;
            var min = int.MaxValue;
            for (var i = 0; i < times.Length; i++)
            {
                if (rightSide[i] && times[i] < min)
                {
                    firstPerson = i;
                    min = times[i];
                }
            }

            //Obcięcie nieoptymalnego rozwiązania
            if (currentTime + times[firstPerson] > minimumTime)
                return;

            //Krok wgłąb
            currentTime += times[firstPerson];
            rightSide[firstPerson] = false;
            var temp = new List<int>() { firstPerson };
            strategy.Add(temp);
            CrossBridgeLeftToRight(times, ref strategy, rightSide, currentTime, ref minimumTime);

            //Powrót do przeszłości
            currentTime -= times[firstPerson];
            rightSide[firstPerson] = true;
            strategy.Remove(temp);


        }
        // MOŻESZ DOPISAĆ POTRZEBNE POLA I METODY POMOCNICZE
        // MOŻESZ NAWET DODAĆ CAŁE KLASY (ALE NIE MUSISZ)

    }
}
