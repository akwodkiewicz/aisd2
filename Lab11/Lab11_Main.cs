using System;

namespace ASD
{
    #region Types

    public class SmurfphoneFactoryTestCase : TestCase
    {
        private readonly double predictedCost;
        private readonly int predictedAmount;
        private readonly int maximumAmount;


        private double resultAmount;
        private int[,] resultTransports;
        private double resultCost;

        private readonly Provider[] providers;
        private readonly Factory[] factories;
        private readonly double distanceCostMultiplier;
        private readonly bool possible;

        public SmurfphoneFactoryTestCase(double predictedCost, int predictedAmount, double timeLimit, Provider[] providers, Factory[] factories, double distanceCostMultiplier, int maximumAmount = int.MaxValue, bool possible = false) : base(timeLimit, null)
        {
            this.predictedCost = predictedCost;
            this.predictedAmount = predictedAmount;
            this.maximumAmount = maximumAmount;
            this.providers = providers;
            this.factories = factories;
            this.distanceCostMultiplier = distanceCostMultiplier;
            this.possible = possible;
        }

        public override void PerformTestCase()
        {
            resultAmount = SmurfphoneFactory.CalculateFlow(providers, factories, distanceCostMultiplier, out resultCost, out resultTransports, maximumAmount);
        }

        public override void VerifyTestCase(out Result resultCode, out string message)
        {
            var testResult = ConfirmTestResult();
            var testAmount = Math.Abs(resultAmount - predictedAmount) < double.Epsilon;
            var testCappedAmount = Math.Abs(resultAmount - maximumAmount) < double.Epsilon;
            var testCost = Math.Abs(predictedCost - resultCost) < double.Epsilon;

            if ((maximumAmount < int.MaxValue && (possible && testCappedAmount || !possible && testAmount) || testAmount) && testCost && testResult == true)
            {
                resultCode = Result.Success;
                message = "OK";
                return;
            }

            string msg = "";
            if (Math.Abs(resultCost - predictedCost) > double.Epsilon)
            {
                msg += $"Bad cost result, is: {resultCost}, should be:{predictedCost} ";
            }
            if (Math.Abs(resultAmount - predictedAmount) > double.Epsilon)
            {
                msg += $"Bad producedAmount result, is: {resultAmount}, should be:{predictedAmount} ";
            }
            if (testResult == null)
            {
                msg += "Transport table is missing ";
            }
            else if (testResult == false)
            {
                msg += "Transport table has wrong values";
            }
            resultCode = Result.BadResult;
            message = msg;
        }


        private bool? ConfirmTestResult()
        {
            if (resultTransports == null)
                return null;

            int sum = 0;
            foreach (var transport in resultTransports)
            {
                sum += transport;
            }
            if (Math.Abs(resultAmount - sum) > double.Epsilon)
                return false;

            double cost = 0;

            //koszty kupienia surowców
            for (int i = 0; i < resultTransports.GetLength(0); i++)
            {
                int providerSum = 0;
                for (int j = 0; j < resultTransports.GetLength(1); j++)
                {
                    providerSum += resultTransports[i, j];
                }

                if (providerSum > providers[i].Capacity)
                    return false;
                cost += providers[i].Cost * providerSum;
            }

            // koszty produkcji w fabrykach
            for (int i = 0; i < resultTransports.GetLength(1); i++)
            {
                int factorySum = 0;
                for (int j = 0; j < resultTransports.GetLength(0); j++)
                {
                    factorySum += resultTransports[j, i];
                }

                cost += factories[i].LowerCost * factorySum;
                if (factorySum > factories[i].Limit)
                    cost += (factorySum - factories[i].Limit) * (factories[i].HigherCost - factories[i].LowerCost);
            }

            // koszty dostawy
            double distance = 0;
            for (int i = 0; i < resultTransports.GetLength(0); i++)
                for (int j = 0; j < resultTransports.GetLength(1); j++)
                {
                    var xDiv = providers[i].Position.X - factories[j].Position.X;
                    var yDiv = providers[i].Position.Y - factories[j].Position.Y;
                    distance += Math.Ceiling(Math.Sqrt(xDiv * xDiv + yDiv * yDiv) * distanceCostMultiplier) * resultTransports[i, j];
                }
            cost += distance;

            return !(Math.Abs(resultCost - cost) > double.Epsilon);
        }
    }

    #endregion

    class Program
    {
        #region TestCaseCreation

        #region Facilities

