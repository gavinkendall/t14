@echo off

cd ..

dotnet publish t14.sln --configuration Release --self-contained true --runtime win-x64
cd bin\Release\netcoreapp3.0\win-x64
for /f "delims=" %%d in ('dir /ad /b') do @rd /q /s "%%d"
del t14.deps.json
del t14.pdb
del t14.runtimeconfig.dev.json
del t14.runtimeconfig.json
cd ..\..\..\..

dotnet publish t14.sln --configuration Release --self-contained true --runtime linux-x64
cd bin\Release\netcoreapp3.0\linux-x64
for /f "delims=" %%d in ('dir /ad /b') do @rd /q /s "%%d"
del t14.deps.json
del t14.pdb
del t14.runtimeconfig.dev.json
del t14.runtimeconfig.json
cd ..\..\..\..

dotnet publish t14.sln --configuration Release --self-contained true --runtime osx-x64
cd bin\Release\netcoreapp3.0\osx-x64
for /f "delims=" %%d in ('dir /ad /b') do @rd /q /s "%%d"
del t14.deps.json
del t14.pdb
del t14.runtimeconfig.dev.json
del t14.runtimeconfig.json

cd ..\..\..\..\build