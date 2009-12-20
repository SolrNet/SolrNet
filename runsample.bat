@echo off
cd solr-1.4.0
md logs
start java -jar start.jar
cd ..
start tools\cassini\cassini.exe "%cd%\SampleSolrApp" 8082
ping 127.0.0.1 -n 2 -w 2000 > nul
tools\curl -I -m5 http://localhost:8983/solr >nul
tools\curl -I -m5 http://localhost:8082 >nul
start http://localhost:8082/
exit