        private static Tuple<Provider[], Factory[]> CreateFacilities1(bool distance, bool limits)
        {
            var providers = new[]
            {
                new Provider(0, 0.0, distance? new Vector2(0,1) : Vector2.Zero),
                new Provider(0, 0.0, distance? new Vector2(0,2) : Vector2.Zero),
                new Provider(0, 0.0, distance? new Vector2(0,3) : Vector2.Zero),
                new Provider(5, 13.0, distance? new Vector2(0,4) : Vector2.Zero),
            };

            var factories = new[]
            {
                new Factory(1.0, 3.0,limits? 2 : int.MaxValue, distance? new Vector2(2,1) : Vector2.Zero),
                new Factory(1.5, 2.0, limits? 10 : int.MaxValue, distance? new Vector2(2,4) : Vector2.Zero),
            };

            return new Tuple<Provider[], Factory[]>(providers, factories);
        }

        private static Tuple<Provider[], Factory[]> CreateFacilities2(bool distance, bool limits)
        {
            var providers = new[]
            {
                new Provider(10, 2.0, Vector2.Zero),
            };

            var factories = new[]
            {
                new Factory(1.0, 4.0,limits? 2 : int.MaxValue, distance? new Vector2(2,1) : Vector2.Zero),
                new Factory(2.0, 3.0, limits? 8 : int.MaxValue, distance? new Vector2(2,-1) : Vector2.Zero),
            };

            return new Tuple<Provider[], Factory[]>(providers, factories);
        }

        private static Tuple<Provider[], Factory[]> CreateFacilities3(bool distance, bool limits)
        {
            var providers = new[]
            {
                new Provider(10, 4.0, Vector2.Zero),
                new Provider(6, 2.5, Vector2.Zero),
            };

            var factories = new[]
            {
                new Factory(2.0, 8.0,limits? 5 : int.MaxValue, distance? new Vector2(2,5) : Vector2.Zero),
            };

            return new Tuple<Provider[], Factory[]>(providers, factories);
        }

        private static Tuple<Provider[], Factory[]> CreateFacilities4(bool distance, bool limits)
        {
            var providers = new[]
            {
                new Provider(20, 4.0, distance? new Vector2(4,1) : Vector2.Zero),
                new Provider(10, 2.5, distance? new Vector2(2,7) : Vector2.Zero),
            };

            var factories = new[]
            {
                new Factory(5.0, 15.0,limits? 5 : int.MaxValue, distance? new Vector2(2,5) : Vector2.Zero),
                new Factory(5.0, 7.0,limits? 10 : int.MaxValue, distance? new Vector2(2,1) : Vector2.Zero),
            };

            return new Tuple<Provider[], Factory[]>(providers, factories);
        }

        private static Tuple<Provider[], Factory[]> CreateFacilities5(bool distance, bool limits)
        {
            var providers = new[]
            {
                new Provider(20, 4.0, distance? new Vector2(4,1) : Vector2.Zero),
                new Provider(10, 2.5, distance? new Vector2(2,7) : Vector2.Zero),
            };

            var factories = new[]
            {
                new Factory(3.0, 3.5, limits? 5 : int.MaxValue, distance? new Vector2(2,20) : Vector2.Zero),
                new Factory(5.0, 7.0, limits? 10 : int.MaxValue, distance? new Vector2(2,1) : Vector2.Zero),
            };

            return new Tuple<Provider[], Factory[]>(providers, factories);
        }

        private static Tuple<Provider[], Factory[]> CreateRandomFacilities(bool distance, bool limits, int providerCount,
            int factoriesCount, int seed, int minCapacity = 0, int maxCapacity = 10, int minCost = 1, int maxCost = 10, double minX = 0, double maxX = 10, double minY = 0, double maxY = 10)
        {
            Random rand = new Random(seed);
            var providers = new Provider[providerCount];
            var factories = new Factory[factoriesCount];


            for (int i = 0; i < providers.Length; i++)
            {
                providers[i] = new Provider(rand.Next(minCapacity, maxCapacity), rand.Next(minCost, maxCost));
            }

            for (int i = 0; i < factories.Length; i++)
            {
                int lowerCost = rand.Next(minCost, maxCost);
                int overpaidCost = Math.Max(rand.Next(minCost, maxCost), lowerCost + 1);
                factories[i] = new Factory(lowerCost, overpaidCost, limits ? rand.Next(minCapacity, maxCapacity) : int.MaxValue);
            }

            if (distance)
            {
                double xRange = maxX - minX;
                double yRange = maxY - minY;
                foreach (var provider in providers)
                {
                    provider.Position = new Vector2(minX + rand.NextDouble() * xRange, minY + rand.NextDouble() * yRange);
                }

                foreach (var factory in factories)
                {
                    factory.Position = new Vector2(minX + rand.NextDouble() * xRange, minY + rand.NextDouble() * yRange);
                }
            }

            return new Tuple<Provider[], Factory[]>(providers, factories);
        }

