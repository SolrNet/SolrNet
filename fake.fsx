#I @"lib"
#r "FakeLib.dll"
#r "System.Xml.Linq"

open System
open System.Xml.Linq
open Fake

let solr = "solr-1.4.0"

// helper functions
let (.>) (a: #seq<XElement>) (b: string) = Extensions.Descendants(a, xname b)
let startsWith s (n: XElement) = n.Value.StartsWith s
let contains s (n: XElement) = n.Value.Contains s
let setValue s (n: XElement) = n.Value <- s
let replaceValue orig repl (n: XElement) = n.Value <- n.Value.Replace((orig:string), (repl:string))
let httpGet = Fake.REST.ExecuteGetCommand null null

module Solr =
    let private cmdline = "-DSTOP.PORT=8079 -DSTOP.KEY=secret -jar start.jar"
    let private execParam cmd = 
        { WorkingDirectory = solr
          Program = "java"
          CommandLine = cmdline + " " + cmd
          Args = [] }
    let start() = 
        asyncShellExec (execParam "") |> Async.StartAsTask |> ignore
        let watch = System.Diagnostics.Stopwatch.StartNew()
        while httpGet "http://localhost:8983/solr/admin/ping" = null do
            if watch.Elapsed > (TimeSpan.FromSeconds 10.)
                then failwith "Solr test instance didn't work"
            System.Threading.Thread.Sleep 500
    let stop() =
        shellExec (execParam "--stop") |> ignore
