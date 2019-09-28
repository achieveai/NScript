@echo off
set DST=%~dp0
set SRC=%1
set Flavor=%2

copy %SRC%\Binaries\%Flavor%\Dlls\Microsoft.CodeAnalysis.CSharp\Microsoft.CodeAnalysis.CSharp.??? %DST%
copy %SRC%\Binaries\%Flavor%\Dlls\Microsoft.CodeAnalysis\*.??? %DST%
