# NugetPackagerAssistant
This tool will assist you when packaging your code into NuGet modules (nupkg). If you have a team repository for instance, this tool could help you packaging your shared dlls, whether you have the source code (csproj) or only the dlls.

Prerequisites
---
- You will need to download [nuget.exe v3.5.0](https://dist.nuget.org/index.html) (official NuGet website). Unzip it where you wish to.
- Have ```.NET 4.5.2``` installed
- Open the ```App.config``` file (located under NugetPackagerAssistant) and set these 3 variables
 - ```nugetDirectory``` : folder containing your previously unzipped nuget.exe - (ex : *C:\Programs\Nuget*)
 - ```outputDirectory``` : folder that will contain the all of the nupkg that will be generated
 - ```publishDirectory``` : the shared folder that contains all of your NuGet packages. This is basically your team repository (it could be on a NAS for instance).

Executing the tool
---
- **User Mode :** Open a ```cmd``` and navigate to your NugetPackagerAssistant directory. Execute ```NugetPackageAssistant.exe``` then read section A or B depending on your situation.
- **Dev Mode :** Build and run the project on Visual Studio (don't forget to set the options (see the Prerequisites section)

There are currently 2 modes when it comes to create a Nuget package

A - Csproj Mode (you have the source code)
---
- When promped, press "A" then paste your **csproj** path ```pathTo/your/csproj/project.csproj```
- Finally, choose whether or not you'd like to publish the generated nupkg

B - Dll Mode (you only have dlls)
---
- When promped, press "B" then paste the uri of a folder containing the dlls you'd wish to NuGet ```pathTo/your/dlls```
- Finally, choose whether or not you'd like to publish the generated nupkg

Contributing
---
If you'd like to add a new feature or to report a bug, please create an issue. Feel free to suggest a pull request too :)
