using System;
using System.Configuration;

namespace NugetPackagerAssistant
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string nugetDir, outputDir, publishDir;
                if (args.Length > 0 && args.Length < 3)
                    throw new ArgumentException("You must provide exactly 3 arguments in this order : nugetDirectory, outputDirectory and publishDirectory.");

                if (args.Length == 0)
                {
                    nugetDir = ConfigurationManager.AppSettings["nugetDirectory"];
                    outputDir = ConfigurationManager.AppSettings["outputDirectory"];
                    publishDir = ConfigurationManager.AppSettings["publishDirectory"];
                }
                else
                {
                    nugetDir = args[0];
                    outputDir = args[1];
                    publishDir = args[2];
                }

                var nugetPackager = new NugetPackager(nugetDir, outputDir, publishDir);
                nugetPackager.GenerateNugetPackage();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Press any key to exit ...");
                Console.ReadKey();
            }
        }
    }
}
