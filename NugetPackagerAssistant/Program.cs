using System;
using System.Configuration;

namespace NugetPackagerAssistant
{
    public static class Program
    {
        private static string NugetDir;
        private static string OutputDir;
        private static string PublishDir;

        public static void Main(string[] args)
        {
            try
            {
                Package(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to exit ...");
                Console.ReadKey();
            }
        }

        public static void Package(string[] args)
        {
            SetInputOptions(args);
            var nugetPackager = new NugetPackager(NugetDir, OutputDir, PublishDir);
            nugetPackager.GenerateNugetPackage();
        }

        public static void SetInputOptions(string[] args)
        {
            if (args.Length > 0 && args.Length < 3)
                throw new ArgumentException("You must provide exactly 3 arguments in this order : nugetDirectory, outputDirectory and publishDirectory.");

            if (args.Length == 0)
            {
                NugetDir = ConfigurationManager.AppSettings["nugetDirectory"];
                OutputDir = ConfigurationManager.AppSettings["outputDirectory"];
                PublishDir = ConfigurationManager.AppSettings["publishDirectory"];
            }
            else
            {
                NugetDir = args[0];
                OutputDir = args[1];
                PublishDir = args[2];
            }
        }
    }
}
