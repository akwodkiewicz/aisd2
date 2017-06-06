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
        /// <returns>Minimalny czas, w jakim wszyscy turyści mogą pokonać most</returns>
        public static int CrossBridge(int[] times, out List<List<int>> strategy)
        {
            strategy = null;
            return 0;
        }

        // MOŻESZ DOPISAĆ POTRZEBNE POLA I METODY POMOCNICZE
        // MOŻESZ NAWET DODAĆ CAŁE KLASY (ALE NIE MUSISZ)

    }
}