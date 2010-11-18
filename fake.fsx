#I @"lib"
#r "FakeLib.dll"
#r "System.Xml.Linq"

open System
open Fake

let solr = "solr-1.4.0"

// helper functions

let (!.) a = !+ a |> Scan

let httpGet = Fake.REST.ExecuteGetCommand null null

let ignoreEx (f: 'a -> 'b) a = try f a with e -> Unchecked.defaultof<'b>

[<AutoOpen>]
module Xml =
    open System.Xml.Linq

    let (.>) (a: #seq<XElement>) (b: string) = Extensions.Descendants(a, xname b)
    let startsWith s (n: XElement) = n.Value.StartsWith s
    let contains s (n: XElement) = n.Value.Contains s
    let setValue s (n: XElement) = n.Value <- s
    let replaceValue orig repl (n: XElement) = n.Value <- n.Value.Replace((orig:string), (repl:string))

module Solr =
    let private cmdline = "-DSTOP.PORT=8079 -DSTOP.KEY=secret -jar start.jar"
    let start() = 
        Shell.AsyncExec("java", cmdline, dir = solr) |> Async.Ignore |> Async.Start
        let watch = System.Diagnostics.Stopwatch.StartNew()
        while httpGet "http://localhost:8983/solr/admin/ping" = null do
            if watch.Elapsed > (TimeSpan.FromSeconds 10.)
                then failwith "Solr test instance didn't work"
            System.Threading.Thread.Sleep 500
    let stop() =
        Shell.Exec("java", cmdline + " --stop", dir = solr) |> ignore

module Git =
    let private gitTimeOut = TimeSpan.FromSeconds 10.

    /// Runs the git command and returns the first line of the result
    let run command =
        try
            let ok,msg,errors = 
                ExecProcessAndReturnMessages (fun info ->  
                  info.FileName <- @"lib\tgit.exe"
                  info.WorkingDirectory <- "."
                  info.Arguments <- command) gitTimeOut
            try
                msg.[0]
            with 
            | exn -> failwithf "Git didn't return a msg.\r\n%s" (errors |> separated "\n")
        with 
        | exn -> failwithf "Could not run \"git %s\".\r\nError: %s" command exn.Message
 
    let describe() = run "describe --tags"
    let sha1() = run "rev-parse HEAD"