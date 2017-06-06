using System;
using System.Text;

namespace Lab14
{
    /// <summary>
    /// Klasa reprezentująca znalezione dopasowanie dwóch ciągów aminokwasów.
    /// </summary>
    public class Alignment
    {
        /// <summary>
        /// Początek podciągu v, który został dopasowany
        /// </summary>
        public int VStart { get; set; }
        /// <summary>
        /// Koniec podciągu v, który został dopasowany (używamy konwencji z stl, tj. to jest indeks pierwszego elementu za dopasowanym podciągiem.
        /// </summary>
        public int VEnd { get; set; }
        /// <summary>
        /// Początek podciągu w, który został dopasowany
        /// </summary>
        public int WStart { get; set; }
        /// <summary>
        /// Koniec podciągu w, który został dopasowany (używamy konwencji z stl, tj. to jest indeks pierwszego elementu za dopasowanym podciągiem.
        /// </summary>
        public int WEnd { get; set; }

        public string AlignedV { get; set; }
        public string AlignedW { get; set; }
    }

    /// <summary>
    /// Klasa znajdująca optymalne dopasowanie dwóch ciągów
    /// </summary>
    public class AlignmentFinder
    {
        // pomocniczy typ przydatny do śledzenia ostatnich operacji
        private enum LastOp
        {
            Skip,
            Change,
            GapV,
            GapW
        }

        /// <summary>
        /// Macierz oceny.
        /// Koszt zamiany znaku c1 na c2 odczytujemy Matrix[c1,c2], macierz ta jest jedynie opakowaniem słownika indeksowanego parami znaków.
        /// </summary>
        public ScoringMatrix Matrix
        {
            get;
        }
        /// <summary>
        /// Koszt wstawienia/usunięcia elementu.
        /// </summary>
        public int Epsilon
        {
            get;
        }

        public AlignmentFinder()
        {
            Matrix = ScoringMatrix.Simple;
            Epsilon = -1;
        }
        public AlignmentFinder(ScoringMatrix sm, int epsilon)
        {
            Matrix = sm;
            Epsilon = epsilon;
        }

