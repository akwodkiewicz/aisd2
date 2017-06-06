using System;
using System.Diagnostics;
using System.Text;

namespace ASD
{    
    /// <summary>
    /// Struktura przechowująca wyniki: minimalną i maksymalną prędkość wynikającą z obliczeń.
    /// </summary>
    public struct Velocities
    {
        public readonly int minVelocity, maxVelocity;
        public Velocities(int minVelocity, int maxVelocity) { this.minVelocity = minVelocity; this.maxVelocity = maxVelocity; }
    }

    delegate Velocities Lab04Method(int[] measurements, out bool[] isBraking);

    class VelocityMeasurementsTestCase : TestCase
    {
        private readonly int[] measurements;
        private readonly int minVelocity, maxVelocity;
        private readonly bool shouldIsBrakingNull;
        private readonly Lab04Method testedFunction;
        private Velocities result;
        private bool[] isBraking;

        public VelocityMeasurementsTestCase(double timeLimit, Lab04Method testedFunction, int[] measurements, int minVelocity, int maxVelocity, bool shouldIsBrakingNull = false) : base(timeLimit, null)
        {
            this.measurements = measurements;
            this.testedFunction = testedFunction;
            this.minVelocity = minVelocity;
            this.maxVelocity = maxVelocity;
            this.shouldIsBrakingNull = shouldIsBrakingNull;
        }

        public override void PerformTestCase()
        {
            result = testedFunction(measurements, out isBraking);
        }

        public override void VerifyTestCase(out Result resultCode, out string message)
        {
            if (minVelocity == result.minVelocity && maxVelocity == result.maxVelocity && IsBrakingArrayCorrect())
            {
                resultCode = Result.Success;
                message = "OK";
            }
            else
            {
                // W przypadku błędu tworzony jest napis informujący która wartość została źle policzona
                // Jeżeli obie wartości są błędne dodaję nową linię tak żeby było to czytelniejsze
                resultCode = Result.BadResult;
                StringBuilder messageText = new StringBuilder("incorrect ");
                if (minVelocity != result.minVelocity) messageText.Append("minimum_velocity=" + result.minVelocity + " expected: " + minVelocity + " ");
                if (minVelocity != result.minVelocity && maxVelocity != result.maxVelocity) messageText.Append("\n\t\t    ");
                if (maxVelocity != result.maxVelocity) messageText.Append("maximum_velocity=" + result.maxVelocity + " expected: " + maxVelocity + " ");
                if (!IsBrakingArrayCorrect() && (minVelocity != result.minVelocity || maxVelocity != result.maxVelocity)) messageText.Append("\n\t\t    ");
                if (!IsBrakingArrayCorrect()) messageText.Append(GetIsBrakingArrayError());
                message = messageText.ToString();
            }            
        }

        private bool IsBrakingArrayCorrect()
        {
            if (shouldIsBrakingNull && isBraking == null) return true;
            if (shouldIsBrakingNull && isBraking != null) return false;
            if (isBraking == null || isBraking.Length != measurements.Length) return false;

            int currentSpeedValue = 0;
            for(int i = 0; i < measurements.Length; i++)
            {
                currentSpeedValue += (isBraking[i] ? -1 : 1) * measurements[i];
            }

            return (Math.Abs(currentSpeedValue) == this.minVelocity);
        }

        private string GetIsBrakingArrayError()
        {
            StringBuilder errorMessage = new StringBuilder("isBraking array ");
            if (isBraking == null) errorMessage.Append("should not be null");
            else if (isBraking != null && shouldIsBrakingNull) errorMessage.Append("should be null");
            else if (isBraking.Length != measurements.Length) errorMessage.Append("has incorrect length");

            return errorMessage.ToString();
        }

        ///CustomIsBrakingArrayCorrect by G.Rozdzialik:
        public bool CustomIsBrakingArrayCorrect(bool[] customIsBraking)
        {
            bool[] initialIsBraking = isBraking;
            isBraking = customIsBraking;
            bool result = IsBrakingArrayCorrect();
            isBraking = initialIsBraking;
            return result;
        }
    }

