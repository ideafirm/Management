:: @ECHO OFF

:: Build packages
cd src\Steeltoe.Management.Diagnostics
dotnet restore --configfile ..\..\nuget.config
IF NOT "%APPVEYOR_REPO_TAG_NAME%"=="" (
    IF NOT "%STEELTOE_VERSION_SUFFIX%"=="" (
        dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.Diagnostics.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg" -Source "%USERPROFILE%\localfeed"
    ) ELSE (
        dotnet pack --configuration %BUILD_TYPE%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.Diagnostics.%STEELTOE_VERSION%.nupkg" -Source "%USERPROFILE%\localfeed"
    )    
)

IF "%APPVEYOR_REPO_TAG_NAME%"=="" (dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX% --include-symbols --include-source)
IF "%APPVEYOR_REPO_TAG_NAME%"=="" (nuget add -Source %USERPROFILE%\localfeed bin\%BUILD_TYPE%\Steeltoe.Management.Diagnostics.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg)
cd ..\..

cd src\Steeltoe.Management.EndpointBase
dotnet restore --configfile ..\..\nuget.config
IF NOT "%APPVEYOR_REPO_TAG_NAME%"=="" (
    IF NOT "%STEELTOE_VERSION_SUFFIX%"=="" (
        dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.EndpointBase.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg" -Source "%USERPROFILE%\localfeed"
    ) ELSE (
        dotnet pack --configuration %BUILD_TYPE%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.EndpointBase.%STEELTOE_VERSION%.nupkg" -Source "%USERPROFILE%\localfeed"
    )    
)

IF "%APPVEYOR_REPO_TAG_NAME%"=="" (dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX% --include-symbols --include-source)
IF "%APPVEYOR_REPO_TAG_NAME%"=="" (nuget add -Source %USERPROFILE%\localfeed bin\%BUILD_TYPE%\Steeltoe.Management.EndpointBase.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg)
cd ..\..

cd src\Steeltoe.Management.EndpointCore
dotnet restore --configfile ..\..\nuget.config
IF NOT "%APPVEYOR_REPO_TAG_NAME%"=="" (
    IF NOT "%STEELTOE_VERSION_SUFFIX%"=="" (
        dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.EndpointCore.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg" -Source "%USERPROFILE%\localfeed"
    ) ELSE (
        dotnet pack --configuration %BUILD_TYPE%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.EndpointCore.%STEELTOE_VERSION%.nupkg" -Source "%USERPROFILE%\localfeed"
    )    
)
IF "%APPVEYOR_REPO_TAG_NAME%"=="" (dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX% --include-symbols --include-source)
IF "%APPVEYOR_REPO_TAG_NAME%"=="" (nuget add -Source %USERPROFILE%\localfeed bin\%BUILD_TYPE%\Steeltoe.Management.EndpointCore.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg)
cd ..\..

cd src\Steeltoe.Management.CloudFoundryCore
dotnet restore --configfile ..\..\nuget.config
IF NOT "%APPVEYOR_REPO_TAG_NAME%"=="" (
    IF NOT "%STEELTOE_VERSION_SUFFIX%"=="" (
        dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.CloudFoundryCore.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg" -Source "%USERPROFILE%\localfeed"
    ) ELSE (
        dotnet pack --configuration %BUILD_TYPE%
        nuget add "bin\%BUILD_TYPE%\Steeltoe.Management.CloudFoundryCore.%STEELTOE_VERSION%.nupkg" -Source "%USERPROFILE%\localfeed"
    )    
)
IF "%APPVEYOR_REPO_TAG_NAME%"=="" (dotnet pack --configuration %BUILD_TYPE% --version-suffix %STEELTOE_VERSION_SUFFIX% --include-symbols --include-source)
IF "%APPVEYOR_REPO_TAG_NAME%"=="" (nuget add -Source %USERPROFILE%\localfeed bin\%BUILD_TYPE%\Steeltoe.Management.CloudFoundryCore.%STEELTOE_VERSION%-%STEELTOE_VERSION_SUFFIX%.nupkg)

cd ..\..