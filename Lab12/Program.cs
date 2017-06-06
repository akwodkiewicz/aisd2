using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;  // dodac w projekcie referencje System.Runtime.Serialization i System.XML

namespace ASD
{

    class Program
    {

        [Serializable]
        class ClosestPointsTestCase : TestCase
        {
            private double Eps = 0.00000001;

            private Tuple<Point, Point> resultP;
            private double resultD;
            private Tuple<Point, Point> properResultP;
            private double properResultD;
            private bool brute;

            private List<Point> points;

            public ClosestPointsTestCase(double timeLimit, List<Point> points, Tuple<Point, Point> properResultP, double properResultD, bool brute=false) : base(timeLimit, null)
            {
                this.properResultP = properResultP;
                this.properResultD = properResultD;
                this.points = points;
                this.brute = brute;
            }

            public override void PerformTestCase()
            {
                resultP = brute ? SweepClosestPair.FindClosestPointsBrute(points, out resultD) : SweepClosestPair.FindClosestPoints(points, out resultD) ;
            }

            public override void VerifyTestCase(out Result resultCode, out string message)
            {
                if( Math.Abs(resultD-properResultD)>Eps )
                {
                    resultCode = Result.BadResult;
                    message = string.Format("Incorrect distance: {0} (expected: {1})", resultD, properResultD);
                    return;
                }

                if ((properResultP.Item1 == resultP.Item1 && properResultP.Item2 == resultP.Item2)
                    || (properResultP.Item1 == resultP.Item2 && properResultP.Item2 == resultP.Item1))
                {
                    resultCode = Result.Success;
                    message = "OK";
                }
                else
                {
                    resultCode = Result.BadResult;
                    message = string.Format("Incorrect points: {0} and {1} (expected: {2} and {3})", resultP.Item1, resultP.Item2, properResultP.Item1, properResultP.Item2);
                }
            }
        }

        private class PointsWithResult
        {
            public List<Point> points;
            public double minDistance;
            public Tuple<Point, Point> pair;

            public PointsWithResult(List<Point> points, Tuple<Point, Point> pair, double minDistance)
            {
                this.points = points;
                this.minDistance = minDistance;
                this.pair = pair;
            }
        }

        static void Main(string[] args)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(TestSet), new Type[] { typeof(ClosestPointsTestCase) } );
            FileStream fs;

            Console.WriteLine("Brute force algorithm tests:");
            fs = new FileStream("STB.dat", FileMode.Open);
            TestSet simpleTestsBrute = (TestSet)dcs.ReadObject(fs);
            fs.Close();
            simpleTestsBrute.PreformTests(true, false);

            Console.WriteLine("Sweep line algorithm tests:");
            fs = new FileStream("ST.dat", FileMode.Open);
            TestSet simpleTests = (TestSet)dcs.ReadObject(fs);
            fs.Close();
            simpleTests.PreformTests(true, false);

            Console.WriteLine("Sweep line algorithm performance tests:");
            fs = new FileStream("PT.dat", FileMode.Open);
            TestSet performanceTests = (TestSet)dcs.ReadObject(fs);
            fs.Close();
            performanceTests.PreformTests(true, true);

            return;
        }

    }

}
