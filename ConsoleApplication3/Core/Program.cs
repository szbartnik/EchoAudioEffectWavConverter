using System;

namespace SMiW.Lab.Core
{
    public static class Program
    {
        /// <summary>
        /// Main entry point
        /// Example usage 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invoke using [inputWavFilePath.wav] [outputWavFilePath.wav]!");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.WriteLine("Reading input file '{0}'", args[0]);
                var dspTester = new DspTester(args[0]);

                Console.WriteLine("Sound processing...");
                dspTester.StartCppProcessing();

                Console.WriteLine("Saving output file '{0}'", args[1]);
                dspTester.SaveOutputFile(args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Successfully saved!");
            Console.ReadKey();
        }
    }
}