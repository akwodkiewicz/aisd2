using ASD;
using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab6;
using TestPlatform;

namespace Pathfinder
{
    class Program
    {
        static void Main(string[] args)
        {
            TestParameters param = new TestParameters();
            param.ShowPositiveTests = false;                        //Czy wyświetlać pozytywne wyniki
            param.ShowNegativeTests = true;                         //Czy wyświetlać negatywne wyniki
            param.ShowTimeoutedTests = true;                        //Czy wyświetlać komunikaty o upływie czasu
            param.ShowThrownExepctions = true;                      //Czy wyświetlać komunikaty o wyrzuconych wyjątkach
            param.CatchExeption = false;                            //Czy łapać wyjątki
            param.WorkOnCopy = false;                               //Czy testować funkcję na kopii argumentów
            param.TimeoutDelay = TimeSpan.FromMilliseconds(20000);   //Czas na pojedyńcze zapytanie

            TestLab6Task1 Test1 = new TestLab6Task1(param);
            Test1.FuncitonToTest = (inq) =>
            {
                Edge[] path;
                return new AnswerLab6Task1(inq.Graph.FindSecondShortestPath(inq.StartingPoint, inq.EndingPoint, out path), path);
            };

            TestLab6Task2 Test2 = new TestLab6Task2(param);
            Test2.FuncitonToTest = (inq) =>
            {
                Edge[] path;
                return new AnswerLab6Task2(inq.Graph.FindSecondSimpleShortestPath(inq.StartingPoint, inq.EndingPoint, out path), path);
            };

            Test1.LoadCases("CasesTask1");
            Test2.LoadCases("CasesTask2");

            Test1.PerformTest();
            Test2.PerformTest();
        }
    }
}


