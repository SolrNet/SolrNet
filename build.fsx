#I @"lib"
#r "FakeLib.dll"
#r "Fake.Gallio.dll"

open System
open Fake

type BuildConfig = Debug | Release

let join sep (s: string seq) = String.Join(sep, s)

/// Runs a msbuild project
let msbuild targets properties project =
    let msBuildExe = "msbuild.exe"
    traceStartTask "MSBuild" project
    let targets = sprintf "/target:%s" (targets |> join ";") |> toParam
    let props = 
        properties
          |> Seq.map (fun (key,value) -> sprintf " /p:%s=%s " key value)
          |> separated ""
 
    let args = toParam project + targets + props + " /m"
    logfn "Building project: %s\n  %s %s" project msBuildExe args
    if not (execProcess3 (fun info ->  
        info.FileName <- msBuildExe
        info.Arguments <- args) TimeSpan.MaxValue)
    then failwithf "Building %s project failed." project
    traceEndTask "MSBuild" project

let slnTarget x = Target x (fun _ -> msbuild [x] [] "solrnet.sln")
slnTarget "Clean"
slnTarget "Rebuild"
Target "Test" (fun _ ->
    let testAssemblies = !+ "**/bin/**/*Tests.dll" |> ScanImmediately
    //testAssemblies |> Seq.iter (printfn "%s")
    testAssemblies |> Gallio.Run (fun p -> { p with Filters = "exclude Category: Integration" })
)

For "Test" <| Dependency "Rebuild"

// start build
Run "Test"