mkdir "..\Output"
set input=..\Source\Examples\
set output=..\Output
set target=\bin\Release\*.*
xcopy %input%AudioDemo%target% %output% /S /Y
xcopy %input%ClothDemo%target% %output% /S /Y
xcopy %input%DnaDemo%target% %output% /S /Y
xcopy %input%EarthDemo%target% %output% /S /Y
xcopy %input%FractalDemo%target% %output% /S /Y
xcopy %input%LifeSimulationDemo%target% %output% /S /Y
xcopy %input%LightsDemo%target% %output% /S /Y
xcopy %input%MaterialDemo%target% %output% /S /Y
xcopy %input%PanoramaDemo%target% %output% /S /Y
xcopy %input%PetzoldDemo%target% %output% /S /Y
xcopy %input%PyramidDemo%target% %output% /S /Y
xcopy %input%SimpleDemo%target% %output% /S /Y
xcopy %input%SolarsystemDemo%target% %output% /S /Y
xcopy %input%StereoDemo%target% %output% /S /Y
xcopy %input%StreamlinesDemo%target% %output% /S /Y
xcopy %input%Studio%target% %output% /S /Y
xcopy %input%Surface%target% %output% /S /Y
xcopy %input%Terrain%target% %output% /S /Y
xcopy %input%Tube%target% %output% /S /Y
del %output%\*.pdb
del %output%\*.vshost.exe
del %output%\*.manifest
del %output%\*.config