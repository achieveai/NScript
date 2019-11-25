@echo off
set DST=%~dp0
set SRC=%1
set Flavor=%2

copy %SRC%\artifacts\bin\Microsoft.CodeAnalysis.CSharp\%Flavor%\netstandard2.0\ref\Microsoft.CodeAnalysis.CSharp.???
copy %SRC%\artifacts\bin\Microsoft.CodeAnalysis\%Flavor%\netstandard2.0\ref\*.??? %DST%
