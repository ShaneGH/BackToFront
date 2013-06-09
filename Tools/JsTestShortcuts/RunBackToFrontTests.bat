deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\BackToFront\*.*" /y
deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\Base\*.*" /y
xcopy "..\..\BackToFront.Tests\Javascript\BackToFront" "..\..\BackToFront.Tests\bin\Debug\Javascript\BackToFront" /E /i /y
xcopy "..\..\BackToFront.Tests\Javascript\Base" "..\..\BackToFront.Tests\bin\Debug\Javascript\Base" /E /i /y
..\..\BackToFront.Tests\bin\Debug\Chutzpah\chutzpah.console.exe /path "..\..\BackToFront.Tests\bin\Debug\Javascript\BackToFront" /testMode JavaScript /failOnScriptError /wait