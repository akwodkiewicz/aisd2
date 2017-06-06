using ASD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab14
{
    class AlignmentTestCase : TestCase
    {
        private AlignmentFinder af;
        private int result;
        private int expectedResult;
        string v, w;
        private Alignment al;
        bool subsequence;


        private bool VerifyAlignment(string x, string ax, int start, int end)
        {
            string subX = x.Substring(start, end - start);
            string croppedAX = ax.Replace("-", "");
            return croppedAX.Equals(subX);
        }

        private int ComputeScore()
        {
            int score = 0;
            int n = al.AlignedV.Length;

            for (int i = 0; i < n; ++i)
            {
                if (al.AlignedV[i] != '-' && al.AlignedW[i] != '-')
                    score += af.Matrix[al.AlignedV[i], al.AlignedW[i]];
                else if (al.AlignedV[i] == '-' && al.AlignedW[i] == '-')
                    throw new ArgumentException("aligned gaps found");
                else
                    score += af.Epsilon;
            }
            return score;
        }


        public AlignmentTestCase(double timeLimit, string v, string w, AlignmentFinder af, int expectedResult, bool subsequence = false) : base(timeLimit, null)
        {
            this.expectedResult = expectedResult;
            this.v = v;
            this.w = w;
            this.af = af;
            this.subsequence = subsequence;
        }

        public override void PerformTestCase()
        {
            if (!subsequence)
                result = af.FindAlignment(v, w, out al);
            else
                result = af.FindSubsequenceAlignment(v, w, out al);
        }

        public override void VerifyTestCase(out Result resultCode, out string message)
        {
            if (result != expectedResult)
            {
                resultCode = Result.BadResult;
                message = $"incorrect result: {result} (expected: {expectedResult})";
                return;
            }
            message = "score OK, ";
            if (al == null)
            {
                resultCode = Result.BadResult;
                message += "null alignment returned";
                return;
            }
            if (al.AlignedV == null)
            {
                resultCode = Result.BadResult;
                message += "null alignment of v returned";
                return;
            }
            if (al.AlignedW == null)
            {
                resultCode = Result.BadResult;
                message += "null alignment of w returned";
                return;
            }
            if (!subsequence)
            {
                if (al.VStart != 0 || al.WStart != 0 || al.VEnd != v.Length || al.WEnd != w.Length)
                {
                    resultCode = Result.BadResult;
                    message += "subsequence returned";
                    return;
                }
            }

            if (!VerifyAlignment(v, al.AlignedV, al.VStart, al.VEnd))
            {
                resultCode = Result.BadResult;
                message += "returned substring is not a proper subsequence of v";
                return;
            }
            if (!VerifyAlignment(w, al.AlignedW, al.WStart, al.WEnd))
            {
                resultCode = Result.BadResult;
                message += "returned substring is not a proper subsequence of w";
                return;
            }
            if (al.AlignedV.Length != al.AlignedW.Length)
            {
                resultCode = Result.BadResult;
                message += "returned alignments of different lengths";
                return;
            }

            try
            {
                int sc = ComputeScore();
                if (sc == expectedResult)
                {
                    message += "alignment OK";
                    resultCode = Result.Success;
                }
                else
                {
                    message += $"returned alignment with score {sc}";
                    resultCode = Result.BadResult;
                }

            }
            catch (Exception ex)
            {
                resultCode = Result.BadResult;
                message += ex.Message;
                return;
            }
        }
    }


    class Program
    {

        const string PheA = "MVNSSKSILIHAQNKNGTHEEEQYLFAVNNTKAEYPRDKTIHQLFEEQVSKRPNNVAIVCENEQLTYHELNVKANQLARIFIEKGIGKDTLVGIMMEKSIDLFIGILAVLKAGGAYVPIDIEYPKERIQYILDDSQARMLLTQKHLVHLIHNIQFNGQVEIFEEDTIKIREGTNLHVPSKSTDLAYVIYTSGTTGNPKGTMLEHKGISNLKVFFENSLNVTEKDRIGQFASISFDASVWEMFMALLTGASLYIILKDTINDFVKFEQYINQKEITVITLPPTYVVHLDPERILSIQTLITAGSATSPSLVNKWKEKVTYINAYGPTETTICATTWVATKETIGHSVPIGAPIQNTQIYIVDENLQLKSVGEAGELCIGGEGLARGYWKRPELTSQKFVDNPFVPGEKLYKTGDQARWLSDGNIEYLGRIDNQVKIRGHRVELEEVESILLKHMYISETAVSVHKDHQEQPYLCAYFVSEKHIPLEQLRQFSSEELPTYMIPSYFIQLDKMPLTSNGKIDRKQLPEPDLTFGMRVDYEAPRNEIEETLVTIWQDVLGSHHH";
        const string Dhv = "AKLLEQIEKWAAETPDQTAFVWRDAKITYKQLKEDSDALAHWISSEYPDDRSPIMVYGHMQPEMIINFLGCVKAGHAYIPVDLSIPADRVQRIAENSGAKLLLSATAVTVTDLPVRIVSEDNLKDIFFTHKGNTPNPEHAVKGDENFYIIYTSGSTGNPKGVQITYNCLVSFTKWAVEDFNLQTGQVFLNQAPFSFDLSVMDIYPSLVTGGTLWAIDKDMIARPKDLFASLEQSDIQVWTSTPSFAEMCLMEASFSESMLPNMKTFLFCGEVLPNEVARKLIERFPKATIMNTYGPTEATVAVTGIHVTEEVLDQYKSLPVGYCKSDCRLLIMKEDGTIAPDGEKGEIVIVGPSVSVGYLGSPELTEKAFTMIDGERAYKTGDAGYVENGLLFYNGRLDFQIKLHGYRMELEEIEHHLRACSYVEGAVIVPIKKGEKYDYLLAVVVPGEHSFEKEFKLTSAIKKELNERLPNYMIPRKFMYQSSIPMTPNGKVDRKKLLSEVTALEHHHHHH";
        const string Yp5 = "MAKLALSAAQHGIWLGQQLDPSSPLYNTAEYVALRGAVELTNLTAAIKQAFAEAATLHLRFGLEHDQPYALVEPQPINLTVHDLRDLPDAEVRAIAWMQHDLGNVVDLATGPLFNTAILQLADDQVWWYLRAHHIALDGYSFALLTKRVAEIYSALQTKATLSPSFGELVPVIAEDHAYQVSIQATLDREFWVNRFADNPQVVSLTQQTALSQPRSIRLSTALANDLIERLTAIAKPSRSTWPDALMAVVAAYLARWNNSESVVLGMPLMSRLGSVALRVPCMAMNIVPLCLNVAAEHDLAQLTAVVAAERNAFRKHGRYRYEQLRRDLGFVGAGRRLFGPVVNIMPFDHPLNFGDCQAQSTTLTAGPVEDLAFNVILRGNQLYLTLEANPACYSQAALEYHFAAIQHLLNGWLANPSIPVAEQQVLPAPLVLDGGELRLPLTSVIERILHNARQQPHALALVTDTEQLSYAELASHVHAWAGQLVQRGVTAGSVVGVALPRSREAIVAILATLCCGAAYLPLDPQWPQSRLASVVAQAQPVLVLAQQAFDLPNLLLVEQLSKANAWFEARVDLAQPAYIMYTSGSTGEPKGVVISHQALAGFVQAAAERYAISAADRVLQFAPLAFDASVEEIFVTLCQGATLVLRNDAMLESLQRFVAACQAHAISVLDLPTAFWHELADSVAQGAVQLPECLRVVIIGGEAALPERVQGWLNVVAPNVRLFNTYGPTEATVVATVAELSDPNQPITIGRPLAGVQAAILGSDQRPIFAGDVGDLYLLGNGLATGYYQRPDLDALNFGQLSQLPHAPRAYRTGDRVRLFAGQLQFVGRSDDEFKISGQRVTPAEIESVFLRHTAVREVAVIGQQLGNASKRLFAAVVVSDASLSVAELRNHASQHLPAAVIPAAITIVERLPRSSAGKIDRKAVAALAPAPVMVNAAINDTPALIRQVWAEVLGQTEFNDEADFFGLGGQSLQTIQVANRLGMALGREVTAALIFRYPTIAGLSQALDPEFEQAPEAAPQFLSDANLPEQIVPKQLNAQPRPIQTVLLTGATGFVGAHLLAELLSTTTTNVICVVRAGSNAAAFERLQASLQHYELPSEQLAEQVEAWQGDLAQPQFGLDDQQWQSLIERCDLIYHNAAMVSVVREYSSLRAVNVNATSEILRLAAVHCTPVHYVSTLAVSPPQSVMHRVPEDFVAAHAGLRDGYSQSKWVAERLLEQAATRGLPVAVYRLGRVVGPNQSNFVNQDDLFWRIVQAGVPRGLLPSLPVEEIWNPVDFAAQTIVQFSHNHRGVRVYNLAPNEPISFAQLLGWVGEYGYAVQLCRVEQWYQALRNADDAMSQATLTFFERQADGGELPSAIGTIENKRLLQTLAAHGIAVPVIDRERFFGYLERCIRTGLLPAPDLRQTSIGIR";
        const string Yp0 = "MSTDGTRDLTARRRELLRRRMADENLLHTSDPEPETAAPAGERPLSPGQQRMWSIQQLDPLTVGYNVTIALDVSGELDADLLGQALELVVARHDILRTVYRLTDDGTGDGQSRVVQVVQDQFTAPYDVCDVRDLDEAARAARVGELARAVAGQPFDLATDPPLRVRLIRTGEASSTLVVVAHHIVWDDGTSAVFFGQLMDDYRRLLSGERVVARRSPRQYADVALGSHGAAGVDDSALAHWRDELTPLPELLDLPGISGGAGGAGQERSHPMREGTGRRVREIARREGASTFMVLFAAVSALLHRYTGAREFLVGAPVVNRDFPGAEDVVGYLGNTIPLRAEVDPSDDFRTLLARSRATCVDAYAHQHVELDDIAKAADAQRFRGDAGLFNVVLSLRSPVLEPFRAAGLQVGRRHVPGTDARFDLTLAVETDGDELAVEANYPARHDADDQVRGLLEHLDRLLDAALADPATPVGELELLAPGERERLLHECNDTATRTDERLLPERFKAQVALTPDALAVTAPGEEAGTELTYAELNARANRLARELVGRGIGPEHVVALAVPRSPAMMVAALAVLKAGAAYVPVDPSYPAERVRLMLADSSPALLLATCTVAAELPDGGVRRLLLDDPEVAEQVAALPGTDLADSDRNAPLLPGHPAYVIYTSGSTGTPKGVVVAHRALSNHLDWAVRRFAGLGGRTLLHSSMSFDFSVTPMYGPLLCGGVLELCEDSPDAIANATGPATFLKITPSHLPLLPSVRFAAEGPRTLVIAGESLHGESLVDRQPPEGEGLDVINEYGPTETTVGCTLHDIPFADGAPAGPVPIGRPVANTRCHVLDQALRPVPAGVPGELYIGGSQLARGYLGRPGLTASRFVADPFAGTGERLYRTGDRVRRRADGALEFVGRVDEQVKIRGHRVEPGEVEAVLLRHPAVAQAVVVGRSDGPSGTYLAAYVVLNDSGQSVVDGAALREQVAAQLPEHMVPGVVVVLGELPLSPSGKADRRALPAPEFTPDAPTSREPSNEVERTLVRLFSEVLHRDDVGVTDSFHALGGDSIVTIQLVARARKAGLRIRPRDVFEHRTAEALAAALPEADAAQTEAAPDDDPVGPVEPTPIVRSFAERGPLGDRHRMSVLLDVPALERGSLVRAVQAVLDTHDALRARSSAGTPGLEVRPRGAVQADSVVRVVAAPRGATPEEVEHEIESAAARLDPRSGVMVQVVWFDAGEQRPGRLLLVVHHSVVDGVSLRILTEDLATAWRAVDAGTDPALPAVGTSLRSWSRGLVRQAPARAHELSHWRSVLAEDSAPGERGVDPARDTWATVRTVTVELDDATTDAVLTTVPQAFFGAVDDVLLAGFGLAVAAWRRDRGEDARSPLVLVEGHGREESAVPGADLSRTVGWFTSQYPVRLGLADIDPDEALAGGPAAGTAVKRVKETLRSVPDHGIGYGMLRYLDPASSGELAAMPEPEFGFNYLGRMDTGSGDWSIAPGGVSAAYDPDMPVAVPLVVNAVTEVGPGGPRLTAHWMYAGNLLADAEVRDLARRWSEALDALVRHAAGPGAGGRTPSDLSLVSLDQAQLDALEAKWKKP";

        private static void DebugTest(string v, string w, AlignmentFinder af, bool subsequence)
        {
            Console.WriteLine("------------------------");
            Console.WriteLine($"v={v}");
            Console.WriteLine($"w={w}");
            Alignment al;
            int sc;
            if (!subsequence)
                sc = af.FindAlignment(v, w, out al);
            else
                sc = af.FindSubsequenceAlignment(v, w, out al);

            Console.WriteLine($"av={al.AlignedV}");
            Console.WriteLine($"aw={al.AlignedW}");
            Console.WriteLine($"returned={sc}");

            int score = 0;
            int n = al.AlignedV.Length;

            for (int i = 0; i < n; ++i)
            {
                if (al.AlignedV[i] != '-' && al.AlignedW[i] != '-')
                    score += af.Matrix[al.AlignedV[i], al.AlignedW[i]];
                else if (al.AlignedV[i] == '-' && al.AlignedW[i] == '-')
                    throw new ArgumentException("aligned gaps found");
                else
                    score += af.Epsilon;
            }


            Console.WriteLine($"computed={score}");
        }


        static void Main(string[] args)
        {
            AlignmentFinder af1 = new AlignmentFinder();
            AlignmentFinder af2 = new AlignmentFinder(ScoringMatrix.PAM250, -5);

            TestSet unweightedTests = new TestSet();
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "AGT", "AGT", af1, 0));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "CCCP", "AAAACCCPAAAAAAAAAA", af1, -14));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "AAACAC", "CACA", af1, -3));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "AGGCTACTTTCA", "GGCTACTATATCA", af1, -3));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "CCGCCT", "AGTGGGGTAAGA", af1, -10));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "AAAAAAAAAAAAAAAAAAAA", "AAGAAGAAGAAGAGAA", af1, -9));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "AAAAWWW", "WWWAAAA", af1, -6));
            unweightedTests.TestCases.Add(new AlignmentTestCase(1, "WWSRWWW", "EWWW", af1, -4));
            unweightedTests.TestCases.Add(new AlignmentTestCase(2, PheA, Dhv, af1, -397));
            unweightedTests.TestCases.Add(new AlignmentTestCase(3, Yp0, Yp5, af1, -1137));

            TestSet weightedTests = new TestSet();
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "AGT", "AGT", af2, 10));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "CCCP", "AAAACCCPAAAAAAAAAA", af2, -28));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "AAACAC", "CACA", af2, 6));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "AGGCTACTTTCA", "GGCTACTATATCA", af2, 47));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "CCGCCT", "AGTGGGGTAAGA", af2, -32));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "AAAAAAAAAAAAAAAAAAAA", "AAGAAGAAGAAGAGAA", af2, 7));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "AAAAWWW", "WWWAAAA", af2, 11));
            weightedTests.TestCases.Add(new AlignmentTestCase(1, "WWSRWWW", "EWWW", af2, 36));
            weightedTests.TestCases.Add(new AlignmentTestCase(2, PheA, Dhv, af2, 464));
            weightedTests.TestCases.Add(new AlignmentTestCase(5, Yp0, Yp5, af2, 1062));


            TestSet subsequenceTests = new TestSet();
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "AGT", "AGT", af2, 10, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "CCCP", "AAAACCCPAAAAAAAAAA", af2, 42, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "AAACAC", "CACA", af2, 26, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "AGGCTACTTTCA", "CTAACT", af2, 30, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "AGGCTACTTTCA", "CTCT", af2, 25, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "AGGCTCTTTCA", "CTACT", af2, 25, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "AAAAWWW", "WWWAAAA", af2, 51, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(1, "WSRWW", "EWWW", af2, 41, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(2, PheA, Dhv, af2, 658, true));
            subsequenceTests.TestCases.Add(new AlignmentTestCase(5, Yp0, Yp5, af2, 1344, true));


            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            Console.WriteLine("Część 1: bez wag dopasowań");
            stopwatch.Start();
            unweightedTests.PreformTests(true, false);
            stopwatch.Stop();
            Console.WriteLine("It took {0}", stopwatch.ElapsedMilliseconds);


            Console.WriteLine("Część 2: z wagami dopasowań");
            stopwatch.Restart();
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            weightedTests.PreformTests(true, false);
            stopwatch.Stop();
            Console.WriteLine("It took {0} on average!", stopwatch.ElapsedMilliseconds/10);

            Console.WriteLine("Część 3: dopasowanie podciągów");
            stopwatch.Restart();
            subsequenceTests.PreformTests(true, false);
            stopwatch.Stop();
            Console.WriteLine("It took {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}
