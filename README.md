# SonarSilencer
Tool to disable all SonarAnalyzer.CSharp analyzers and keep only the ones you want to actually use via .editorconfig

Usage: `sonarsilencer "Critical Security Hotspot" "Major Bug"`

You provide the positive list (the categories you want to keep)

Current categories at the time of writing:

* Minor Code Smell
* Major Code Smell
* Major Bug
* Critical Code Smell
* Blocker Code Smell
* Critical Bug
* Critical Vulnerability
* Info Code Smell
* Blocker Bug
* Blocker Vulnerability
* Minor Bug
* Critical Security Hotspot
* Minor Security Hotspot
* Major Security Hotspot
* Blocker Security Hotspot
* Major Vulnerability