    class Lab04_Main
    {
        static void Main()
        {
            TestSet finalVelocitiesTests = new TestSet();
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 0 }, 0, 0));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 10 }, 10, 10));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 10, 3, 5, 4 }, 2, 22));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 4, 11, 5, 5, 5 }, 0, 30));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 10, 10, 5, 3, 1 }, 1, 29));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 10, 10, 5, 3, 1, 9, 24, 3, 4, 19, 18, 7, 7, 8, 10, 5 }, 1, 143));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, new int[] { 7, 10, 2, 18, 4, 6, 6 }, 1, 53));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, GenerateTestArray(20, 1024), 1, 1101));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, GenerateTestArray(100, 1024), 0, 4650));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, GenerateTestArray(100, 123424), 1, 5337));
            finalVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.FinalVelocities, GenerateTestArray(1000, 123424), 1, 49209));

            Console.WriteLine("\nFinal velocities tests");
            finalVelocitiesTests.PreformTests(verbose: true, checkTimeLimit: false);


            TestSet journeyVelocitiesTests = new TestSet();
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, new int[] { 10 }, 10, 10, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, new int[] { 10, 1, 1, 1 }, 7, 13, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, new int[] { 10, 3, 5, 4 }, 2, 22, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, new int[] { 4, 11, 5, 5, 5 }, 0, 30, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, new int[] { 10, 10, 5, 3, 1 }, 0, 29, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, new int[] { 5, 5, 10, 23, 45, 2, 1, 23, 9, 0, 8, 4, 1, 24, 86, 5, 6, 100, 353, 4, 5, 67, 32, 45, 23, 34, 56, 32, 23 }, 0, 1031, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, GenerateTestArray(20, 1024), 0, 1101, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, GenerateTestArray(100, 1024), 0, 4650, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, GenerateTestArray(100, 123424), 0, 5337, true));
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, GenerateTestArray(1000, 123424), 0, 49209, true));
            int[] x = { 2, 2 }, y = GenerateTestArray(1000, 123424), z = new int[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            journeyVelocitiesTests.TestCases.Add(new VelocityMeasurementsTestCase(5, VelocityMeasurements.JourneyVelocities, z, 0, 49213, true));



            Console.WriteLine("\nJourney velocities tests");
            journeyVelocitiesTests.PreformTests(verbose: true, checkTimeLimit: false);

            //Custom Tests by G.Rozdzialik:
            Console.WriteLine("Custom tests");

            int[] testMeasurementsCounts = new int[] { 5,100,150,200,300,400,500,1500,4000 };
            int[] seeds = new int[] { 123456,1000,123456,123456,123456,123456,123456,123456,123456 };

            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < testMeasurementsCounts.Length; i++)
            {
                int[] measurements = GenerateTestArray(testMeasurementsCounts[i],seeds[i]);
                bool[] isBrakingValue;

                stopwatch.Restart();
                Velocities velocities = VelocityMeasurements.FinalVelocities(measurements,out isBrakingValue);
                stopwatch.Stop();

                Console.WriteLine("Custom test {0,-1}:",i + 1,velocities.minVelocity,velocities.maxVelocity);
                Console.WriteLine("  min = {0}",velocities.minVelocity);
                Console.WriteLine("  max = {0}",velocities.maxVelocity);

                VelocityMeasurementsTestCase testCase = new VelocityMeasurementsTestCase(2,VelocityMeasurements.FinalVelocities,
                    measurements,velocities.minVelocity,velocities.maxVelocity);

                if (testCase.CustomIsBrakingArrayCorrect(isBrakingValue))
                    Console.WriteLine("  isBrakingValue correct");
                else
                    Console.WriteLine("  isBrakingValue INVALID");
                Console.WriteLine("  time elapsed = {0,5} ms",stopwatch.ElapsedMilliseconds);
                Console.WriteLine(String.Empty);
            }
            //------------------------------------------------------------------------






        }

        static int[] GenerateTestArray(int numberOfElements, int seed)
        {
            Random r = new Random(seed);
            int[] testArray = new int[numberOfElements];
            for (int i = 0; i < numberOfElements; i++) testArray[i] = r.Next(100);
            return testArray;
        }
    }
}
