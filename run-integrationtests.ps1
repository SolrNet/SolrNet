Write-Output "Starting SOLR" 
Set-Location .\solr
$solr = start-process java -argumentlist "-jar start.jar" -PassThru

for($i=1; $i -le 10; $i++)
{
    try { 
        $response = Invoke-WebRequest "http://127.0.0.1:8983/solr"
        Write-Output "Solr is running"
        break;
    } 
    catch {
        $_.Exception.Response.StatusCode.Value__
        Write-Warning "$i/10 Solr not ready yet"
    }

    if ($i -eq 10) 
    {
        Write-Error "Solr is not running"
        Stop-Process $solr.Id
        exit
    }
   

    Start-Sleep -s 1
}

Set-Location ..
Write-Output "Start running tests"

$tests = Get-ChildItem -path . -Recurse -Include *Test*.dll | where { $_.FullName -match "$($_.name.substring(0,$_.name.length - 4))\\bin" } 

foreach($test in $tests)
{
    .\packages\xunit.runner.console.2.2.0\tools\xunit.console.exe $test -trait "Category=Integration"
    if ($LASTEXITCODE -ne 0 ) 
    {
        Write-Error "Abort test run, last test failed"
        break
    }
}




Write-Output "Shutting down SOLR" 
Stop-Process $solr.Id