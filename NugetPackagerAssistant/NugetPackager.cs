using NugetPackagerAssistant.Common;
using NugetPackagerAssistant.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NugetPackagerAssistant
{
    public class NugetPackager
    {
        private enum NugetPackagingMode { Csproj = 0, Dll = 1 };
        private string _csprojPathFromuserInput;
        private string _dllDirectoryPathFromUserInput;
        private string[] _dlls;
        private readonly string _nugetExePath;
        private readonly List<string> _nuspecs = new List<string>();
        private readonly string _outputDirectory;
        private readonly string _publishDirectory;
        private string _tmpDirectoryPath;
        private NugetPackagingMode _usersModeChoice;

        public NugetPackager(string nugetDir, string outputDir, string publishDir)
        {
            _outputDirectory = outputDir;
            _publishDirectory = publishDir;
            _nugetExePath = string.Concat(nugetDir, Path.DirectorySeparatorChar, "nuget.exe");
        }

        public void GenerateNugetPackage()
        {
            DisplayMenu();
            GetUsersModeChoice();

            if (_usersModeChoice == NugetPackagingMode.Csproj)
                PackCsproj();
            else
                GenerateNuspecAndPackDlls();

            PublishNupkgToSharedFolder();
        }

        private void CleanNuspecs()
        {
            foreach (var nuspec in _nuspecs)
            {
                var xmlHelper = new XmlHelper(nuspec);
                var dllName = string.Concat(Path.GetFileNameWithoutExtension(nuspec), ".dll");

                var fileElement = new XElement(
                    "file",
                    new XAttribute("src", string.Concat(_dllDirectoryPathFromUserInput, Path.DirectorySeparatorChar, dllName)),
                    new XAttribute("target", string.Concat("lib", Path.DirectorySeparatorChar, dllName))
                );

                xmlHelper.RemoveElement("package/metadata/licenseUrl");
                xmlHelper.RemoveElement("package/metadata/projectUrl");
                xmlHelper.RemoveElement("package/metadata/iconUrl");
                xmlHelper.RemoveElement("package/metadata/tags");
                xmlHelper.RemoveElement("package/metadata/dependencies");
                xmlHelper.AddElement("package/metadata", new XElement("files"));
                xmlHelper.AddElement("package/files", fileElement, false);
                xmlHelper.Save();
            }
        }

        private void CopyFileToDirectory(string fileName)
        {
            var targetLocation = string.Concat(_publishDirectory, Path.DirectorySeparatorChar, Path.GetFileName(fileName));
            File.Copy(fileName, targetLocation, true);
            Console.WriteLine("{0} has been published", Path.GetFileName(fileName));
        }

        private void DisplayMenu()
        {
            var sb = new StringBuilder();
            sb.AppendLine("NuGet Packager");
            sb.AppendLine("==============");
            sb.AppendLine("There are two options to create a NuGet :");
            sb.AppendLine("A - You have the source code (with the .csproj)");
            sb.AppendLine("B - You only have a DLL (a tierce library for instance)");
            Console.WriteLine(sb.ToString());
        }

        private void GenerateNuspec()
        {
            if (_dlls.Length == 0)
                throw new NuspecException("No DLL found in this folder.");

            Process process = ProcessHelper.StartNewProcess(_nugetExePath, _tmpDirectoryPath);

            foreach (string dll in _dlls)
            {
                process.StartInfo.Arguments = string.Format("spec -a \"{0}\"", dll);
                process.Start();

                // The PackNuspecs method needs to wait for all of the .nuspec be created before packing them into .nupkg
                process.WaitForExit(10000);

                _nuspecs.Add(string.Concat(_tmpDirectoryPath, Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension(dll), ".nuspec"));
                Console.WriteLine("{0}.nuspec created", Path.GetFileNameWithoutExtension(dll));
            }
        }

        private void GenerateNuspecAndPackDlls()
        {
            GetDllsFromUserInput();

            Directory.CreateDirectory(_tmpDirectoryPath);

            GenerateNuspec();
            CleanNuspecs();
            PackNuspecs();

            Directory.Delete(_tmpDirectoryPath, true);
        }

        private void GetCsprojFromUserInput()
        {
            Console.WriteLine("Please enter the full path of the .csproj that you would like to NuGet :");
            _csprojPathFromuserInput = Console.ReadLine();
        }

        private void GetDllsFromUserInput()
        {
            _dllDirectoryPathFromUserInput = string.Empty;
            while (!IsValidPath(_dllDirectoryPathFromUserInput))
            {
                Console.WriteLine("Please enter the directory path in which you have your DLLs that you would like to NuGet :");
                _dllDirectoryPathFromUserInput = Console.ReadLine();
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            _dlls = Directory.GetFiles(_dllDirectoryPathFromUserInput, "*.dll");
            _tmpDirectoryPath = string.Concat(_dllDirectoryPathFromUserInput, Path.DirectorySeparatorChar, "tmp");
        }

        private void GetUsersModeChoice()
        {
            _usersModeChoice = ConsoleHelper.PromptChoices(ConsoleKey.A, ConsoleKey.B) == ConsoleKey.A
                ? NugetPackagingMode.Csproj
                : NugetPackagingMode.Dll;
        }

        private bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                // If the path is invalid, it will throw an Exception
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Path.GetFullPath(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void PackCsproj()
        {
            GetCsprojFromUserInput();

            var arguments = string.Format("pack \"{0}\" -noninteractive -prop configuration=Release -build -outputdirectory \"{1}\"", _csprojPathFromuserInput, _outputDirectory);
            var process = ProcessHelper.StartNewProcess(_nugetExePath, _outputDirectory, arguments);

            process.Start();
            process.WaitForExit(10000);

            Console.WriteLine("1 .nupkg generated in {0}", _outputDirectory);
        }

        private void PackNuspecs()
        {
            Process process = ProcessHelper.StartNewProcess(_nugetExePath, _outputDirectory);

            Directory.CreateDirectory(_outputDirectory);

            foreach (string nuspec in _nuspecs)
            {
                process.StartInfo.Arguments = string.Format("pack \"{0}\" -outputdirectory \"{1}\" -nopackageanalysis -noninteractive", nuspec, _outputDirectory);
                process.Start();
                process.WaitForExit(10000);

                Console.WriteLine("{0}.nupkg created", Path.GetFileNameWithoutExtension(nuspec));
            }
        }

        private void PublishNupkgToSharedFolder()
        {
            Console.WriteLine("Would you like us to publish your recently created .nupkg to your publish folder ?");

            if (ConsoleHelper.PromptChoices(ConsoleKey.Y, ConsoleKey.N) == ConsoleKey.Y)
            {
                Directory.GetFiles(_outputDirectory, "*.nupkg").ToList().ForEach(CopyFileToDirectory);
                Console.WriteLine("The publication of your .nupkg has complete");
            }
        }
    }
}
