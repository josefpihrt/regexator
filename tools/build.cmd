@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild" "..\src\Regexator.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Release,RunCodeAnalysis=false,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /nr:false ^
 /v:normal ^
 /m

pause