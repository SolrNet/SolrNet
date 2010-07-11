@echo off
cd solr-1.4.0
md logs
start java -jar start.jar
cd ..
start tools\cassini\cassini.exe "%cd%\SampleSolrApp" 8082
cscript /nologo pingsolr.js
start http://localhost:8082/
exit