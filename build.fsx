#I @"lib"
#r "FakeLib.dll"
#r "Fake.Gallio.dll"

open System
open System.Linq
open Fake

let (=.) a b = StringComparer.InvariantCultureIgnoreCase.Compare(a,b) = 0
let flip f x y = f y x
let def = flip defaultArg
let anyOf l e = l |> Seq.exists (fun i -> i =. e)

let config =
    fsi.CommandLineArgs
    |> Seq.tryFind (anyOf ["/debug"; "/release"])
    |> def "/debug"
    |> fun e -> e.Substring(1)

let target = 
    fsi.CommandLineArgs
    |> Seq.skip 1
    |> Seq.tryFind (fun c -> not(c.StartsWith "/"))
    |> def "BuildAll"

let slnBuild sln x = 
    sln |> build (fun p -> { p with 
                                Targets = [x] 
                                Properties = ["Configuration", config] })
let mainSln = slnBuild "solrnet.sln"
let sampleSln = slnBuild "SampleSolrApp.sln"

Target "Clean" (fun _ -> 
    mainSln "Clean"
    sampleSln "Clean"
    DeleteDir "merged"
)

Target "Build" (fun _ -> mainSln "Rebuild")
Target "BuildSample" (fun _ -> sampleSln "Rebuild")

Target "BuildAll" DoNothing

Target "Test" (fun _ ->
    let testAssemblies = !+ "**/bin/**/*Tests.dll" |> ScanImmediately
    testAssemblies |> Gallio.Run (fun p -> { p with Filters = "exclude Category: Integration" })
)

Target "Merge" (fun _ ->
    DeleteDir "merged"
    CreateDir "merged"
    let libs = ["SolrNet"; "SolrNet.DSL"; "HttpWebAdapters"; "Castle.Facilities.SolrNetIntegration"; "Ninject.Integration.SolrNet"; "NHibernate.SolrNet"; "Structuremap.SolrNetIntegration"]
    let dlls = libs |> List.map (fun l -> l + ".dll")
    let dirs = libs |> List.map (fun l -> l @@ "bin" @@ config)
    let main = "SolrNet\\bin" @@ config @@ "SolrNet.dll"
    let output = "merged" @@ dlls.[0]
    ILMerge (fun p -> { p with 
                            ToolPath = "lib\\ilmerge.exe"
                            Libraries = dlls |> Seq.skip 1 |> Seq.toList
                            SearchDirectories = dirs
                            Internalize = InternalizeExcept "ilmerge.exclude"
                            XmlDocs = true
                       }) output main
)

"Test" <== ["BuildAll"]
"BuildAll" <== ["Build";"Merge";"BuildSample"]


// start build
Run target