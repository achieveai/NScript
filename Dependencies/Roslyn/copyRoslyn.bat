@echo off
set DST=%~dp0
set SRC=%1
set Flavor=%2

copy %SRC%\artifacts\bin\Microsoft.CodeAnalysis.CSharp\%Flavor%\netstandard2.0\Microsoft.CodeAnalysis.CSharp.??? %DST%
copy %SRC%\artifacts\bin\Microsoft.CodeAnalysis\%Flavor%\netstandard2.0\*.??? %DST%
