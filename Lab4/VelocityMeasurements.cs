using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    class VelocityMeasurements
    {
        /// <summary>
        /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w momencie wypadku.
        /// </summary>
        /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
        /// <param name="isBrakingValue">Tablica zwracająca informację dla każdego z pomiarów z tablicy measurements informację bool czy dla sekwencji dającej 
        /// minimalną prędkość wynikową traktować dany pomiar jako hamujący (true) przy przyspieszający (false)</param>
        /// <returns>Struktura Velocities z informacjami o najniższej i najwyższej możliwej prędkości w momencie wypadku</returns>
        /// 
        /// <remarks>
        /// Złożoność pamięciowa algorytmu powinna być nie większa niż O(sumy_wartości_pomiarów).
        /// Złożoność czasowa algorytmu powinna być nie większa niż O(liczby_pomiarów * sumy_wartości_pomiarów).
        /// </remarks>
        public static Velocities FinalVelocities(int[] measurements,out bool[] isBrakingValue)
        {
            int minVelocity = 0, maxVelocity = 0, len = measurements.Length, v, bestBrakingVelocity = 0;
            maxVelocity = measurements.Sum();
            int halfVelocity = maxVelocity / 2, temp;
            int[] lastMeasurement = new int[halfVelocity + 1];
            isBrakingValue = new bool[len];
            lastMeasurement[0] = -1;
            for (int i = 1; i <= halfVelocity; i++)
                lastMeasurement[i] = -2;

            for (int i = 0; i < len; i++)
            {
                if (measurements[i] == 0
                    || measurements[i] > halfVelocity)
                    continue;
                for (v = 0; v <= halfVelocity - measurements[i]; v++)
                {
                    if (lastMeasurement[v] != -2                    // v było już osiągnięte...
                        && lastMeasurement[v] != i                      // ...w poprzednich iteracjach (nie aktualnej!),
                        && lastMeasurement[v + measurements[i]] == -2) // a lepsza prędkość nie była dotychczas osiągnięta (jeśli była, to nie pogarszać wyniku)
                    {
                        temp = v + measurements[i];
                        lastMeasurement[temp] = i;
                        if (temp > bestBrakingVelocity)
                            bestBrakingVelocity = temp;
                    }
                }
            }

            v = bestBrakingVelocity;
            if (lastMeasurement[bestBrakingVelocity] >= 0)
                while (v > 0)
                {
                    isBrakingValue[lastMeasurement[v]] = true;
                    v -= measurements[lastMeasurement[v]];
                }
            minVelocity = maxVelocity - (2 * bestBrakingVelocity);
            return new Velocities(minVelocity,maxVelocity);
        }


        /// <summary>
        /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w trakcie całego okresu trwania podróży.
        /// </summary>
        /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
        /// <param name="isBrakingValue">W tej wersji algorytmu proszę ustawić parametr na null</param>
        /// <returns>Struktura Velocities z informacjami o najniższej i najwyższej możliwej prędkości na trasie</returns>
        /// 
        /// <remarks>
        /// Złożoność pamięciowa algorytmu powinna być nie większa niż O(sumy_wartości_pomiarów).
        /// Złożoność czasowa algorytmu powinna być nie większa niż O(liczby_pomiarów * sumy_wartości_pomiarów).
        /// </remarks>
        public static Velocities JourneyVelocities(int[] measurements,out bool[] isBrakingValue)
        {
            isBrakingValue = null;  // Nie zmieniać !!!
            int len = measurements.Length, v, 
                maxVelocity = measurements.Sum(), halfVelocity = maxVelocity / 2,
                bestMinVelocity = 0, currentBestBrakingVelocity = -1,
                currentHalfVelocity, currentMaxVelocity = 0;
            int[] lastMeasurement = new int[halfVelocity + 1];
            lastMeasurement[0] = -1;
            for (int i = 1; i <= halfVelocity; i++)
                lastMeasurement[i] = -2;

            for (int i = 0; i < len; i++)
            {
                if (measurements[i] == 0)
                    continue;

                currentMaxVelocity += measurements[i];
                currentHalfVelocity = currentMaxVelocity / 2;
                if (currentBestBrakingVelocity == -1)
                {
                    currentBestBrakingVelocity = 0;
                    bestMinVelocity = currentMaxVelocity - 2 * currentBestBrakingVelocity;
                }

                if (measurements[i] > halfVelocity)
                    continue;
                for (v = 0; v <= halfVelocity - measurements[i]; v++)
                {
                    if (lastMeasurement[v] != -2                        // v było już osiągnięte...
                        && lastMeasurement[v] != i)                     // ...w poprzednich iteracjach (nie aktualnej!), 
                    {
                        if(lastMeasurement[v + measurements[i]] == -2) // ten warunek nie jest uwzględniany do wyliczania bestMinVelocity
                            lastMeasurement[v + measurements[i]] = i;

                        if (v + measurements[i] <= currentHalfVelocity
                            && currentMaxVelocity - (2 * (v + measurements[i])) < bestMinVelocity)
                        {
                            currentBestBrakingVelocity = v + measurements[i];
                            bestMinVelocity = currentMaxVelocity - (2 * currentBestBrakingVelocity);
                            if(bestMinVelocity==0)
                                return new Velocities(bestMinVelocity,maxVelocity);
                        }
                    }
                }
            }
                        
            return new Velocities(bestMinVelocity,maxVelocity);
        }
    }
}
