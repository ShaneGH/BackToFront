IF NOT EXIST Javascript\Runtime\Bin mkdir Javascript\Runtime\Bin
Javascript\Chutzpah\chutzpah.console.exe /path ..\..\Javascript\UnitTests /testMode TypeScript /junit Javascript\Runtime\Bin\TestsResults.xml
EXIT 0