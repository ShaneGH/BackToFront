deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\UnitTests\*.*" /y
deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\Base\*.*" /y
xcopy "..\..\BackToFront.Tests\Javascript\UnitTests" "..\..\BackToFront.Tests\bin\Debug\Javascript\UnitTests" /E /i /y
xcopy "..\..\BackToFront.Tests\Javascript\Base" "..\..\BackToFront.Tests\bin\Debug\Javascript\Base" /E /i /y
..\..\BackToFront.Tests\bin\Debug\Chutzpah\chutzpah.console.exe /path "..\..\BackToFront.Tests\bin\Debug\Javascript\UnitTests" /testMode JavaScript /failOnScriptError /wait