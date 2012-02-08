@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%cd%
SET BUILD_DIR=%~d0%~p0%
SET NANT="%BUILD_DIR%lib\Nant\nant.exe"
SET build.config.settings="%DIR%\settings\UppercuT.config"

%NANT% -logger:NAnt.Core.DefaultLogger -quiet /f:"%BUILD_DIR%build\default.build" -D:build.config.settings=%build.config.settings% %*

echo BUILDING
msbuild

echo.
echo RUNNING TESTS
lib\nspec.0.9.58\tools\NSpecRunner.exe warmup.Tests\bin\Debug\warmup.Tests.dll

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:usage
echo.
echo Usage: build.bat
echo.

goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish