#I @"lib"
#r "FakeLib.dll"
#r "NuGet.Core.dll"
#r "System.Xml.Linq"

open System
open System.IO
open Fake
open Fake.FileUtils

let solr = "solr-1.4.0"

// helper functions

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

module Nu =
    open NuGet

    let build version name desc dependencies =
        let builder = 
            PackageBuilder(
                Id = name,
                Title = name,
                Version = Version version,
                Description = desc,
                LicenseUrl = Uri("http://www.apache.org/licenses/LICENSE-2.0"),
                Language = "en-US",
                ProjectUrl = Uri("http://code.google.com/p/solrnet")
            )
        builder.Authors.Add "Mauricio Scheffer and contributors" |> ignore
        builder.Owners.Add "Mauricio Scheffer" |> ignore
        let buildFiles d =
            !+ ("nuget" @@ d @@ "*") -- ("nuget" @@ d)
            |> Scan
            |> Seq.map (fun f -> PhysicalPackageFile(SourcePath = f, TargetPath = d @@ Path.GetFileName(f)))
        let files = ["lib";"content"] |> Seq.collect buildFiles
        builder.Files.AddRange (files |> Seq.map (fun i -> upcast i))
        let deps = 
            dependencies
            |> Seq.map (fun (id,v) -> PackageDependency(id = id, versionSpec = VersionUtility.ParseVersionSpec v))
        builder.Dependencies.AddRange deps
        use fs = File.Create (sprintf "%s.%s%s" name version Constants.PackageExtension)
        builder.Save fs

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
    let sha1() = 
        try
            let headref = (File.ReadAllLines ".git\\HEAD").[0]
            let headref = headref.Substring(5) // assume ref
            (File.ReadAllLines (".git" @@ headref)).[0]
        with :? DirectoryNotFoundException as e -> null
