@echo off
msbuild /p:Configuration=Release /m SolrNet.sln
runsample
