@echo off
cd solr
md logs
start java -jar start.jar
cd ..
start tools\cassini\cassini.exe "%cd%\SampleSolrApp" 8082
cscript /nologo pingsolr.js
start http://localhost:8082/
exit