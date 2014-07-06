# Giles - Watcher extraordinaire..  
a continuous test runner for .NET applications. True to form, Giles will watch you kick ass, and let you know right away when you have made a mistake (intentionally or not).

# Usage:

```
giles.exe -s [path_to_solution]
```

or

```
giles.ps1 (from root of solution, this will usually find your solution and test assemblies for you)
```

(```giles-x86.exe``` and ```giles-x86.ps1``` are also available for strictly 32-bit applications)


# Command Line Help:

Giles Options

  s, SolutionPath        Required. Path to the sln file

  t, TestAssemblyPath    Optional, Giles will try to find the test assembly

  help                   Display this help screen.



# Interactive Console:

While in the interactive console, press ? for a list of options:

```
Interactive Console Options:
   ? = Display options
   C = Clear the window
   I = Show current configuration
   R = Run build & tests now
   V = Display all messages from last test run
   E = Display errors from last test run
   B = Set Build Delay
   F = Set Test Filters
   H = Clear Test Filters
   Q = Quit
   P = Toggle Pause Giles
```

# FAQ
Giles shows build error while VS compiles without problem. Why?
   - change MSBuild.exe path in Giles config file. Giles uses .NET default MSBuild while some VS (like 2013) uses standalone version (for example: C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe)

For more information and requirement to run Giles, please visit the [wiki](https://github.com/codereflection/Giles/wiki)
