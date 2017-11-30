namespace Evidence
{
    using System;

    using BenchmarkDotNet.Running;


    public class Program
    {

        /// <summary>
        /// Entry point for running benchmarks.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<StructByRefVsAlternatives>();
        }

    }
}
