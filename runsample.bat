@echo off
cd solr
md logs
start java -jar start.jar
cd..
start "" "%programfiles%\iis express\iisexpress.exe" "/path:%cd%\SampleSolrApp" "/port:8082"
cscript /nologo pingsolr.js
start http://localhost:8082/
exit