        /// <summary>
        /// Funkcja znajdująca najlepsze dopasowanie ciągów (bez uwzględniania podciągów).
        /// </summary>
        /// <param name="v">pierwszy ciąg wejściowy</param>
        /// <param name="w">drugi ciąg wejściowy</param>
        /// <param name="alignment">obiekt opisujący najlepsze dopasowanie.
        /// Uwaga, w wersji bez uwzględniania podciągów ustaw:
        /// alignment.VStart = 0;
        /// alignment.WStart = 0;
        /// alignment.VEnd = v.Length;
        /// alignment.WEnd = w.Length;
        /// </param>
        /// <returns>wartość najlepszego dopasowania</returns>
        public int FindAlignment(string v, string w, out Alignment alignment)
        {
            alignment = new Alignment();
            int a = v.Length, b = w.Length;
            int[,] matrix = new int[v.Length + 1, w.Length + 1];
            LastOp[,] action = new LastOp[v.Length + 1, w.Length + 1];

            for (int i = 0; i <= a; i++)
            {
                matrix[i, 0] = i * Epsilon;
                action[i, 0] = LastOp.GapW;
            }
            for (int i = 0; i <= b; i++)
            {
                matrix[0, i] = i * Epsilon;
                action[0, i] = LastOp.GapV;
            }
            for (int i = 1; i <= a; i++)
                for (int j = 1; j <= b; j++)
                    matrix[i, j] = Maximum(
                                    matrix[i - 1, j] + Epsilon,
                                    matrix[i, j - 1] + Epsilon,
                                    matrix[i - 1, j - 1] + Matrix[v[i - 1], w[j - 1]],
                                    out action[i, j]
                                    );

            StringBuilder vRes = new StringBuilder();
            StringBuilder wRes = new StringBuilder();
            while (a > 0 || b > 0)
            {
                switch (action[a, b])
                {
                    case LastOp.GapV:
                        vRes.Append('-');
                        wRes.Append(w[b - 1]);
                        --b;
                        break;
                    case LastOp.GapW:
                        vRes.Append(v[a - 1]);
                        wRes.Append('-');
                        --a;
                        break;
                    case LastOp.Change:
                        vRes.Append(v[a - 1]);
                        wRes.Append(w[b - 1]);
                        --a; --b;
                        break;
                }
            }
            var alignedV = vRes.ToString().ToCharArray();
            Array.Reverse(alignedV);
            var alignedW = wRes.ToString().ToCharArray();
            Array.Reverse(alignedW);
            alignment.VStart = 0;
            alignment.WStart = 0;
            alignment.VEnd = v.Length;
            alignment.WEnd = w.Length;
            alignment.AlignedV = new string(alignedV);
            alignment.AlignedW = new string(alignedW);
            //-----------------------------------------
            //Console.WriteLine();
            //Console.WriteLine("Alignment with cost: {0}", matrix[v.Length, w.Length]);
            //Console.WriteLine("Word v: {0}", alignment.AlignedV);
            //Console.WriteLine("Word w: {0}", alignment.AlignedW);
            //-----------------------------------------
            return matrix[v.Length, w.Length];
        }
        private static int Maximum(int a, int b, int c, out LastOp op)
        {
            if (a >= b && a >= c)
            {
                op = LastOp.GapW;
                return a;
            }
            if (b >= a && b >= c)
            {
                op = LastOp.GapV;
                return b;
            }
            op = LastOp.Change;
            return c;
        }
        /// <summary>
        /// Funkcja znajdująca najlepsze dopasowanie podciągów.
        /// </summary>
        /// <param name="v">pierwszy ciąg wejściowy</param>
        /// <param name="w">drugi ciąg wejściowy</param>
        /// <param name="alignment">obiekt opisujący najlepsze dopasowanie podciągów.
        /// Uwaga, w wersji z podciągami ustaw:
        /// alignment.VStart = indeks pierwszego elementu optymalnego podciągu v, który dopasowywaliśmy
        /// alignment.WStart = indeks pierwszego elementu optymalnego podciągu w, który dopasowywaliśmy
        /// alignment.VEnd = indeks pierwszego elementu za optymalnym podciągiem v, który dopasowywaliśmy
        /// alignment.WEnd = indeks pierwszego elementu za optymalnym podciągiem w, który dopasowywaliśmy
        /// </param>
        /// <returns>wartość najlepszego dopasowania</returns>
        public int FindSubsequenceAlignment(string v, string w, out Alignment alignment)
        {
            alignment = new Alignment();
            int bestV = 0, bestW = 0, best = 0;
            LastOp[,] action = new LastOp[v.Length + 1, w.Length + 1];
            int[,] matrix = new int[v.Length + 1, w.Length + 1];
            for (int i = 0; i <= v.Length; i++)
            {
                matrix[i, 0] = 0;
                action[i, 0] = LastOp.Skip;
            }
            for (int j = 0; j <= w.Length; j++)
            {
                matrix[0, j] = 0;
                action[0, j] = LastOp.Skip;
            }

            for (int i = 1; i <= v.Length; i++)
                for (int j = 1; j <= w.Length; j++)
                {
                    var max = Maximum(
                                matrix[i - 1, j] + Epsilon,
                                matrix[i, j - 1] + Epsilon,
                                matrix[i - 1, j - 1] + Matrix[v[i - 1], w[j - 1]],
                                out action[i, j]
                                );
                    if (max > 0)
                    {
                        matrix[i, j] = max;
                        if (max > best)
                        {
                            best = max;
                            bestV = i;
                            bestW = j;
                        }
                    }
                    else
                    {
                        action[i, j] = LastOp.Skip;
                        matrix[i, j] = 0;
                    }
                }

            int a = bestV, b = bestW;
            StringBuilder vRes = new StringBuilder();
            StringBuilder wRes = new StringBuilder();
            while (action[a, b] != LastOp.Skip)
            {
                switch (action[a, b])
                {
                    case LastOp.GapV:
                        vRes.Append('-');
                        wRes.Append(w[b - 1]);
                        b--;
                        break;
                    case LastOp.GapW:
                        vRes.Append(v[a - 1]);
                        wRes.Append('-');
                        a--;
                        break;
                    case LastOp.Change:
                        vRes.Append(v[a - 1]);
                        wRes.Append(w[b - 1]);
                        a--; b--;
                        break;
                }
            }
            var alignedV = vRes.ToString().ToCharArray();
            Array.Reverse(alignedV);
            var alignedW = wRes.ToString().ToCharArray();
            Array.Reverse(alignedW);
            alignment.VStart = a;
            alignment.WStart = b;
            alignment.VEnd = bestV;
            alignment.WEnd = bestW;
            alignment.AlignedV = new string(alignedV);
            alignment.AlignedW = new string(alignedW);
            //-----------------------------------------
            //Console.WriteLine();
            //Console.WriteLine("Alignment with cost: {0}", matrix[v.Length, w.Length]);
            //Console.WriteLine("Word v: {0}", alignment.AlignedV);
            //Console.WriteLine("Word w: {0}", alignment.AlignedW);
            //-----------------------------------------
            return matrix[bestV, bestW];
        }

    }

}