        #endregion

        #region BasicTests

        private static SmurfphoneFactoryTestCase CreateBasicTest1()
        {
            var facilities = CreateFacilities1(false, false);

            return new SmurfphoneFactoryTestCase(70, 5, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest2()
        {
            var facilities = CreateFacilities2(false, false);
            return new SmurfphoneFactoryTestCase(30, 10, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest3()
        {
            var facilities = CreateFacilities3(false, false);
            return new SmurfphoneFactoryTestCase(87, 16, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest4()
        {
            var facilities = CreateFacilities4(false, false);
            return new SmurfphoneFactoryTestCase(255, 30, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest5()
        {
            var facilities = CreateFacilities5(false, false);
            return new SmurfphoneFactoryTestCase(195, 30, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest6()
        {
            var facilities = CreateRandomFacilities(false, false, 5, 5, 121314);
            return new SmurfphoneFactoryTestCase(170, 25, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest7()
        {
            var facilities = CreateRandomFacilities(false, false, 10, 10, 9183264);
            return new SmurfphoneFactoryTestCase(213, 29, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        private static SmurfphoneFactoryTestCase CreateBasicTest8()
        {
            var facilities = CreateRandomFacilities(false, false, 25, 21, 67896);
            return new SmurfphoneFactoryTestCase(741, 113, 1.0, facilities.Item1, facilities.Item2, 0.0);
        }

        #endregion


        #region FullTests

        private static SmurfphoneFactoryTestCase CreateFullTestCase1()
        {
            var providers = new[]
            {
                new Provider(5,2,new Vector2(0,1)),
                new Provider(10,2.5, new Vector2(0,2)),
            };
            var factories = new[]
            {
                new Factory(3, 5, 3, new Vector2(2, 3)),
                new Factory(4, 8, 6, new Vector2(2, 1)),
            };
            return new SmurfphoneFactoryTestCase(138, 15, 1.0, providers, factories, 1.0);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase2()
        {
            var facilities = CreateFacilities1(true, true);
            return new SmurfphoneFactoryTestCase(82.5, 5, 1.0, facilities.Item1, facilities.Item2, 1.0);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase3()
        {
            var facilities = CreateFacilities2(true, true);
            return new SmurfphoneFactoryTestCase(88, 10, 1.0, facilities.Item1, facilities.Item2, 2.0);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase4()
        {
            var facilities = CreateFacilities3(true, true);
            return new SmurfphoneFactoryTestCase(297, 16, 1.0, facilities.Item1, facilities.Item2, 1.5);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase5()
        {
            var facilities = CreateFacilities4(true, true);
            return new SmurfphoneFactoryTestCase(475, 30, 1.0, facilities.Item1, facilities.Item2, 2.4);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase6()
        {
            var facilities = CreateFacilities5(true, true);
            return new SmurfphoneFactoryTestCase(595, 30, 1.0, facilities.Item1, facilities.Item2, 3.0);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase7()
        {
            var facilities = CreateFacilities5(true, true);
            return new SmurfphoneFactoryTestCase(302.5, 30, 1.0, facilities.Item1, facilities.Item2, 0.2);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase8()
        {
            var facilities = CreateFacilities5(true, true);
            return new SmurfphoneFactoryTestCase(267.5, 30, 1.0, facilities.Item1, facilities.Item2, 0.1);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase9()
        {
            var facilities = CreateRandomFacilities(true, true, 10, 20, 12345);
            return new SmurfphoneFactoryTestCase(246, 31, 1.0, facilities.Item1, facilities.Item2, 0.1);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase10()
        {
            var facilities = CreateRandomFacilities(true, true, 15, 15, 123456);
            return new SmurfphoneFactoryTestCase(439, 57, 1.0, facilities.Item1, facilities.Item2, 0.1);
        }

        private static SmurfphoneFactoryTestCase CreateFullTestCase11()
        {
            var facilities = CreateRandomFacilities(true, true, 10, 5, 223344);
            return new SmurfphoneFactoryTestCase(449, 52, 1.0, facilities.Item1, facilities.Item2, 0.1);
        }

        #endregion


        #region CappedProductionTests

        private static SmurfphoneFactoryTestCase CreateCappedTestCase1()
        {
            var facilities = CreateFacilities1(true, true);
            return new SmurfphoneFactoryTestCase(55.5, 3, 1.0, facilities.Item1, facilities.Item2, 2.0, 3, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase2()
        {
            var facilities = CreateFacilities2(true, true);
            return new SmurfphoneFactoryTestCase(76, 6, 1.0, facilities.Item1, facilities.Item2, 4.0, 6, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase3()
        {
            var facilities = CreateFacilities3(true, true);
            return new SmurfphoneFactoryTestCase(108, 7, 1.0, facilities.Item1, facilities.Item2, 1.5, 7, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase4()
        {
            var facilities = CreateFacilities4(true, true);
            return new SmurfphoneFactoryTestCase(157.5, 15, 1.0, facilities.Item1, facilities.Item2, 1.0, 15, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase5()
        {
            var facilities = CreateFacilities5(true, true);
            return new SmurfphoneFactoryTestCase(136, 12, 1.0, facilities.Item1, facilities.Item2, 1.0, 12, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase6()
        {
            var facilities = CreateFacilities5(true, true);
            return new SmurfphoneFactoryTestCase(39, 3, 1.0, facilities.Item1, facilities.Item2, 2.0, 3, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase7()
        {
            var providers = new[]
            {
                new Provider(5, 2, new Vector2(0, 1)),
                new Provider(10, 2.5, new Vector2(0, 2)),
            };
            var factories = new[]
            {
                new Factory(3, 5, 3, new Vector2(2, 3)),
                new Factory(4, 8, 6, new Vector2(2, 1)),
            };
            return new SmurfphoneFactoryTestCase(138, 15, 1.0, providers, factories, 1.0, 22);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase8()
        {
            var facilities = CreateRandomFacilities(true, true, 7, 4, 11);
            return new SmurfphoneFactoryTestCase(396, 24, 1.0, facilities.Item1, facilities.Item2, 2.0, 27);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase9()
        {
            var facilities = CreateRandomFacilities(true, true, 6, 4, 76839420);
            return new SmurfphoneFactoryTestCase(394, 21, 1.0, facilities.Item1, facilities.Item2, 3.0, 21, true);
        }

        private static SmurfphoneFactoryTestCase CreateCappedTestCase10()
        {
            var facilities = CreateRandomFacilities(true, true, 40, 30, 92131);
            return new SmurfphoneFactoryTestCase(1952, 120, 1.0, facilities.Item1, facilities.Item2, 10, 120, true);
        }
        #endregion

        #endregion


        static void Main()
        {

            if (Graphs.GraphLibraryVersion.ToString() != "7.0.3")
            {
                Console.WriteLine("\nPobierz nową wersję biblioteki\n");
                return;
            }

            var simpleTests = new TestSet();

            simpleTests.TestCases.Add(CreateBasicTest1());
            simpleTests.TestCases.Add(CreateBasicTest2());
            simpleTests.TestCases.Add(CreateBasicTest3());
            simpleTests.TestCases.Add(CreateBasicTest4());
            simpleTests.TestCases.Add(CreateBasicTest5());
            simpleTests.TestCases.Add(CreateBasicTest6());
            simpleTests.TestCases.Add(CreateBasicTest7());
            simpleTests.TestCases.Add(CreateBasicTest8());
            Console.WriteLine("Standard tests:");
            simpleTests.PreformTests(true, false);


            var fullTests = new TestSet();
            fullTests.TestCases.Add(CreateFullTestCase1());
            fullTests.TestCases.Add(CreateFullTestCase2());
            fullTests.TestCases.Add(CreateFullTestCase3());
            fullTests.TestCases.Add(CreateFullTestCase4());
            fullTests.TestCases.Add(CreateFullTestCase5());
            fullTests.TestCases.Add(CreateFullTestCase6());
            fullTests.TestCases.Add(CreateFullTestCase7());
            fullTests.TestCases.Add(CreateFullTestCase8());
            fullTests.TestCases.Add(CreateFullTestCase9());
            fullTests.TestCases.Add(CreateFullTestCase10());
            fullTests.TestCases.Add(CreateFullTestCase11());
            Console.WriteLine("Tests with limits:");
            fullTests.PreformTests(true, false);


            var cappedTests = new TestSet();
            cappedTests.TestCases.Add(CreateCappedTestCase1());
            cappedTests.TestCases.Add(CreateCappedTestCase2());
            cappedTests.TestCases.Add(CreateCappedTestCase3());
            cappedTests.TestCases.Add(CreateCappedTestCase4());
            cappedTests.TestCases.Add(CreateCappedTestCase5());
            cappedTests.TestCases.Add(CreateCappedTestCase6());
            cappedTests.TestCases.Add(CreateCappedTestCase7());
            cappedTests.TestCases.Add(CreateCappedTestCase8());
            cappedTests.TestCases.Add(CreateCappedTestCase9());
            cappedTests.TestCases.Add(CreateCappedTestCase10());
            Console.WriteLine("Test with capped production:");
            cappedTests.PreformTests(true, false);

        }
    }
}
