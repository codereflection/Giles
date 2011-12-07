# Giles - Watcher extraordinaire..  
a continuous test runner for .NET applications. True to form, Giles will watch you kick ass, and let you know right away when you have made a mistake (intentionally or not).

# Useage:

giles.exe -s [path_to_solution]


# Command Line Help:

Giles Options

  s, SolutionPath        Required. Path to the sln file

  t, TestAssemblyPath    Required. Path to the test assembly dll ( Optional, Giles will try to find the test assembly )

  help                   Display this help screen.



# Interactive Console:

While in the interactive console, press ? for a list of options:

Interactive Console Options:
  ? = Display options 
  C = Clear the window
  I = Show current configuration
  R = Run build & tests now
  V = Display last test run messages
  E = Display last test run errors
  B = Set Build Delay
  Q = Quit  

For more information and requirement to run Giles, please visit the [wiki](https://github.com/codereflection/Giles/wiki)