del "..\..\BackToFront.Tests\bin\Debug\Javascript\CURRENT_TEST.js"
deltree "..\..\BackToFront.Tests\bin\Debug\Javascript\Base\*.*" /y
copy "..\..\BackToFront.Tests\Javascript\CURRENT_TEST.js" "..\..\BackToFront.Tests\bin\Debug\Javascript\CURRENT_TEST.js"
xcopy "..\..\BackToFront.Tests\Javascript\Base" "..\..\BackToFront.Tests\bin\Debug\Javascript\Base" /E /i /y
..\..\BackToFront.Tests\bin\Debug\Chutzpah\chutzpah.console.exe /path "..\..\BackToFront.Tests\bin\Debug\Javascript\CURRENT_TEST.js" /testMode JavaScript /failOnScriptError /debug  /openInBrowser