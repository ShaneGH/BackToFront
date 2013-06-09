deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\WebExpressions\*.*" /y
deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\Base\*.*" /y
xcopy "..\..\BackToFront.Tests\Javascript\WebExpressions" "..\..\BackToFront.Tests\bin\Debug\Javascript\WebExpressions" /E /i /y
xcopy "..\..\BackToFront.Tests\Javascript\Base" "..\..\BackToFront.Tests\bin\Debug\Javascript\Base" /E /i /y
..\..\BackToFront.Tests\bin\Debug\Chutzpah\chutzpah.console.exe /path "..\..\BackToFront.Tests\bin\Debug\Javascript\WebExpressions" /testMode JavaScript /failOnScriptError /debug  /openInBrowser