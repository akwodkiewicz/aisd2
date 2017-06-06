using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab14
{
    public class ScoringMatrix: Dictionary<Tuple<char,char>,int>
    {        
        private static char[] characters = new char[] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };
        public static ScoringMatrix PAM250
        {
            get
            {
                string[] pam250 = new string[]    {"A  C  D  E  F  G  H  I  K  L  M  N  P  Q  R  S  T  V  W  Y",
                                                "A  2 -2  0  0 -3  1 -1 -1 -1 -2 -1  0  1  0 -2  1  1  0 -6 -3",
                                                "C -2 12 -5 -5 -4 -3 -3 -2 -5 -6 -5 -4 -3 -5 -4  0 -2 -2 -8  0",
                                                "D  0 -5  4  3 -6  1  1 -2  0 -4 -3  2 -1  2 -1  0  0 -2 -7 -4",
                                                "E  0 -5  3  4 -5  0  1 -2  0 -3 -2  1 -1  2 -1  0  0 -2 -7 -4",
                                                "F -3 -4 -6 -5  9 -5 -2  1 -5  2  0 -3 -5 -5 -4 -3 -3 -1  0  7",
                                                "G  1 -3  1  0 -5  5 -2 -3 -2 -4 -3  0  0 -1 -3  1  0 -1 -7 -5",
                                                "H -1 -3  1  1 -2 -2  6 -2  0 -2 -2  2  0  3  2 -1 -1 -2 -3  0",
                                                "I -1 -2 -2 -2  1 -3 -2  5 -2  2  2 -2 -2 -2 -2 -1  0  4 -5 -1",
                                                "K -1 -5  0  0 -5 -2  0 -2  5 -3  0  1 -1  1  3  0  0 -2 -3 -4",
                                                "L -2 -6 -4 -3  2 -4 -2  2 -3  6  4 -3 -3 -2 -3 -3 -2  2 -2 -1",
                                                "M -1 -5 -3 -2  0 -3 -2  2  0  4  6 -2 -2 -1  0 -2 -1  2 -4 -2",
                                                "N  0 -4  2  1 -3  0  2 -2  1 -3 -2  2  0  1  0  1  0 -2 -4 -2",
                                                "P  1 -3 -1 -1 -5  0  0 -2 -1 -3 -2  0  6  0  0  1  0 -1 -6 -5",
                                                "Q  0 -5  2  2 -5 -1  3 -2  1 -2 -1  1  0  4  1 -1 -1 -2 -5 -4",
                                                "R -2 -4 -1 -1 -4 -3  2 -2  3 -3  0  0  0  1  6  0 -1 -2  2 -4",
                                                "S  1  0  0  0 -3  1 -1 -1  0 -3 -2  1  1 -1  0  2  1 -1 -2 -3",
                                                "T  1 -2  0  0 -3  0 -1  0  0 -2 -1  0  0 -1 -1  1  3  0 -5 -3",
                                                "V  0 -2 -2 -2 -1 -1 -2  4 -2  2  2 -2 -1 -2 -2 -1  0  4 -6 -2",
                                                "W -6 -8 -7 -7  0 -7 -3 -5 -3 -2 -4 -4 -6 -5  2 -2 -5 -6 17  0",
                                                "Y -3  0 -4 -4  7 -5  0 -1 -4 -1 -2 -2 -5 -4 -4 -3 -3 -2  0 10"};

                ScoringMatrix m = new ScoringMatrix();                                                                             
                                
                for (int i = 0; i < characters.Length; ++i)
                {                   
                    string[] tokens = pam250[i+1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < characters.Length; ++j)
                        m.Add(new Tuple<char, char>(characters[i], characters[j]), int.Parse(tokens[j + 1]));                    
               }
                return m;
            }
        }

        public static ScoringMatrix Simple
        {
            get
            {                
                ScoringMatrix m = new ScoringMatrix();
                for (int i = 0; i < characters.Length; ++i)
                    for (int j = 0; j < characters.Length; ++j)
                    {
                        int w = (i == j) ? 0 : -1;
                        m.Add(new Tuple<char, char>(characters[i], characters[j]), w);
                }
                return m;
            }
        }

        public int this[char c1, char c2]
        {
            get
            {
                if (!(characters.Contains(c1) && characters.Contains(c2)))
                    throw new ArgumentException("wrong amino acid symbol");
                return this[new Tuple<char, char>(c1, c2)];
            }
        }
    } 
}
