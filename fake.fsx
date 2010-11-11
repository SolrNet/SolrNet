#I @"lib"
#r "FakeLib.dll"
#r "System.Xml.Linq"

open System
open System.IO
open System.Xml.Linq
open Fake

let solr = "solr-1.4.0"

// helper functions
let httpGet = Fake.REST.ExecuteGetCommand null null

[<AutoOpen>]
module Xml =
    let (.>) (a: #seq<XElement>) (b: string) = Extensions.Descendants(a, xname b)
    let startsWith s (n: XElement) = n.Value.StartsWith s
    let contains s (n: XElement) = n.Value.Contains s
    let setValue s (n: XElement) = n.Value <- s
    let replaceValue orig repl (n: XElement) = n.Value <- n.Value.Replace((orig:string), (repl:string))

[<AutoOpen>]
module FileUtils =
    let flip f x y = f y x
    let rm_rf f = 
        if Directory.Exists f
            then DeleteDir f
            else DeleteFile f
    let mkdir = CreateDir
    let cp_r src dest = 
        if Directory.Exists src
            then CopyDir dest src allFiles
            else CopyFile dest src
    let cp = flip CopyFile
    let chdir = Directory.SetCurrentDirectory
    let cd = chdir
    let pwd = Directory.GetCurrentDirectory

type Shell() =
    static member private GetParams (cmd, ?args, ?dir) =
        let args = defaultArg args ""
        let dir = defaultArg dir (Directory.GetCurrentDirectory())
        { WorkingDirectory = dir
          Program = cmd
          CommandLine = args 
          Args = [] }
        
    static member Exec (cmd, ?args, ?dir) = 
        shellExec (Shell.GetParams(cmd, ?args = args, ?dir = dir))

    static member AsyncExec (cmd, ?args, ?dir) =
        asyncShellExec (Shell.GetParams(cmd, ?args = args, ?dir = dir))

module Solr =
    let private cmdline = "-DSTOP.PORT=8079 -DSTOP.KEY=secret -jar start.jar"
    let start() = 
        Shell.AsyncExec("java", cmdline, dir = solr) |> Async.StartAsTask |> ignore
        let watch = System.Diagnostics.Stopwatch.StartNew()
        while httpGet "http://localhost:8983/solr/admin/ping" = null do
            if watch.Elapsed > (TimeSpan.FromSeconds 10.)
                then failwith "Solr test instance didn't work"
            System.Threading.Thread.Sleep 500
    let stop() =
        Shell.Exec("java", cmdline + " --stop", dir = solr) |> ignore
