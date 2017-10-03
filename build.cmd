%nuget% restore T4CodeGenerator.sln
"%MsBuildExe%" T4CodeGenerator.sln /p:Configuration="%Configuration%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false
%nuget% pack "ReSharperExtension\package.nuspec" -Version %PackageVersion%
%nuget% pack "Generators.Core\package.nuspec" -Version %PackageVersion%
