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
            int minimumTime = int.MaxValue;
            bool[] rightSide = new bool[times.Length];
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
                    if (rightSide[secondPerson]|| currentTime + Math.Max(times[firstPerson], times[secondPerson]) > minimumTime)
                        continue;

                    currentTime += Math.Max(times[firstPerson], times[secondPerson]);
                    rightSide[firstPerson] = rightSide[secondPerson] = true;
                    List<int> temp = new List<int>() { firstPerson, secondPerson };
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
            if (rightSide.Count(b => b) == times.Length)
            {
                bestStrategy = strategy.ToList();
                minimumTime = currentTime;
                return;
            }

            //ROZWIĄZANIE EXTRA TESTÓW -- WYBIERAMY ZAWSZE NAJSZYBSZĄ OSOBĘ DO POWROTU
            int firstPerson2 = -1;
            int min = int.MaxValue;
            for(int i=0;i<times.Length;i++)
            {
                if (rightSide[i] && times[i] < min)
                {
                    firstPerson2 = i;
                    min = times[i];
                }
            }

            //------- PĘTLA NIE JEST 'AKTYWNA' - BREAK WYKONUJE SIĘ PO POJEDYNCZYM PRZEJŚCIU ---------------
            for (int firstPerson = 0; firstPerson < times.Length; firstPerson++)
            {
                if (!rightSide[firstPerson2] || currentTime + times[firstPerson2] > minimumTime)
                    continue;

                currentTime += times[firstPerson2];
                rightSide[firstPerson2] = false;
                List<int> temp = new List<int>() { firstPerson2 };
                strategy.Add(temp);
                CrossBridgeLeftToRight(times, ref strategy, rightSide, currentTime, ref minimumTime);

                //powrót do przeszłości
                currentTime -= times[firstPerson2];
                rightSide[firstPerson2] = true;
                strategy.Remove(temp);
                break;
            }
        }
        // MOŻESZ DOPISAĆ POTRZEBNE POLA I METODY POMOCNICZE
        // MOŻESZ NAWET DODAĆ CAŁE KLASY (ALE NIE MUSISZ)

    }
}
